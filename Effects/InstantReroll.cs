using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class InstantReroll : IEffect
{
    public string Name => "Instant Reroll";
    public EffectType Outcome => EffectType.Bad;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Try again and again and again!";

    public void Use(PlayerAvatar roller)
    {
        try
        {
            List<GameObject> diceList = new List<GameObject>();
            foreach (var name in RepoDice.RegisteredDiceNames)
            {
                diceList.Add(Misc.GetValuableByName(name));
            }
        
            GameObject randomPrefab = diceList[UnityEngine.Random.Range(0, diceList.Count)];
            Vector3 spawnPos = (roller.transform.position + roller.transform.forward);
            var dice = Networker.Instance.spawnValuable(randomPrefab, spawnPos, 3, useList:true, additionalPrefabs:diceList);
            foreach (GameObject d in dice)
            {
                var dbh = d.GetComponent<DieBehaviour>();
                dbh.fromRainbow = true;
                dbh.StartCoroutine(dbh.delayedRoll());
            }
        }
        catch (Exception e)
        {
            RepoDice.SuperLog(e.StackTrace+ " " + e.Message);
        }
        
    }
}