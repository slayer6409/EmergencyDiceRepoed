using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class InfiniteStamina : IEffect
{
    public string Name => "InfiniteStamina";
    public EffectType Outcome => EffectType.Great;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "You can run!";

    public void Use(PlayerAvatar roller)
    {
        if (SemiFunc.IsMultiplayer())
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Networker.Instance.photonView.RPC("infiniteStaminaRPC", RpcTarget.Others, roller.photonView.ViewID);
                Networker.Instance.infiniteStaminaRPC(roller.photonView.ViewID);
            }
        }
        else
        {
            Networker.Instance.infiniteStaminaRPC(roller.photonView.ViewID);
        }
    }
}