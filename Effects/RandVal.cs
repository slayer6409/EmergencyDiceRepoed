using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class RandVal : IEffect
{
    public string Name => "Random Valuable";
    public EffectType Outcome => EffectType.Good;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Could be worthless?";

    public void Use(PlayerAvatar roller)
    {
        Vector3 spawnPos = roller.transform.position + roller.transform.forward * 2.5f;
        GameObject? randomPrefab = Misc.GetRandomValuable();
        Networker.Instance.spawnValuable(randomPrefab, spawnPos, 1);
    }

}