using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class Robe : IEffect
{
    public string Name => "Robe";
    public EffectType Outcome => EffectType.Awful;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Spooky Guy";

    public void Use(PlayerAvatar roller)
    {
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Misc.SpawnEnemy("Robe", 1, spawnPos);
    }
}