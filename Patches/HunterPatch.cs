using System.Collections;
using HarmonyLib;
using Photon.Pun;
using RepoDice.Effects;
using UnityEngine;

namespace RepoDice.Patches;
[HarmonyPatch(typeof(EnemyHunter), "ShootRPC")]
public static class EnemyHunter_ShootRPC_MultiShot
{
    private static bool isFiringBurst = false;

    [HarmonyPrefix]
    public static bool Prefix(EnemyHunter __instance, Vector3 _hitPosition)
    {
        if (isFiringBurst) return true;

        var shooter = __instance.GetComponent<FastShooterMono>();
        if (!PhotonNetwork.IsMasterClient || shooter == null)
            return true;

        __instance.StartCoroutine(FireBurst(__instance, _hitPosition, shooter.shotsAtOnce));
        return false;
    }

    private static IEnumerator FireBurst(EnemyHunter hunter, Vector3 hitPos, int count)
    {
        isFiringBurst = true;

        for (int i = 0; i < count; i++)
        {
            Vector3 spread = UnityEngine.Random.insideUnitSphere * 0.5f;
            Vector3 spreadPos = hitPos + spread;

            hunter.photonView.RPC("ShootRPC", RpcTarget.All, spreadPos);

            yield return new WaitForSecondsRealtime(0.066f);
        }

        isFiringBurst = false;
    }
}