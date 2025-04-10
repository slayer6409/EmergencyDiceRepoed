using HarmonyLib;
using Photon.Pun;
using REPOLib.Modules;
using Unity.VisualScripting;
using UnityEngine;

namespace RepoDice.Patches;

[HarmonyPatch(typeof(GameplayManager))]
public class GameplayManagerPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPostfix]
    public static void StartPatch()
    {
        if(Networker.Instance != null) return;
        if (SemiFunc.IsMultiplayer())
        {
            if(PhotonNetwork.IsMasterClient) PhotonNetwork.InstantiateRoomObject(RepoDice.networker.name, Vector3.zero, Quaternion.identity);
        }
        else
        {
            UnityEngine.Object.Instantiate(RepoDice.networker, Vector3.zero, Quaternion.identity);
        }
    }
}
