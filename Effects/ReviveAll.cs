using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class ReviveAll : IEffect
{
    public string Name => "Revive All";
    public EffectType Outcome => EffectType.Great;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Welcome Back!";

    public void Use(PlayerAvatar roller)
    {
        foreach (var player in GameDirector.instance.PlayerList)
        {
            Networker.Instance.photonView.RPC(nameof(Networker.Instance.reviveRPC), RpcTarget.All ,player.photonView.ViewID);
        }
    }
}