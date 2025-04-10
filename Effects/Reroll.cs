using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
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
        GameObject randomPrefab = Misc.GetValuableByName(RepoDice.RegisteredDiceNames[Random.Range(0, RepoDice.RegisteredDiceNames.Count)]);
        Vector3 spawnPos = (roller.transform.position + roller.transform.forward);
        Networker.Instance.spawnValuable(randomPrefab, spawnPos, 1);
    }
}