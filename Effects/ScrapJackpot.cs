using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class ScrapJackpot : IEffect
{
    public string Name => "Scrap Jackpot";
    public EffectType Outcome => EffectType.Great;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Scrap Jackpot";

    public void Use(PlayerAvatar roller)
    {
        DoScrapJackpot(roller, 5);
    }
    public void DoScrapJackpot(PlayerAvatar roller, int amount = 5)
    {
        for (int i = 0; i < amount; i++)
        {
            Vector3 spawnPos = roller.transform.position + roller.transform.forward + new Vector3(Random.Range(1,4),Random.Range(1,2), Random.Range(1,4));
            if(!Misc.IsValidGround(spawnPos))continue;
            GameObject? randomPrefab = Misc.GetRandomValuable();
            if (randomPrefab == null) return;
            if (SemiFunc.IsMultiplayer())
            {
                string valuablePath = ResourcesHelper.GetValuablePrefabPath(randomPrefab);
                PhotonNetwork.InstantiateRoomObject(valuablePath, spawnPos, Quaternion.identity);
            }
            else
            {
                UnityEngine.Object.Instantiate(randomPrefab, spawnPos, Quaternion.identity);
            }
        }
    }
}