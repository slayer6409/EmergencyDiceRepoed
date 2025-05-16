using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using Photon.Pun;
using Photon.Realtime;
using RepoDice.Effects;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RepoDice.Dice;

public abstract class DieBehaviour : Trap
{
    public bool fromRainbow = false;
    public static List<IEffect> AllowedEffects = new List<IEffect>();
    public static List<IEffect> AllEffects = new List<IEffect>();
    public Dictionary<int, EffectType[]> RollToEffect = new Dictionary<int, EffectType[]>();
    public bool isRolling = false;
    protected GameObject DiceModel;
    public List<IEffect> Effects = new List<IEffect>();
    public Rigidbody rb;
    public PhysGrabObject physGrabObject;
    public PlayerAvatar? lastHolder;
    public static List<ConfigEntry<bool>> effectConfigs = new List<ConfigEntry<bool>>();
    public LayerMask groundMask = ~0;
    public bool isItem = false;
    public ItemToggle itemToggle;
    public bool prevToggleState;
    public bool rolledFromE = false;
    public PhotonView view;
    private bool hasRolled = false;
    
    public virtual void SetupDiceEffects()
    {
        Effects = new List<IEffect>(AllowedEffects);
    }
    public virtual void SetupRollToEffectMapping()
    {
        RollToEffect.Add(1, new EffectType[] { EffectType.Awful });
        RollToEffect.Add(2, new EffectType[] { EffectType.Bad });
        RollToEffect.Add(3, new EffectType[] { EffectType.Mixed, EffectType.Bad });
        RollToEffect.Add(4, new EffectType[] { EffectType.Mixed, EffectType.Good });
        RollToEffect.Add(5, new EffectType[] { EffectType.Good });
        RollToEffect.Add(6, new EffectType[] { EffectType.Great });
    }
    public void Start()
    {
        PhysGrabObjectImpactDetector component = GetComponent<PhysGrabObjectImpactDetector>();
        if (component == null)
        {
            RepoDice.Logger.LogError("Failed to get PhysGrabObjectImpactDetector component!");
            return;
        }
        itemToggle = gameObject.GetComponent<ItemToggle>();
        physGrabObject = GetComponent<PhysGrabObject>();
        rb = GetComponent<Rigidbody>();
        if (!isItem)
        {
            Value val = ScriptableObject.CreateInstance<Value>();
            val.valueMin = Math.Max(0, 180);
            val.valueMax = Math.Max(val.valueMin, 2250);
        }
        DiceModel = gameObject.transform.Find("Model").gameObject;
        view = GetComponent<PhotonView>();
        SetupDiceEffects();
        SetupRollToEffectMapping();
    }

    public IEnumerator delayedRoll()
    {
        yield return new WaitForSeconds(0.5f);
        isRolling = true;
        BlockPickupAndRollRPC();
    }

    //I have it like this, so I can possibly make things happen in the shop
    public virtual bool doShopStuff()
    {
        return RunManager.instance.levelCurrent == RunManager.instance.levelShop;
    }
    
    public void FixedUpdate()
    { 
        if (isRolling) return;

        if (doShopStuff()) return;

        if (itemToggle != null && itemToggle.toggleState != prevToggleState)
        {
            isRolling = true;
            rolledFromE = true;
            if(SemiFunc.IsMasterClientOrSingleplayer()) BlockPickupAndRollRPC();
            else view.RPC(nameof(BlockPickupAndRollRPC), RpcTarget.MasterClient);
        }
        prevToggleState = itemToggle != null && itemToggle.toggleState;
        
        if (physGrabObject.playerGrabbing.Count == 1 && lastHolder != physGrabObject.playerGrabbing[0].playerAvatar)
        {
            lastHolder = physGrabObject.playerGrabbing[0].playerAvatar;
            return;
        }
        
        if (isRolling) return;
        if(physGrabObject.grabbed)return;
        
        if (rb.velocity.magnitude > RepoDice.throwSpeed.Value)
        {
            isRolling = true;
            if (SemiFunc.IsMasterClientOrSingleplayer()) BlockPickupAndRollRPC();
        }
    }

    
    [PunRPC]
    public void BlockPickupAndRollRPC()
    {
       
        if (!SemiFunc.IsMultiplayer())
        {
            setStuff();
        }
        else
        {
            view.RPC(nameof(setStuff),RpcTarget.All);
        }

        if (lastHolder == null) lastHolder = Misc.GetRandomAlivePlayer();

        if (lastHolder != null)
        {
            StartCoroutine(waitForRoll());
        }
        else
        {
            RepoDice.Logger.LogWarning("Tried to roll but no player found.");
        }
    }

    [PunRPC]
    public void setStuff()
    {
        physGrabObject.grabDisableTimer = 4f;
        StartCoroutine(doSillyShit());
    }

    public IEnumerator doSillyShit()
    {
        while (isRolling)
        {
            foreach (var player in new List<PhysGrabber>(physGrabObject.playerGrabbing))
            {
                player.ReleaseObject();
            }
            if (physGrabObject.playerGrabbing.Count == 0)
            { 
                if (rolledFromE && SemiFunc.IsMasterClientOrSingleplayer())
                { 
                    if (rb != null)
                    {
                        Vector3 randomDir = Random.onUnitSphere;
                        randomDir.y = Mathf.Abs(randomDir.y); 
                        float force = Random.Range(6f, 15f);   
                        rb.AddForce(randomDir * force, ForceMode.Impulse);
                        RepoDice.SuperLog($"rolling with {force}");
                        rolledFromE = false;
                    }
                }
            }
            yield return null;
        }
    }
    public IEnumerator waitForRoll()
    {
        float timer = 0f;
        float SpinVelocity = 0f;
        float SpinAcceleration = 1000f;

        while (timer < 3)
        {
            SpinVelocity += SpinAcceleration * Time.deltaTime;
            transform.Rotate(Vector3.up, SpinVelocity * Time.deltaTime);
            transform.Rotate(Vector3.forward, SpinVelocity * Time.deltaTime);
            transform.Rotate(Vector3.right, SpinVelocity * Time.deltaTime);

            timer += Time.deltaTime;
            yield return null;
        }
        try
        {
            doRoll();
        }
        catch(Exception e)
        {
            RepoDice.Logger.LogError(e.Message);
            isRolling = false;
            hasRolled = false;
        }
        yield return new WaitForSeconds(0.5f);
        if(isRolling) doDestroy();
    }

    public void doRoll()
    {
        if (hasRolled)
        {
            RepoDice.SuperLog("Roll() was called more than once!");
            return;
        }
        hasRolled = true;
        Roll();
    }

    public virtual void doDestroy()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
    
    bool IsGrounded(Rigidbody rgbdy, float checkDistance = 0.1f, LayerMask mask = default)
    { 
        if (mask == default) mask = groundMask;
        return Physics.Raycast(rgbdy.position, Vector3.down, checkDistance + 0.1f, groundMask);
    }
    
    IEnumerator WaitUntilGrounded(float checkDistance = 0.1f, LayerMask groundMask = default)
    {
        
        float timeout = 10f;
        float elapsed = 0f;

        while (!IsGrounded(rb, checkDistance, groundMask))
        {
            if (elapsed >= timeout)
            {
                RepoDice.Logger.LogWarning("Ground check timed out.");
                break;
            }
            elapsed += Time.deltaTime;
            yield return null;
        }

        physGrabObject.alterMassValue=9999f; 
        StartCoroutine(waitForRoll());
    }

    public void explodeMachoAndGlitch(int chance)
    {
        var allPlayers = SemiFunc.PlayerGetAll();
        foreach (var player in allPlayers)
        {
            if (player.steamID == RepoDice.glitchSteamID || player.steamID == RepoDice.machoSteamID)
            {
                if (player.steamID == RepoDice.machoSteamID) chance += 10;
                var randomRange = Random.Range(0, 101);
                if (randomRange > chance) continue;
                var pv = player.photonView.ViewID;
                Networker.Instance.ExplodeRandomRPC(pv);
                Networker.Instance.photonView.RPC("ExplodeRandomRPC", RpcTarget.Others, pv);
            }
        }
    }
    public virtual void Roll()
    {
        int diceRoll = UnityEngine.Random.Range(1, 7);
        IEffect randomEffect = GetRandomEffect(diceRoll, Effects);
        if (randomEffect == null) return;
        if(randomEffect == new InstantReroll()) if(fromRainbow) randomEffect = GetRandomEffect(diceRoll+1, Effects);
        Networker.Instance.photonView.RPC("LogToAllRPC", RpcTarget.Others,$"Rolling {randomEffect.Name}");
        Networker.Instance.LogToAllRPC($"Rolling {randomEffect.Name}");
        string messageToSay = $"Rolling {randomEffect.Name}";
        if(!RepoDice.SpoilerMode.Value) messageToSay = randomEffect.Tooltip;
        lastHolder.photonView.RPC("ChatMessageSendRPC", RpcTarget.All, messageToSay, false);
        
        randomEffect.Use(lastHolder);
        explodeMachoAndGlitch(1);
    }

    public void playDiceSound(EffectType effectType)
    {
        string sound = GetSoundKey(effectType);
        Networker.Instance.photonView.RPC("PlayAudioAtPoint", RpcTarget.Others, this.transform.position, sound);
        Networker.Instance.PlayAudioAtPoint(this.transform.position, sound);
    }
    private string GetSoundKey(EffectType effectType)
    {
        return effectType switch
        {
            EffectType.Awful => "Bell2",
            EffectType.Bad => "Bad1",
            _ => "Good2"
        };
    }
    public virtual IEffect? GetRandomEffect(int diceRoll, List<IEffect> effects)
    {
        List<IEffect> rolledEffects = new List<IEffect>();
        if(effects.Count == 0) effects = new List<IEffect>(Effects);
        foreach (IEffect effect in effects)
            if (RollToEffect[diceRoll].Contains(effect.Outcome))
                rolledEffects.Add(effect);

        if (rolledEffects.Count == 0) return null;
        int randomEffectID = UnityEngine.Random.Range(0, rolledEffects.Count);
        return rolledEffects[randomEffectID];
    }
    
    public static void Config()
    {
        RepoDice.MainRegisterNewEffect(new Alarm());
        RepoDice.MainRegisterNewEffect(new BEEGGNOME());
        RepoDice.MainRegisterNewEffect(new BigBang());
        RepoDice.MainRegisterNewEffect(new BigPlate());
        RepoDice.MainRegisterNewEffect(new BigTiny());
        RepoDice.MainRegisterNewEffect(new Duckzilla());
        RepoDice.MainRegisterNewEffect(new ExplodeRandom());
        RepoDice.MainRegisterNewEffect(new ExtraStaminaForAll());
        RepoDice.MainRegisterNewEffect(new Gnomes());
        RepoDice.MainRegisterNewEffect(new Hidden());
        RepoDice.MainRegisterNewEffect(new InfiniteStamina());
        RepoDice.MainRegisterNewEffect(new LittleShits());
        RepoDice.MainRegisterNewEffect(new MiniRobe());
        RepoDice.MainRegisterNewEffect(new PitchDown());
        RepoDice.MainRegisterNewEffect(new PitchRandom());
        RepoDice.MainRegisterNewEffect(new PitchUp());
        RepoDice.MainRegisterNewEffect(new RandItem());
        RepoDice.MainRegisterNewEffect(new RandomStat());
        RepoDice.MainRegisterNewEffect(new RandUpgrade());
        RepoDice.MainRegisterNewEffect(new RandUpgradeMulti());
        RepoDice.MainRegisterNewEffect(new RandVal());
        RepoDice.MainRegisterNewEffect(new Robe());
        RepoDice.MainRegisterNewEffect(new ReviveAll());
        RepoDice.MainRegisterNewEffect(new SelectEffect());
        RepoDice.MainRegisterNewEffect(new ScrapJackpot());
        RepoDice.MainRegisterNewEffect(new TinyBig());
        RepoDice.MainRegisterNewEffect(new TinyJeffery());
        RepoDice.MainRegisterNewEffect(new Reroll());
        RepoDice.MainRegisterNewEffect(new UGF());
        RepoDice.MainRegisterNewEffect(new ExtraLife());
        RepoDice.MainRegisterNewEffect(new ExtraLives());
        RepoDice.MainRegisterNewEffect(new AttractAll());
        RepoDice.MainRegisterNewEffect(new RespawnEnemies());
        RepoDice.MainRegisterNewEffect(new ReturnToShip());
        RepoDice.MainRegisterNewEffect(new FreebirdEnemy(), true);
        RepoDice.MainRegisterNewEffect(new HingeBreaker());
        RepoDice.MainRegisterNewEffect(new SemiTransparentDoors());
        RepoDice.MainRegisterNewEffect(new BigFan());
        RepoDice.MainRegisterNewEffect(new DecreasedValue());
        RepoDice.MainRegisterNewEffect(new IncreasedValue());
        RepoDice.MainRegisterNewEffect(new Sauger());
        RepoDice.MainRegisterNewEffect(new JumpscareEffect());
        RepoDice.MainRegisterNewEffect(new ShootyShooter());
        RepoDice.MainRegisterNewEffect(new SeeNoEvil());
        RepoDice.MainRegisterNewEffect(new PerfectHatred());
        RepoDice.MainRegisterNewEffect(new InstantReroll());
        RepoDice.MainRegisterNewEffect(new TurnThatFrown());
        
        RepoDice.MainRegisterNewEffect(new TestDoorStuck(), superDebug:true);
        RepoDice.MainRegisterNewEffect(new GlitchyEnemy(), superDebug:true);
        RepoDice.MainRegisterNewEffect(new GlitchyPlayer(), superDebug:true);
        RepoDice.MainRegisterNewEffect(new FixPlayer(), superDebug:true);
        
        if(RepoDice.WesleysEnemiesPresent)RepoDice.MainRegisterNewEffect(new TinyLostDroid());
        if(RepoDice.WesleysEnemiesPresent)RepoDice.MainRegisterNewEffect(new ShortGusher());
        if(RepoDice.BigNukePresent)RepoDice.MainRegisterNewEffect(new SmolBigNuke());
        
    }
}