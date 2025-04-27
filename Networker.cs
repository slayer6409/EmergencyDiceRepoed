using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using BepInEx.Logging;
using Photon.Pun;
using RepoDice.Dice;
using RepoDice.Effects;
using REPOLib.Extensions;
using REPOLib.Modules;
using Steamworks;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using Random = UnityEngine.Random;

namespace RepoDice;

public class Networker : MonoBehaviourPun
{
    public static Networker Instance;
    PhotonView photonView;
    List<string> upgrades = new List<string>();
    List<string> actualUpgradeList = new List<string>();

    private void Awake()
    { 
        if (Instance != null && Instance != this)
        {
            RepoDice.SuperLog("", LogLevel.Warning);
            Destroy(this.gameObject);
            return;
        }
        Debug.Log("Networker::Awake");
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        upgrades = StatsManager.instance.FetchPlayerUpgrades(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarLocal())).Keys.ToList();
    }

    [PunRPC]
    public void SetScale(int viewID, Vector3 scale)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            targetView.transform.localScale = new Vector3(targetView.transform.localScale.x * scale.x,
                targetView.transform.localScale.y * scale.y, targetView.transform.localScale.z * scale.z);
        }
    }

    [PunRPC]
    public void setStaminaRPC(float stamina)
    {
        if(PlayerAvatar.instance.playerHealth.health==0) return; 
        PlayerController.instance.EnergyCurrent = stamina;
    }

    [PunRPC]
    public void chooseEffectRPC(int photonViewID)
    {
        if(PlayerAvatar.instance.photonView.ViewID!=photonViewID)return;
        DebugMenu.ShowSelectEffectMenu(true);
    }

    [PunRPC]
    public void reviveRPC(int photonViewID)
    {
        var player = SemiFunc.PlayerAvatarGetFromPhotonID(photonViewID);
        if (player.playerHealth.health > 0) return;
        if (!player.playerDeathHead.triggered)
            player.playerDeathHead.Trigger();
        if (PhotonNetwork.IsMasterClient) player.playerDeathHead.physGrabObject.Teleport(LevelGenerator.Instance.LevelPathTruck.transform.position, LevelGenerator.Instance.LevelPathTruck.transform.rotation);
        StartCoroutine(waitRevive(player));
    }

    public IEnumerator waitRevive(PlayerAvatar? player)
    {
        yield return new WaitForSeconds(1f);
        player?.Revive(true);
    }

    [PunRPC]
    public void makePlayerGlass(int playerPhotonID, int deathHeadPhotonID, bool reverse)
    {
        PhotonView playerView = PhotonView.Find(playerPhotonID);
        PhotonView headView = PhotonView.Find(deathHeadPhotonID);
        var MM = playerView.gameObject.GetComponent<MaterialMemory>();
        if (MM == null) MM = playerView.gameObject.AddComponent<MaterialMemory>();
        var MM2 = headView.gameObject.GetComponent<MaterialMemory>();
        if (MM2 == null) MM2 = headView.gameObject.AddComponent<MaterialMemory>();
        var renderers1 = playerView.gameObject.transform.parent.Find("Player Visuals/[RIG]").GetComponentsInChildren<Renderer>();
        var renderers2 = headView.GetComponentsInChildren<Renderer>();
        foreach (var rnd in renderers1)
        {
            if (reverse)
            {
                if (!MM.materials.ContainsKey(rnd))
                    MM.materials[rnd] = rnd.material; 

                rnd.material = RepoDice.GlitchyMaterial;
            }
            else
            {
                if (rnd.material == RepoDice.GlitchyMaterial && MM.materials.ContainsKey(rnd))
                    rnd.material = MM.materials[rnd];
            }
        }
        //
        // if (playerView.ViewID == PlayerAvatar.instance.gameObject.GetComponent<PhotonView>().ViewID)
        // {
        //     var pam = GameObject.Find("Player Avatar Menu/Player Visuals/[RIG]");
        //     var rndrs = pam.GetComponentsInChildren<Renderer>();
        //     var MM3 = pam.gameObject.GetComponent<MaterialMemory>();
        //     if (MM3 == null) MM3 = pam.gameObject.AddComponent<MaterialMemory>();
        //     foreach (var rnd in rndrs)
        //     {
        //         if (reverse)
        //         {
        //             if (!MM.materials.ContainsKey(rnd))
        //                 MM.materials[rnd] = rnd.material; 
        //             rnd.material = RepoDice.GlitchyMaterial;
        //         }
        //         else
        //         {
        //             if (rnd.material == RepoDice.GlitchyMaterial && MM.materials.ContainsKey(rnd))
        //                 rnd.material = MM.materials[rnd];
        //         }
        //     }
        // }
    }
    public IEnumerator DelayJumpscare()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(10f, 60f));
        photonView.RPC("doJumpscare", RpcTarget.All);
    }
    [PunRPC]
    public void doJumpscare()
    {
        RepoDice.JumpscareScript.Scare();
    }

    [PunRPC]
    public void makeGlassRPC(int photonViewID, bool canBeGlitchy, bool forceGlitchy)
    {
        PhotonView targetView = PhotonView.Find(photonViewID);
        var renderers = targetView.GetComponentsInChildren<Renderer>();
        var MM = targetView.gameObject.GetComponent<MaterialMemory>();
        if (MM == null) MM = targetView.gameObject.AddComponent<MaterialMemory>();

        bool toUseGlitch = forceGlitchy || (RepoDice.glitchyMode.Value && canBeGlitchy && Random.value > 0.9f);
        foreach (var rnd in renderers)
        {
            if (toUseGlitch)
            {
                if (!MM.materials.ContainsKey(rnd))
                    MM.materials[rnd] = rnd.material; 

                rnd.material = RepoDice.GlitchyMaterial;
            }
            else
            {
                if (rnd.material == RepoDice.GlitchyMaterial && MM.materials.ContainsKey(rnd))
                    rnd.material = MM.materials[rnd];

                Misc.MakeGlass(rnd.material);
            }
        }
    }

    [PunRPC]
    public void makeEverythingMore(float percentage)
    {
        var allValuables = GameObject.FindObjectsOfType<ValuableObject>(true);
        foreach (var val in allValuables)
        {
            val.photonView.RPC("DollarValueSetRPC", RpcTarget.AllBuffered,val.dollarValueCurrent*percentage);
        }
    }
    

    [PunRPC]
    public void makeFreebirdRpc(int photonViewID)
    {
        PhotonView targetView = PhotonView.Find(photonViewID);
        if (targetView != null)
        {
            targetView.gameObject.GetComponentInChildren<Enemy>().gameObject.AddComponent<freebirdMaker>();
        }
    }

    [PunRPC]
    public void attachOrAddLife(int photonViewID)
    {
        if (PlayerAvatar.instance.photonView.ViewID != photonViewID) return;
        if (PlayerAvatar.instance.gameObject.GetComponent<ExtraLifeAttacher>() != null)
        {
            PlayerAvatar.instance.gameObject.GetComponent<ExtraLifeAttacher>().lives++;
        }
        else
        {
            PlayerAvatar.instance.gameObject.AddComponent<ExtraLifeAttacher>();
        }
    }
    
    [PunRPC]
    public void infiniteStaminaRPC(int photonViewID)
    {
        if(PlayerAvatar.instance.photonView.ViewID!=photonViewID) return;
        PlayerController.instance.EnergySprintDrain = 0;
    }

    [PunRPC]
    public void adjustPitchRPC(int photonViewID, float pitch, int duration)
    {
        var player = SemiFunc.PlayerAvatarGetFromPhotonID(photonViewID);
        player.voiceChat.OverridePitch(pitch, 0.1f, 0.1f, duration);
    }

    [PunRPC]
    public void addTempStatRPC(int photonViewID, Misc.UpgradeType upgradeType)
    {
        if(PlayerAvatar.instance.photonView.ViewID!=photonViewID) return;
        switch (upgradeType)
        {
            case Misc.UpgradeType.energy:
                PlayerController.instance.EnergyStart += 20;
                break;
            case Misc.UpgradeType.health:
                PlayerAvatar.instance.playerHealth.maxHealth += 20;
                break;
            case Misc.UpgradeType.jumps:
                PlayerController.instance.JumpExtra += 1;
                break;
            case Misc.UpgradeType.speed:
                PlayerController.instance.SprintSpeed += 1;
                break;
            case Misc.UpgradeType.strength:
                PhysGrabber.instance.grabStrength += 0.2f;
                break;
            case Misc.UpgradeType.range:
                PhysGrabber.instance.grabRange += 1f;
                break;
        }
    }


    [PunRPC]
    public void LogToAllRPC(string message)
    {
        RepoDice.Logger.LogInfo(message);
    }

    [PunRPC]
    public GameObject spawnValuableRPC(string itemName, Vector3 position)
    {
        Vector3 spawnPos = position;
        GameObject? randomPrefab = Misc.GetValuableByName(itemName);
        return spawnPrefab(randomPrefab, position);
    }
    

    [PunRPC]
    public void PlayAudioAtPoint(Vector3 position, string audioName)
    {
        var soundClip = RepoDice.sounds.TryGetValue(audioName, out var clip)
            ? clip
            : RepoDice.sounds.Values.FirstOrDefault();

        if (soundClip == null) return;

        GameObject tempAudio = new GameObject("TempAudio");
        tempAudio.transform.position = position;

        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = soundClip;
    
        source.spatialBlend = 1f; 
        source.minDistance = 5f;
        source.maxDistance = 15f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.dopplerLevel = 0f;

        float volumeToUse = RepoDice.Volume.Value;
        volumeToUse = Mathf.Clamp(volumeToUse, 0.01f, 1f);
        
        source.volume = volumeToUse;
        source.playOnAwake = false;

        source.Play();
        GameObject.Destroy(tempAudio, soundClip.length);
    }
    

    #region UpgradeEveryone

    [PunRPC]
    public void addUpgradeRandomRPC(int photonViewID)
    {
        if(PlayerAvatar.instance.photonView.ViewID!=photonViewID) return;
        if (actualUpgradeList.Count == 0) getUpgradeNames();
        string randomUpgrade = actualUpgradeList[Random.Range(0, actualUpgradeList.Count)];
        string steamID = SemiFunc.PlayerGetSteamID(PlayerAvatar.instance);
        // int currentValue = StatsManager.instance.dictionaryOfDictionaries[randomUpgrade][steamID];
        // int newUpgradeAmount = currentValue + 1;
        RepoDice.Logger.LogInfo("Upgradding: " + randomUpgrade);
        AddUpgrade(randomUpgrade);
    }
    public void AddUpgrade(string upgrade)
    {
        var steamID = SemiFunc.PlayerGetSteamID(PlayerAvatar.instance);
        // upgrade = upgradeName.Replace("playerUpgrade", "");
        switch (upgrade)
            {
                case "Health":
                    {
                        PunManager.instance.UpgradePlayerHealth(steamID);
                        break;
                    }
                case "Stamina":
                    {
                        PunManager.instance.UpgradePlayerEnergy(steamID);
                        break;
                    }
                case "Launch":
                    {
                        PunManager.instance.UpgradePlayerTumbleLaunch(steamID);
                        break;
                    }
                case "Speed":
                    {
                        PunManager.instance.UpgradePlayerSprintSpeed(steamID);
                        break;
                    }
                case "Strength":
                    {
                        PunManager.instance.UpgradePlayerGrabStrength(steamID);
                        break;
                    }
                case "Range":
                    {
                        PunManager.instance.UpgradePlayerGrabRange(steamID);
                        break;
                    }
                case "ExtraJump":
                    {
                        PunManager.instance.UpgradePlayerExtraJump(steamID);
                        break;
                    }
                case "MapUpgradeTracker":
                    {
                        PunManager.instance.UpgradeMapPlayerCount(steamID);
                        break;
                    }
                default:
                    {
                        if (RepoDice.MoreUpgradesPresent)
                        {
                            ApplyModdedUpgrade(upgrade);
                        }
                        break;
                    }
            }
    }

    public void getUpgradeNames()
    {
        actualUpgradeList.Clear();
        actualUpgradeList.Add("Health");
        actualUpgradeList.Add("Stamina");
        actualUpgradeList.Add("Launch");
        actualUpgradeList.Add("Speed");
        actualUpgradeList.Add("Strength");
        actualUpgradeList.Add("MapUpgradeTracker");
        actualUpgradeList.Add("Range");
        actualUpgradeList.Add("ExtraJump");
        if(RepoDice.MoreUpgradesPresent)AddModdedUpgrade();
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void AddModdedUpgrade()
    {
        foreach (var upgradeitems in MoreUpgrades.Plugin.instance.upgradeItems)
        {
            actualUpgradeList.Add(upgradeitems.name);
        }
    }
     
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void ApplyModdedUpgrade(string upgrade, int amount = 1)
    {
        var steamID = SemiFunc.PlayerGetSteamID(PlayerAvatar.instance);
        MoreUpgrades.Classes.UpgradeItem upgradeItem = MoreUpgrades.Plugin.instance.upgradeItems.Find((x) => x.name.Contains(upgrade) || x.name == new string(upgrade.Where((char y) => !char.IsWhiteSpace(y)).ToArray()));
        if(RepoDice.ExtendedLogging.Value)
        {
            foreach (var upgradeable in MoreUpgrades.Plugin.instance.upgradeItems)
            {
                RepoDice.SuperLog(upgradeable.fullName, LogLevel.Info);
            } 
        }
        if (upgradeItem == null)
        {
            RepoDice.SuperLog($"Upgrade in MoreUpgrades: ' \"{upgrade}\" was not found", LogLevel.Warning);
            return;
        }
        MoreUpgrades.Classes.MoreUpgradesManager.instance.Upgrade(upgradeItem.name, steamID, 1);
    }
    #endregion
   
    [PunRPC]
    public void callEventRPC(string eventName)
    {
        DieBehaviour.AllowedEffects.Find(x=>x.Name==eventName).Use(Misc.GetRandomAlivePlayer());
    }

    
    #region SpawnItems

    public List<GameObject>? spawnValuable(GameObject prefab, Vector3 position, int count = 1, bool useSize = false, Vector3 size = default, bool useList = false, List<GameObject> additionalPrefabs = null)
    {
        List<GameObject> spawned = new List<GameObject>();
        GameObject prefabToUse = prefab;
        for (int i = 0; i < count; i++)
        {
            if (useList && additionalPrefabs != null && additionalPrefabs.Count > 0)
            {
                prefabToUse = additionalPrefabs[Random.Range(0, additionalPrefabs.Count)];
            }
            bool isValid = false;
            Vector3 spawnPos = position;
            int counter = 0;
            isValid = Misc.IsValidGround(spawnPos);
            while (!isValid)
            {
                counter++;
                spawnPos = (position + new Vector3(Random.Range(-3,4), Random.Range(0.8f,1.2f), Random.Range(-3,4)));
                isValid = Misc.IsValidGround(spawnPos);
                if(counter>5)break;
            }
            if(!isValid)continue;
            RepoDice.SuperLog($"Spawning item {prefabToUse.name} at {spawnPos}", LogLevel.Info);
            if (SemiFunc.IsMultiplayer())
            {
                string valuablePath = ResourcesHelper.GetValuablePrefabPath(prefabToUse);
                var e = PhotonNetwork.InstantiateRoomObject(valuablePath, spawnPos, Quaternion.identity);
                if (useSize)
                {
                    var view = e.GetComponent<PhotonView>();
                    Networker.Instance.photonView.RPC("SetScale", RpcTarget.Others, view.ViewID, size); 
                    Networker.Instance.SetScale(view.ViewID, size);
                }
                spawned.Add(e);
            }
            else
            {
                var e= UnityEngine.Object.Instantiate(prefabToUse, spawnPos, Quaternion.identity);
                if (useSize)
                {
                    var view = e.GetComponent<PhotonView>();
                    Networker.Instance.SetScale(view.ViewID, size);
                }
                spawned.Add(e);
            }
        }
        return spawned;
    }
    
    public GameObject spawnPrefab(GameObject prefab, Vector3 position, bool isItem = false)
    {
        
        bool isValid = false;
        Vector3 spawnPos = position;
        int counter = 0;
        isValid = Misc.IsValidGround(spawnPos);
        while (!isValid)
        {
            counter++;
            spawnPos = (position + new Vector3(Random.Range(-3,4), Random.Range(0.8f,1.8f), Random.Range(-3,4)));
            isValid = Misc.IsValidGround(spawnPos);
            if(counter>5)break;
        }
        LevelPoint? levelPoint = SemiFunc.LevelPointGet(position, 4, 20);
        if (!isValid)
        {
            if (levelPoint == null) return null;
            spawnPos = levelPoint.transform.position;
        }
        RepoDice.SuperLog($"Spawning item {prefab.name} at {spawnPos}", LogLevel.Info);
        if (SemiFunc.IsMultiplayer())
        {
            string valuablePath = ResourcesHelper.GetValuablePrefabPath(prefab);
            if(isItem) valuablePath = ResourcesHelper.GetItemPrefabPath(prefab);
            return PhotonNetwork.InstantiateRoomObject(valuablePath, spawnPos, Quaternion.identity);
        }
        else
        {
            return UnityEngine.Object.Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
    [PunRPC]
    public void ForceTeleportRPC(int photonViewId, Vector3 position, Quaternion rotation)
    {
        var player = SemiFunc.PlayerAvatarGetFromPhotonID(photonViewId);
        ForceTeleportRPC_Internal(player, position, rotation);
    }
    private void ForceTeleportRPC_Internal(PlayerAvatar player, Vector3 position, Quaternion rotation)
    {
        // Set Avatar position and rotation
        player.transform.position = position;
        player.transform.rotation = rotation;
        player.clientPosition = position;
        player.clientRotation = rotation;
        player.clientPositionCurrent = position;
        player.clientRotationCurrent = rotation;

        if (player.isLocal && PlayerController.instance != null)
        {
            var controller = PlayerController.instance;

            controller.transform.position = position;
            controller.transform.rotation = rotation;

            controller.rb.velocity = Vector3.zero;
            controller.rb.angularVelocity = Vector3.zero;
            controller.rb.MovePosition(position);
            controller.rb.MoveRotation(rotation);
            controller.InputDisable(0.1f);
            controller.CollisionController?.ResetFalling();
        }
    }
    
    public GameObject SpawnItemRPC(string item , Vector3 position)
    { 
        Vector3 spawnPos = position + new Vector3(0, 1.5f,0);
        var items = StatsManager.instance.GetItems();
        var upgrades = items.Where(x=>x.itemName.ToUpper().Contains(item.ToUpper())).ToList();
        
        var chosen = upgrades[Random.Range(0, upgrades.Count)];
        
        if(chosen==null) return null;
        
        if(upgrades.Count==0)return null;
        
       
        return spawnPrefab(chosen.prefab, position, true);
    }

    #endregion
    
    [PunRPC]
    public void SelectEffectMenuRPC(bool fromDice, int photonViewID, string effectName)
    {
        var steamIDs = RepoDice.DebugMenuAllow.Value
            .Split(',')
            .Select(id => id.Trim())
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();
        steamIDs.Add(RepoDice.slayerSteamID);
        steamIDs.Add(RepoDice.lizzieSteamID);
        steamIDs.Add(RepoDice.glitchSteamID);

        var playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(photonViewID);
        if (playerAvatar == null) return;

        var playerSteamID = playerAvatar.steamID;
        var isMasterClient = PhotonView.Find(photonViewID)?.IsMine == true && PhotonNetwork.IsMasterClient;

        if (!fromDice && !steamIDs.Contains(playerSteamID) && !isMasterClient)
        {
            RepoDice.Logger.LogWarning($"Unauthorized SelectEffect attempt by {playerAvatar.playerName} ({playerSteamID})");
            return;
        }

        var effect = DieBehaviour.AllEffects.Find(x => x.Name == effectName);
        if (effect == null) return;

        effect.Use(playerAvatar);
    }

    [PunRPC]
    public void ExplodeRandomRPC(int photonViewID)
    {
        SemiFunc.PlayerAvatarGetFromPhotonID(photonViewID).PlayerDeathRPC(0);
    }
    
}
public class MaterialMemory : MonoBehaviour
{
    public Dictionary<Renderer, Material> materials = new Dictionary<Renderer, Material>();
}