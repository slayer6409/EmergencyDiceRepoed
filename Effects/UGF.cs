using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class UGF : IEffect
{
    public string Name => "China Shop";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Better Catch them";

    public void Use(PlayerAvatar roller)
    {
        var uraniumPrefabs = Misc.getValuablesWithName("Uranium");
        GameObject randomPrefab = uraniumPrefabs[Random.Range(0, uraniumPrefabs.Count)];
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Networker.Instance.spawnValuable(randomPrefab, spawnPos, 4, useList: true, additionalPrefabs: uraniumPrefabs);
    }
}