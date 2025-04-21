using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class PitchRandom : IEffect
{
    public string Name => "Pitch Random";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "This isn't weed";
    
    public void Use(PlayerAvatar roller)
    {
        foreach (var player in SemiFunc.PlayerGetAll())
        {
            Networker.Instance.photonView.RPC("adjustPitchRPC", RpcTarget.All,player.photonView.ViewID, Random.Range(0.5f,2.0f), Random.Range(15,41));
        }
    }
    
}