using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using REPOLib.Objects;
using UnityEngine;

namespace RepoDice.Effects;

public class SemiTransparentDoors : IEffect
{
    public string Name => "Semi Transparent Doors";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "hehehe";
    public void Use(PlayerAvatar roller)
    {
        var doors = GameObject.FindObjectsOfType<Transform>(true) // `true` includes inactive if needed
            .Where(t => t.gameObject.name.ToLowerInvariant().Contains("door") && !t.gameObject.name.ToLowerInvariant().Contains("blocked"))
            .Select(t => t.gameObject)
            .ToList();
        foreach (var door in doors)
        {
            var pv = door.GetComponent<PhotonView>();
            if (pv == null) door.GetComponentInChildren<PhotonView>();
            if (pv == null) door.GetComponentInParent<PhotonView>();
            if (pv != null) Networker.Instance.photonView.RPC("makeGlassRPC", RpcTarget.All, pv.ViewID);
            else
            {
                RepoDice.SuperLog(door.name + "has no photon view");
            }
        }
        
    }
   

}