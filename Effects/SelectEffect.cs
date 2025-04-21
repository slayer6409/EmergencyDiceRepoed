using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class SelectEffect : IEffect
{
    public string Name => "Select an Effect";
    public EffectType Outcome => EffectType.Great;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "I can choose if someone dies";

    public void Use(PlayerAvatar roller)
    {
        Networker.Instance.chooseEffectRPC(roller.photonView.ViewID);
        Networker.Instance.photonView.RPC("chooseEffectRPC", RpcTarget.Others, roller.photonView.ViewID);
    }
}