using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class PitchUp : IEffect
{
    public string Name => "Pitch Up";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "I got into the balloons again!";

    public void Use(PlayerAvatar roller)
    {
        Networker.Instance.photonView.RPC("adjustPitchRPC", RpcTarget.All,roller.photonView.ViewID, 1.5f, 30);
    }

}