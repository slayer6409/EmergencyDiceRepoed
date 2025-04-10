using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class ExplodeRandom : IEffect
{
    public string Name => "Explode Random";
    public EffectType Outcome => EffectType.Awful;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Kaboom";

    public void Use(PlayerAvatar roller)
    {
        var pv = Misc.GetRandomAlivePlayer().photonView.ViewID;
        if (SemiFunc.IsMasterClientOrSingleplayer())
        {
           Networker.Instance.ExplodeRandomRPC(pv);
           Networker.Instance.photonView.RPC("ExplodeRandomRPC", RpcTarget.Others, pv);
        }
    }
}