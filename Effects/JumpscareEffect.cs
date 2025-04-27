using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class JumpscareEffect : IEffect
{
    public string Name => "Jumpscare";
    public EffectType Outcome => EffectType.Bad;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Did Something Happen?!";

    public void Use(PlayerAvatar roller)
    {
        Networker.Instance.StartCoroutine(Networker.Instance.DelayJumpscare());
    }
}