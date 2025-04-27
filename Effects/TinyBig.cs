using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class TinyBig : IEffect
{
    public string Name => "Tiny Big";
    public EffectType Outcome => EffectType.Good;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "It big but tiny";

    public void Use(PlayerAvatar roller)
    {
        var big = Misc.GetValuablesBySize(Misc.Size.big);
        big.AddRange(Misc.GetValuablesBySize(Misc.Size.veryTall));
        big.AddRange(Misc.GetValuablesBySize(Misc.Size.wide));

        GameObject randomPrefab = big[Random.Range(0, big.Count)];
        Vector3 spawnPos = (roller.transform.position + roller.transform.forward);
        var scale = new Vector3(0.25f, 0.25f, 0.25f);
        Networker.Instance.spawnValuable(randomPrefab, spawnPos, 4, true, scale, true, big);
    }
}