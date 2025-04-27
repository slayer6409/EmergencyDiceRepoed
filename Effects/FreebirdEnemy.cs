using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Modules;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

namespace RepoDice.Effects;

public class FreebirdEnemy : IEffect
{
    public string Name => "Freebird Enemy";
    public EffectType Outcome => EffectType.Awful;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "FREE BIRDDDDDDDDDDDDDD";

    public void Use(PlayerAvatar roller)
    {
        string[] excluded = { "gnome", "destroyer", "banger", "spewer" ,"test", "1","2","3","4","5","6","7","8","9", "droid", "lost", "gnomes"};
        var allEnemies = EnemyDirector.instance.enemiesDifficulty1
            .Concat(EnemyDirector.instance.enemiesDifficulty2)
            .Concat(EnemyDirector.instance.enemiesDifficulty3)
            .Where(e => !excluded.Any(ex => e.name.ToLowerInvariant().Contains(ex)))
            .ToList();
        
        var enemyToSpawn = allEnemies[Random.Range(0, allEnemies.Count)].name;
        Misc.SpawnEnemy(enemyToSpawn, 1, roller.transform.position+roller.transform.forward*2,true);
    }
}
public class freebirdMaker : MonoBehaviour
{
    public EnemyNavMeshAgent agent;
    private AudioSource audiosrc;
    public Enemy enemy;
    public int speed = 120, acceleration = 999;
    public void Awake()
    {
        if (agent == null) agent = gameObject.GetComponent<EnemyNavMeshAgent>();
        if (agent == null) return;
        if (enemy == null) enemy = gameObject.GetComponent<Enemy>();
        if (enemy == null) return;
        
        audiosrc = gameObject.AddComponent<AudioSource>();
        audiosrc.loop = true;
        audiosrc.playOnAwake = true;
        audiosrc.volume = RepoDice.muteFreebird.Value ? 0f : Mathf.Clamp01(RepoDice.Volume.Value) * 0.3f;
        audiosrc.spatialBlend = 1f;
        audiosrc.rolloffMode = AudioRolloffMode.Linear;  
        audiosrc.minDistance = 2f;  
        audiosrc.maxDistance = 15f;
        audiosrc.dopplerLevel = 0f;
        audiosrc.transform.localPosition = Vector3.up * 1f;
        audiosrc.priority = 10;
        //AddRainbowTrail();
        if (RepoDice.Copyright.Value)
        {
            audiosrc.clip = RepoDice.LoadedAssets.LoadAsset<AudioClip>("SpazzmaticaPolka");
        }
        else
        {
            audiosrc.clip = RepoDice.LoadedAssets.LoadAsset<AudioClip>("Freebird");
        }
        audiosrc.Play();
        agent.OverrideAgent(speed, acceleration,99999f);
    }
    private void OnDestroy()
    {
        if (audiosrc != null && audiosrc.isPlaying)
            audiosrc.Stop();
    }
    private void AddRainbowTrail()
    {
        if (gameObject.GetComponent<TrailRenderer>() != null)
            return;
        var trail = gameObject.AddComponent<TrailRenderer>();
        trail.time = 1.5f;
        trail.startWidth = 0.4f;
        trail.endWidth = 0.05f;
        trail.minVertexDistance = 0.1f;
        trail.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        trail.receiveShadows = false;
        trail.alignment = LineAlignment.View;
        trail.material = new Material(Shader.Find("Sprites/Default"));

        var gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.red, 0f),
                new GradientColorKey(Color.yellow, 0.2f),
                new GradientColorKey(Color.green, 0.4f),
                new GradientColorKey(Color.cyan, 0.6f),
                new GradientColorKey(Color.blue, 0.8f),
                new GradientColorKey(Color.magenta, 1f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(1f, 0f),
                new GradientAlphaKey(0f, 1f)
            }
        );
        trail.colorGradient = gradient;
    }
    public void Update()
    {
        if(enemy==null) return;
        if (!enemy.EnemyParent.Spawned)
        {
            audiosrc.Stop();
        }
        if(!Mathf.Approximately(agent.Agent.speed, speed)) agent.OverrideAgent(speed, acceleration, 99999f);
    }
}