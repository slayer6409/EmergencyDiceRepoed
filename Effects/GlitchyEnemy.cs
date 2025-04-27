using System.Linq;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;

namespace RepoDice.Effects;

public class GlitchyEnemy : IEffect
{
    public string Name => "Glitchy Enemy";
    public EffectType Outcome => EffectType.Awful;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "He got a bit broken";

    public void Use(PlayerAvatar roller)
    {   
        string[] excluded = { "destroyer", "spewer", "scorcher" , "1","2","3","4","5","6","7","8","9"};
        var allEnemies = EnemyDirector.instance.enemiesDifficulty1
            .Concat(EnemyDirector.instance.enemiesDifficulty2)
            .Concat(EnemyDirector.instance.enemiesDifficulty3)
            .Where(e => !excluded.Any(ex => e.name.ToLowerInvariant().Contains(ex)))
            .ToList();
        var enemyToSpawn = allEnemies[Random.Range(0, allEnemies.Count)].name;
        Vector3 spawnPos = roller.transform.position + roller.transform.forward;
        Misc.SpawnEnemy(enemyToSpawn, 1, spawnPos, false, true);
    }
}