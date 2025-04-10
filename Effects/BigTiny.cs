using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class BigTiny : IEffect
{
    public string Name => "Big Tiny";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "It tiny but big";

    public void Use(PlayerAvatar roller)
    {
        var tiny = Misc.GetValuablesBySize(Misc.Size.tiny);
        tiny.AddRange(Misc.GetValuablesBySize(Misc.Size.small));
        GameObject randomPrefab = tiny[Random.Range(0, tiny.Count)];
        Vector3 spawnPos = (roller.transform.position + roller.transform.forward);
        var scale = new Vector3(4f, 4f, 4f);
        Networker.Instance.spawnValuable(randomPrefab, spawnPos, 4, true, scale, true, tiny);
    }
}