using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class Gnomes : IEffect
{
    public string Name => "Gnomes";
    public EffectType Outcome => EffectType.Bad;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Ankle Biters!";

    public void Use(PlayerAvatar roller)
    { 
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Misc.SpawnEnemy("Gnome", 2, spawnPos);
    }
}