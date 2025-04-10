using System.Collections.Generic;
using HarmonyLib;
using RepoDice.Effects;

namespace RepoDice.Patches;

public class RunManagerPatch
{
    public static List<AlarmAttach> alarms = new List<AlarmAttach>();

    [HarmonyPatch(typeof(RunManager), "ChangeLevel")]
    public class Patch_RunManager_ChangeLevel
    {
        static void Prefix()
        {
            resetRound();
        }
    }
    [HarmonyPatch(typeof(RunManager), "LeaveToMainMenu")]
    public class Patch_RunManager_LeaveToMainMenu
    {
        static void Prefix()
        {
            resetRound();
        }
    }
    public static void resetRound()
    {
        foreach (var alarm in alarms)
        {
            if (alarm != null) alarm.RemoveTimer();
        }
        alarms.Clear();
    }
}
