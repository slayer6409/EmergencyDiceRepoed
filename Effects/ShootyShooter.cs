using System;
using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;
using Random = UnityEngine.Random;

namespace RepoDice.Effects;

public class ShootyShooter : IEffect
{
    public string Name => "Shooty Shooter";
    public EffectType Outcome => EffectType.Abhorrent;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Shooty Shooter";

    public void Use(PlayerAvatar roller)
    { 
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Misc.SpawnEnemy("Huntsman", 1, spawnPos, hunterStuff:true);
    }
}

public class FastShooterMono : MonoBehaviour
{
    public EnemyHunter hunter;
    public int shotsAtOnce = 8;
    public void Awake()
    {
        if (hunter == null) hunter = gameObject.GetComponent<EnemyHunter>();
    }

    public void Update()
    {
        if(!SemiFunc.IsMasterClientOrSingleplayer()) return;
        if (hunter == null) return;
        if (hunter.enemy.StateDespawn) return;

        hunter.shootFast = true;
        hunter.shotsFiredMax = 8;
        
        if (hunter.currentState == EnemyHunter.State.Shoot || 
            hunter.currentState == EnemyHunter.State.ShootEnd || 
            hunter.currentState == EnemyHunter.State.Aim)
        {
            hunter.stateTimer = 0f;
        }
    }
}