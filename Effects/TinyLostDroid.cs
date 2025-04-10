using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class TinyLostDroid : IEffect
{
    public string Name => "Tiny Lost Droid";
    public EffectType Outcome => EffectType.Awful;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Pew Pew";

    public void Use(PlayerAvatar roller)
    { 
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Misc.SpawnAndScaleEnemy("Lost Droid", 1, spawnPos, new Vector3(0.6f,0.6f,0.6f));
    }
}