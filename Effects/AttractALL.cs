using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class AttractAll : IEffect
{
    public string Name => "Attract ALL";
    public EffectType Outcome => EffectType.Bad;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Pspspsps";

    public void Use(PlayerAvatar roller)
    {
        EnemyDirector.instance.SetInvestigate(roller.transform.position, 5000f);
    }
}