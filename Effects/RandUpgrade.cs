using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class RandUpgrade : IEffect
{
    public string Name => "Random Upgrade";
    public EffectType Outcome => EffectType.Good;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Useful box";

    public void Use(PlayerAvatar roller)
    {
        Vector3 spawnPos = roller.transform.position + roller.transform.forward * 2.5f;
        Networker.Instance.SpawnItem("Upgrade", spawnPos);
    }

}