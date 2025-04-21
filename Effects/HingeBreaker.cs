using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using REPOLib.Objects;
using UnityEngine;

namespace RepoDice.Effects;

public class HingeBreaker : IEffect
{
    public string Name => "Hinge Breaker";
    public EffectType Outcome => EffectType.Mixed;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Sorry I Forted";
    public void Use(PlayerAvatar roller)
    {
        PhysGrabHinge[] allHinges = GameObject.FindObjectsOfType<PhysGrabHinge>();
        foreach (var hinge in allHinges)
        {
            if(!hinge.transform.parent.gameObject.name.ToUpper().Contains("DOOR"))continue;
            if (!hinge.dead && !hinge.broken)
            {
                hinge.HingeBreakRPC(); // Locally apply all effects
                if (PhotonNetwork.IsMasterClient)
                {
                    hinge.photon.RPC("HingeBreakRPC", RpcTarget.All);
                    hinge.GetComponent<Rigidbody>()?.AddExplosionForce(5000f, hinge.transform.position - hinge.transform.forward * 2f, 5f);
                    var phys = hinge.transform.gameObject.GetComponent<PhysGrabObject>();
                    if (phys == null) phys = hinge.transform.parent.gameObject.GetComponent<PhysGrabObject>();
                    if (phys == null) phys = hinge.transform.parent.gameObject.GetComponentInChildren<PhysGrabObject>();
                    if (phys != null) phys.SetPositionLogic(LevelGenerator.Instance.LevelPathTruck.transform.position,LevelGenerator.Instance.LevelPathTruck.transform.rotation);
                    if (phys == null) RepoDice.SuperLog("Unknown Door Detected");
                }
            }
        }
    }
  

}