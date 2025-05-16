using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class ReturnToShip : IEffect
{
    public string Name => "Return To Ship";
    public EffectType Outcome => EffectType.Good;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Back home I go!";

    public void Use(PlayerAvatar roller)
    { 
        if(SemiFunc.IsMultiplayer()) Networker.Instance.photonView.RPC("ForceTeleportRPC", RpcTarget.All, roller.photonView.ViewID, LevelGenerator.Instance.LevelPathTruck.transform.position,LevelGenerator.Instance.LevelPathTruck.transform.rotation);
            else Networker.Instance.ForceTeleportRPC(roller.photonView.ViewID, LevelGenerator.Instance.LevelPathTruck.transform.position,LevelGenerator.Instance.LevelPathTruck.transform.rotation);
    }
}