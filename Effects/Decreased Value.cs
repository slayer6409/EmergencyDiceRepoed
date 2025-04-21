using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class DecreasedValue : IEffect
{
    public string Name => "Decreased Value";
    public EffectType Outcome => EffectType.Awful;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "This is worth less now";

    public void Use(PlayerAvatar roller)
    { 
        var allValuables = GameObject.FindObjectsOfType<ValuableObject>(true);
        foreach (var val in allValuables)
        {
            var decreaseBy = Random.Range(0.75f, 0.99f);
            val.photonView.RPC("DollarValueSetRPC", RpcTarget.AllBuffered,val.dollarValueCurrent*decreaseBy);
        }
    }
}