using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class SmolBigNuke : IEffect
{
    public string Name => "Smol Big Nuke";
    public EffectType Outcome => EffectType.Good;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Big Boom Tiny Package";

    public void Use(PlayerAvatar roller)
    {
        var randItem = Misc.GetValuableByName("Valuable Nuke");
        Vector3 spawnPos = roller.transform.position + roller.transform.forward * 2.5f;
        Networker.Instance.spawnValuable(randItem, spawnPos, 1, true, new Vector3(0.25f, 0.25f, 0.25f));
    }
}