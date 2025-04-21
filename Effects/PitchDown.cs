using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class PitchDown : IEffect
{
    public string Name => "Pitch Down";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "I'm on drugs";
    
    public void Use(PlayerAvatar roller)
    {
        Networker.Instance.photonView.RPC("adjustPitchRPC", RpcTarget.All,roller.photonView.ViewID, 0.5f, 30);
    }
    
}