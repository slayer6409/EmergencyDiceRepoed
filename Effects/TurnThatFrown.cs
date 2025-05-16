using System.Collections;
using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class TurnThatFrown : IEffect
{
    public string Name => "Turn that frown";
    public EffectType Outcome => EffectType.Bad;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Upsidedown";

    public static int breakTime = 90;
    public static bool isTimerRunning = false;
    public static int IsNeckBroken = 0;
    public void Use(PlayerAvatar roller)
    {
        foreach (PlayerAvatar player in SemiFunc.PlayerGetAll())
        {
            Networker.Instance.photonView.RPC(nameof(Networker.Instance.forceNeckBreak), RpcTarget.All, player.photonView.ViewID);
        }
    }
    public static void BreakNeck()
    {
        IsNeckBroken += 2;
    }
    public static void FixNeck()
    {
        IsNeckBroken = 4;
    }
    public static IEnumerator WaitTime()
    {
        isTimerRunning = true;
        RepoDice.SuperLog($"Breaking neck for {breakTime} seconds");
        yield return new WaitForSeconds(breakTime);
        FixNeck();
        isTimerRunning = false;
    }

}