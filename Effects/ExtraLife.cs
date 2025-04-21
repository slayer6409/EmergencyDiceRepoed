using System;
using System.Collections;
using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class ExtraLives : IEffect
{
    public string Name => "Extra Lives for All";
    public EffectType Outcome => EffectType.Great;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Did anything happen???";

    public void Use(PlayerAvatar roller)
    {
        foreach (var player in GameDirector.instance.PlayerList)
        {
            Networker.Instance.attachOrAddLife(player.photonView.ViewID);
            Networker.Instance.photonView.RPC("attachOrAddLife", RpcTarget.Others ,player.photonView.ViewID);
        }
    }
}


public class ExtraLife : IEffect
{
    public string Name => "Extra Life";
    public EffectType Outcome => EffectType.Good;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Did anything happen?";

    public void Use(PlayerAvatar roller)
    {
            Networker.Instance.attachOrAddLife(roller.photonView.ViewID);
            Networker.Instance.photonView.RPC("attachOrAddLife", RpcTarget.Others ,roller.photonView.ViewID);
    }
}


public class ExtraLifeAttacher : MonoBehaviour
{
    public bool isReviving = false;
    public int lives = 1;
    public void Update()
    {
        if(isReviving)return;
        if (PlayerAvatar.instance.playerHealth.health <= 0 && lives > 0) 
        {
            lives--;
            Networker.Instance.StartCoroutine(doRevive());   
            return;
        } 
        if(lives <= 0)removeThis();
        
    }
    
    public IEnumerator doRevive()
    {
        isReviving = true;
        yield return new WaitUntil(() => PlayerAvatar.instance.deadSet);
        yield return new WaitForSeconds(3f);
        Networker.Instance.photonView.RPC("reviveRPC", RpcTarget.All, PlayerAvatar.instance.photonView.ViewID);
        //waiting to account for ping
        yield return new WaitForSeconds(1.1f);
        PlayerAvatar.instance.photonView.RPC("ChatMessageSendRPC", RpcTarget.All, $"I'm back! With {lives} lives remaining!", false);
        isReviving = false;
    }
    public void removeThis()
    {
        Destroy(this);
    }
}