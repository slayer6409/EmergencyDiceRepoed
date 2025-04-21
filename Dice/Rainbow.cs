using System.Collections.Generic;
using Photon.Pun;
using RepoDice.Effects;
using RepoDice.Visual;
using UnityEngine;

namespace RepoDice.Dice;

public class Rainbow : DieBehaviour
{
    public override void SetupRollToEffectMapping()
    {
        RollToEffect.Add(1, new EffectType[] { EffectType.Bad, EffectType.Awful });
        RollToEffect.Add(2, new EffectType[] { EffectType.Mixed, EffectType.Bad, EffectType.Awful });
        RollToEffect.Add(3, new EffectType[] { EffectType.Mixed });
        RollToEffect.Add(4, new EffectType[] { EffectType.Mixed });
        RollToEffect.Add(5, new EffectType[] { EffectType.Mixed });
        RollToEffect.Add(6, new EffectType[] { EffectType.Good, EffectType.Great });
    }

    public void Start()
    {
        base.Start();
        var colorChanger = DiceModel.AddComponent<CycleColors>();
        colorChanger.colors = new List<Color>()
        {
            new Color(1f, 0f, 0f),     
            new Color(1f, 0.5f, 0f),
            new Color(1f, 1f, 0f),
            new Color(0f, 1f, 0f),
            new Color(0f, 0f, 1f),
            new Color(0.29f, 0f, 0.51f),
            new Color(0.56f, 0f, 1f)
        };
        var chance = Random.Range(0, 10);
        if (chance > 8)
        {
            colorChanger.colors = new List<Color>()
            {
                new Color(Random.value, Random.value, Random.value),     
                new Color(Random.value, Random.value, Random.value),
                new Color(Random.value, Random.value, Random.value),
                new Color(Random.value, Random.value, Random.value),
                new Color(Random.value, Random.value, Random.value),
                new Color(Random.value, Random.value, Random.value),
                new Color(Random.value, Random.value, Random.value)
            };
        }
        colorChanger.lerpDuration = 0.6f;
    }

    public override void Roll()
    {
        int diceRoll = UnityEngine.Random.Range(1, 7);
        IEffect randomEffect = GetRandomEffect(diceRoll, Effects);
        if (diceRoll == 1 && !fromRainbow) randomEffect = new InstantReroll();
        if (randomEffect == null) return;
        Networker.Instance.photonView.RPC("LogToAllRPC", RpcTarget.Others,$"Rolling {randomEffect.Name}");
        Networker.Instance.LogToAllRPC($"Rolling {randomEffect.Name}");
        string messageToSay = $"Rolling {randomEffect.Name}";
        if(!RepoDice.SpoilerMode.Value) messageToSay = randomEffect.Tooltip;
        lastHolder.photonView.RPC("ChatMessageSendRPC", RpcTarget.All, messageToSay, false);
        
        randomEffect.Use(lastHolder);
        explodeMachoAndGlitch(1);
    }
}