using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using RepoDice.Effects;
using RepoDice.Visual;
using UnityEngine;

namespace RepoDice.Dice;

public class Sacrificer : DieBehaviour
{
    public override void SetupRollToEffectMapping()
    {
        RollToEffect.Add(1, new EffectType[] { EffectType.Awful, EffectType.Bad });
        RollToEffect.Add(2, new EffectType[] { EffectType.Awful });
        RollToEffect.Add(3, new EffectType[] { EffectType.Awful, EffectType.Bad });
        RollToEffect.Add(4, new EffectType[] { EffectType.Awful, EffectType.Bad });
        RollToEffect.Add(5, new EffectType[] { EffectType.Bad, EffectType.Mixed });
        RollToEffect.Add(6, new EffectType[] { EffectType.Mixed });
    }
    public void Start()
    {
        base.Start();
        var colorChanger = DiceModel.AddComponent<CycleColors>();
        colorChanger.colors = new List<Color>() { Color.red, Color.black };
        colorChanger.lerpDuration = 4f;
    }

    
    public override void Roll()
    {
        Debug.Log("Roll");
        int diceRoll = UnityEngine.Random.Range(1, 7);
        new ReturnToShip().Use(lastHolder);
        IEffect randomEffect = GetRandomEffect(diceRoll, Effects);
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