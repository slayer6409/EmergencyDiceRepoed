using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class SeeNoEvil : IEffect
{
    public string Name => "See No Evil";
    public EffectType Outcome => EffectType.Horrendous;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "See No Evil";

    public void Use(PlayerAvatar roller)
    {
        foreach (var enemy in EnemyDirector.instance.enemiesSpawned)
        {
            Networker.Instance.photonView.RPC(nameof(Networker.Instance.makeGlassRPC), RpcTarget.All, enemy.photonView.ViewID, false, false);
        }
    }
}