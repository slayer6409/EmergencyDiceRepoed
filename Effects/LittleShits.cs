using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class LittleShits : IEffect
{
    public string Name => "Rugrats";
    public EffectType Outcome => EffectType.Bad;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "I hate them";

    public void Use(PlayerAvatar roller)
    {
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Misc.SpawnEnemy("Rugrat",3, spawnPos);
    }
}