using System;
using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class RandomStat : IEffect
{
    public string Name => "Temp Stat";
    public EffectType Outcome => EffectType.Good;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "I feel different";

    public void Use(PlayerAvatar roller)
    {
        var pv = Misc.GetRandomAlivePlayer().photonView.ViewID;
        Misc.UpgradeType value = Misc.GetRandomEnum<Misc.UpgradeType>();
        if(SemiFunc.IsMultiplayer()) Networker.Instance.photonView.RPC(nameof(Networker.Instance.addTempStatRPC), RpcTarget.All,roller.photonView.ViewID,value);
        else Networker.Instance.addTempStatRPC(roller.photonView.ViewID,value);
        
    }
}