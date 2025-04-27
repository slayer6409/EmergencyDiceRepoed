using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class TinyCandy : IEffect
{
    public string Name => "Tiny Candy";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Annoying AF";

    public void Use(PlayerAvatar roller)
    {
        var tiny = Misc.getValuablesWithName("candy");
        GameObject randomPrefab = tiny[Random.Range(0, tiny.Count)];
        Vector3 spawnPos = (roller.transform.position + roller.transform.forward);
        var scale = new Vector3(0.25f, 0.25f, 0.25f);
        Networker.Instance.spawnValuable(randomPrefab, spawnPos, 4, true, scale, true, tiny);
    }
}