using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using REPOLib.Objects;
using UnityEngine;

namespace RepoDice.Effects;

public class Doors : IEffect
{
    public string Name => "Doors";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Doors";
    public void Use(PlayerAvatar roller)
    {
        // Vector3 spawnPos;
        // for (int i = 0; i < 4; i++)
        // {
        //     spawnPos = (roller.transform.position + roller.transform.forward)+new Vector3(Random.Range(-3,4), Random.Range(0.8f,1.8f), Random.Range(-3,4));
        //     if(!Misc.IsValidGround(spawnPos))continue;
        //     GameObject randomPrefab = doors[Random.Range(0, doors.Count)];
        //     if (SemiFunc.IsMultiplayer())
        //     {
        //         string valuablePath = ResourcesHelper.GetValuablePrefabPath(randomPrefab);
        //         PhotonNetwork.Instantiate(valuablePath, spawnPos, Quaternion.identity);
        //     }
        //     else
        //     {
        //         UnityEngine.Object.Instantiate(randomPrefab, spawnPos, Quaternion.identity);
        //     }
        // }
    }
  

}