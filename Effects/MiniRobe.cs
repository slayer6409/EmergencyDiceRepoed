using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class MiniRobe : IEffect
{
    public string Name => "Mini Robe";
    public EffectType Outcome => EffectType.Awful;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "He Smol";

    public void Use(PlayerAvatar roller)
    { 
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Misc.SpawnAndScaleEnemy("Robe", 1, spawnPos, new Vector3(0.5f,0.5f,0.5f));
    }
}