using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class Sauger : IEffect
{
    public string Name => "Sauger";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "MY BOY";

    public void Use(PlayerAvatar roller)
    {
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Misc.SpawnEnemy("Sauger", 1, spawnPos);
    }
}