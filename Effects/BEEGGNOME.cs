using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class BEEGGNOME : IEffect
{
    public string Name => "BEEGGNOME";
    public EffectType Outcome => EffectType.Bad;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "They BEEG";

    public void Use(PlayerAvatar roller)
    { 
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Misc.SpawnAndScaleEnemy("Gnome", 2, spawnPos, new Vector3(3f, 3f, 3f));
    }
}