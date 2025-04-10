using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class ShortGusher : IEffect
{
    public string Name => "Short Gusher";
    public EffectType Outcome => EffectType.Awful;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "He Short";

    public void Use(PlayerAvatar roller)
    { 
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Misc.SpawnAndScaleEnemy("Gusher", 1, spawnPos, new Vector3(1f,0.5f,1f));
    }
}