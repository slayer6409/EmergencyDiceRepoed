using HarmonyLib;
using UnityEngine;

namespace RepoDice.Patches;

[HarmonyPatch(typeof(GameDirector), "gameStateEnd")]
public class GameDirector_EndPatch
{
    static void Postfix(GameDirector __instance)
    {
        if (__instance.currentState == GameDirector.gameState.EndWait)
        {
            RunManagerPatch.resetRound();
        }
    }
}
