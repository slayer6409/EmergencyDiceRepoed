using System;
using RepoDice.Effects;
using UnityEngine;

namespace RepoDice.Dice;

public class Gambler : DieBehaviour
{
    public override void SetupRollToEffectMapping()
    {
        RollToEffect.Add(1, new EffectType[] { EffectType.Awful });
        RollToEffect.Add(2, new EffectType[] { EffectType.Bad });
        RollToEffect.Add(3, new EffectType[] { EffectType.Mixed, EffectType.Bad, EffectType.Good});
        RollToEffect.Add(4, new EffectType[] { EffectType.Mixed, EffectType.Good, EffectType.Bad});
        RollToEffect.Add(5, new EffectType[] { EffectType.Good });
        RollToEffect.Add(6, new EffectType[] { EffectType.Great });
    }
    public void Start()
    {
        base.Start();
        DiceModel.AddComponent<CycleSigns>(); 
    }

}