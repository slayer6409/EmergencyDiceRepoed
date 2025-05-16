using HarmonyLib;
using RepoDice.Effects;
using UnityEngine;

namespace RepoDice;

[HarmonyPatch(typeof(PlayerController))]
static class PlayerControllerPatch
{
    [HarmonyPostfix, HarmonyPatch(typeof(PlayerController), "Update")]
    private static void Update_Postfix(PlayerController __instance)
    {
        Transform cam = __instance.cameraGameObjectLocal.transform;
        switch (TurnThatFrown.IsNeckBroken) 
        {
            case 0: return;
            case 1: cam.eulerAngles = new Vector3(cam.eulerAngles.x, cam.eulerAngles.y, 90f); break;
            case 2: cam.eulerAngles = new Vector3(cam.eulerAngles.x, cam.eulerAngles.y, 180f); break;
            case 3: cam.eulerAngles = new Vector3(cam.eulerAngles.x, cam.eulerAngles.y, -90f); break;
            case 4: cam.eulerAngles = new Vector3(cam.eulerAngles.x, cam.eulerAngles.y, 0f); TurnThatFrown.IsNeckBroken = 0; break;
            default: TurnThatFrown.IsNeckBroken = 0; break;
        }
        if(TurnThatFrown.IsNeckBroken!=0 && !TurnThatFrown.isTimerRunning) Networker.Instance.StartCoroutine(TurnThatFrown.WaitTime());
    }
}