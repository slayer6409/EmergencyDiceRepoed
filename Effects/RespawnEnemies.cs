using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class RespawnEnemies : IEffect
{
    public string Name => "Respawn Enemies";
    public EffectType Outcome => EffectType.Awful;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Welcome Back Everyone!";

    public void Use(PlayerAvatar roller)
    {
        foreach (var enemy in EnemyDirector.instance.enemiesSpawned)
        {
            enemy.DespawnedTimerSet(0);
        }
    }
}