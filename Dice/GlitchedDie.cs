using System;
using System.Collections.Generic;
using RepoDice.Effects;

namespace RepoDice.Dice;

public class GlitchedDie : DieBehaviour
{
    private PhysGrabObjectImpactDetector physGrabObjectImpactDetector;
    private ItemAttributes itemAttributes;
    
    
    void Start()
    {
        physGrabObjectImpactDetector = GetComponent<PhysGrabObjectImpactDetector>();
        itemAttributes = GetComponent<ItemAttributes>();
        base.Start();
    }

    public void LateUpdate()
    {
        
    }

    public override void SetupRollToEffectMapping()
    {
        RollToEffect.Add(1, new EffectType[] { });
        RollToEffect.Add(2, new EffectType[] { EffectType.Awful ,EffectType.Bad, EffectType.Mixed, EffectType.Good });
        RollToEffect.Add(3, new EffectType[] { EffectType.Awful ,EffectType.Bad, EffectType.Mixed, EffectType.Good });
        RollToEffect.Add(4, new EffectType[] { EffectType.Awful ,EffectType.Bad, EffectType.Mixed, EffectType.Good });
        RollToEffect.Add(5, new EffectType[] { EffectType.Awful ,EffectType.Bad, EffectType.Mixed, EffectType.Good });
        RollToEffect.Add(6, new EffectType[] { EffectType.Awful ,EffectType.Bad, EffectType.Mixed, EffectType.Good, EffectType.Great });
    }
    
    public override IEffect? GetRandomEffect(int diceRoll, List<IEffect> effects)
    {
        if (diceRoll == 1) return new GlitchyEnemy();
        return base.GetRandomEffect(diceRoll, effects);
    }

    public override void doDestroy()
    {
        if (SemiFunc.IsMasterClientOrSingleplayer())
        {
            if (!SemiFunc.RunIsShop())
            {
                if (!RepoDice.glitchedRespawn.Value)
                {
                    StatsManager.instance.ItemRemove(itemAttributes.instanceName);
                    physGrabObjectImpactDetector.DestroyObject();
                }
                else
                {
                    base.doDestroy();
                }
            }
        }
    }
}