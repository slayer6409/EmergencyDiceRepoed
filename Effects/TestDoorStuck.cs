using System;
using System.Collections.Generic;
using System.Linq;
using BepInEx.Configuration;
using Photon.Pun;
using RepoDice.Dice;
using RepoDice.Patches;
using REPOLib.Modules;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RepoDice.Effects;

public class TestDoorStuck : IEffect
{
    public string Name => "TestDoorStuck";
    public EffectType Outcome => EffectType.Great;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => ">:D";
    
    public void Use(PlayerAvatar roller)
    {
        Networker.Instance.photonView.RPC(nameof(Networker.Instance.doSillyShit), RpcTarget.All);
    }
}