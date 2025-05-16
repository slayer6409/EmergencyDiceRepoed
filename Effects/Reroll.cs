using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class Reroll : IEffect
{
    public string Name => "Reroll";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Try again!";

    public void Use(PlayerAvatar roller)
    {
        string RandomDice = RepoDice.RegisteredDiceNames[Random.Range(0, RepoDice.RegisteredDiceNames.Count)];
        Vector3 spawnPos = (roller.transform.position + roller.transform.forward);
        if (!RandomDice.Contains("ItemDie"))
        {
            GameObject randomPrefab = Misc.GetValuableByName(RandomDice);
            Networker.Instance.spawnValuable(randomPrefab, spawnPos, 1);
        }
        else
        {
            var allItems = StatsManager.instance.itemDictionary.Values.ToList();
            Item? randomPrefab = allItems.FirstOrDefault(x => x.name == RandomDice.Replace("_ItemDie",""));
            if (randomPrefab != null)
            {
                Networker.Instance.SpawnItem(randomPrefab.itemName, spawnPos);
            }
        }
        
    }
}