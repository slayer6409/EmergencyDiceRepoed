using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class IncreasedValue : IEffect
{
    public string Name => "Increased Value";
    public EffectType Outcome => EffectType.Great;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Oooh Shiny";

    public void Use(PlayerAvatar roller)
    { 
        var allValuables = GameObject.FindObjectsOfType<ValuableObject>(true);
        foreach (var val in allValuables)
        {
            var increaseBy = Random.Range(1.05f, 1.5f);
            val.photonView.RPC("DollarValueSetRPC", RpcTarget.AllBuffered,val.dollarValueCurrent*increaseBy);
        }
    }
}