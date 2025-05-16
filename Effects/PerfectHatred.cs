using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class PerfectHatred : IEffect
{
    public string Name => "Perfect Hatred";
    public EffectType Outcome => EffectType.Horrendous;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Perfect Hatred";

    public void Use(PlayerAvatar roller)
    {
        var randomPrefab = Misc.GetValuableByName("Sacrificer");
        foreach (var player in SemiFunc.PlayerGetAll())
        {
            if (player.playerHealth.health <= 0) continue;
            var dice = Networker.Instance.spawnValuable(randomPrefab, player.transform.position + player.transform.forward, 1);
            foreach (GameObject d in dice)
            {
                var dbh = d.GetComponent<DieBehaviour>();
                dbh.fromRainbow = false;
                dbh.lastHolder = player;
                dbh.StartCoroutine(dbh.delayedRoll());
            }
        }
    }
}