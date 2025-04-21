using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class BigPlate : IEffect
{
    public string Name => "Big Uranium";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "It big but breakable";

    public void Use(PlayerAvatar roller)
    {
        var uraniumPrefabs = Misc.getValuablesWithName("Uranium");
        GameObject randomPrefab = uraniumPrefabs[Random.Range(0, uraniumPrefabs.Count)];
        Vector3 spawnPos = (roller.transform.position + roller.transform.forward );
        Networker.Instance.spawnValuable(randomPrefab, spawnPos, 1, true, new Vector3(3f, 3f, 3f));
    }
}