using System.Collections.Generic;
using RepoDice.Effects;
using UnityEngine;

namespace RepoDice.Dice;

public class Saint : DieBehaviour
{
    public override void SetupRollToEffectMapping()
    {
        RollToEffect.Add(1, new EffectType[] { });
        RollToEffect.Add(2, new EffectType[] { EffectType.Mixed });
        RollToEffect.Add(3, new EffectType[] { EffectType.Good });
        RollToEffect.Add(4, new EffectType[] { EffectType.Good });
        RollToEffect.Add(5, new EffectType[] { EffectType.Great });
        RollToEffect.Add(6, new EffectType[] { });
    }
    public void Start()
    {
        base.Start();
        DiceModel.transform.Find("halo").gameObject.AddComponent<HaloSpin>();
    }

    public override IEffect? GetRandomEffect(int diceRoll, List<IEffect> effects)
    {
        if (diceRoll == 0) return null;
        if (diceRoll == 6) return new SelectEffect();
        return base.GetRandomEffect(diceRoll, effects);
        
    }
}
public class HaloSpin : MonoBehaviour
{
    void Update()
    {
        transform.rotation = Quaternion.Euler(
            Mathf.Sin(Time.time) * 15f,
            Time.time * 30f % 360f,
            0f
        );
    }
}