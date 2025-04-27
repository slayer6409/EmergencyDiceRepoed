using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class BigFan : IEffect
{
    public string Name => "Big Fan";
    public EffectType Outcome => EffectType.Bad;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "I'm your biggest fan!";

    public void Use(PlayerAvatar roller)
    {
        var uraniumPrefabs = Misc.getValuablesWithName("Fan");
        GameObject randomPrefab = uraniumPrefabs[Random.Range(0, uraniumPrefabs.Count)];
        Vector3 spawnPos = (roller.transform.position + roller.transform.forward );
        var fans = Networker.Instance.spawnValuable(randomPrefab, spawnPos, 1, true, new Vector3(3f, 3f, 3f));
        foreach (var fan in fans)
        {
            Networker.Instance.photonView.RPC("makeGlassRPC",RpcTarget.All,fan.GetComponent<PhotonView>().ViewID, true, false);
        }
    }
}