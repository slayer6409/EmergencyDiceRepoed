using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class RandItem : IEffect
{
    public string Name => "Random Item";
    public EffectType Outcome => EffectType.Good;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Spawns a random Item!";

    public void Use(PlayerAvatar roller)
    {
        var allItems = StatsManager.instance.GetItems();
        var randItem = allItems[Random.Range(0, allItems.Count)];
        Vector3 spawnPos = roller.transform.position + roller.transform.forward * 2.5f;
        Networker.Instance.SpawnItemRPC(randItem.itemName, spawnPos);
    }
}