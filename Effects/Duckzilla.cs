using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class Duckzilla : IEffect
{
    public string Name => "Duckzilla";
    public EffectType Outcome => EffectType.Awful;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "BIG JEFFERY";

    public void Use(PlayerAvatar roller)
    { 
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Misc.SpawnAndScaleEnemy("Duck", 1, spawnPos, new Vector3(3,3,3));
    }
}