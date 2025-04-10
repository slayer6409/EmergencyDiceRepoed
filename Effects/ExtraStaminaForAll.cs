using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class ExtraStaminaForAll : IEffect
{
    public string Name => "Extra Stamina???";
    public EffectType Outcome => EffectType.Great;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "You can run!";

    public void Use(PlayerAvatar roller)
    {
        
        if (SemiFunc.IsMultiplayer())
        {
            if (PhotonNetwork.IsMasterClient)
            {
                Networker.Instance.photonView.RPC("setStaminaRPC", RpcTarget.Others, 420.69f);
                Networker.Instance.setStaminaRPC(420.69f);
            }
        }
        else
        {
            Networker.Instance.setStaminaRPC(420.69f);
        }
    }
}