using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using BepInEx.Logging;
using Photon.Pun;
using RepoDice.Effects;
using REPOLib.Extensions;
using REPOLib.Modules;
using Unity.VisualScripting;
using UnityEngine;
using Logger = Photon.Voice.Unity.Logger;
using Random = UnityEngine.Random;

namespace RepoDice;

public class Misc : MonoBehaviour
{
    public enum Size
    {
        tiny,
        small,
        medium,
        big,
        wide,
        tall,
        veryTall
    }

    public enum UpgradeType
    {
        health,
        energy,
        strength,
        jumps,
        speed,
        range
    }
    public static T GetRandomEnum<T>() where T : Enum
    {
        Array values = Enum.GetValues(typeof(T));
        return (T)values.GetValue(UnityEngine.Random.Range(0, values.Length));
    }
    
    public static readonly Dictionary<Size, List<GameObject>> valuablePrefabsBySize = new();
    public static readonly Dictionary<string, GameObject> valuablePrefabsByName = new();
    public static void CacheValuables()
    {
        valuablePrefabsBySize.Clear();
        valuablePrefabsByName.Clear();

        if (RunManager.instance == null)
        {
            RepoDice.Logger.LogError("Failed to cache LevelValuables. RunManager instance is null.");
            return;
        }

        foreach (var level in RunManager.instance.levels)
        {
            foreach (var valuablePreset in level.ValuablePresets)
            {
                try
                {
                    var sizeMapping = new Dictionary<Size, List<GameObject>>()
                    {
                        { Size.tiny, valuablePreset.tiny },
                        { Size.small, valuablePreset.small },
                        { Size.medium, valuablePreset.medium },
                        { Size.big, valuablePreset.big },
                        { Size.wide, valuablePreset.wide },
                        { Size.tall, valuablePreset.tall },
                        { Size.veryTall, valuablePreset.veryTall }
                    };

                    foreach (var pair in sizeMapping)
                    {
                        if (!valuablePrefabsBySize.TryGetValue(pair.Key, out var list))
                        {
                            list = new List<GameObject>();
                            valuablePrefabsBySize[pair.Key] = list;
                        }

                        foreach (var item in pair.Value)
                        {
                            if (item != null)
                            {
                                RepoDice.SuperLog($"Found Valuable Preset: {item.name}");
                                if (!list.Contains(item))
                                    list.Add(item);

                                var lowerName = item.name.ToLower();
                                if (!valuablePrefabsByName.ContainsKey(lowerName))
                                    valuablePrefabsByName[lowerName] = item;
                            }
                        }
                    }
                }
                catch (Exception e)
                {
                    RepoDice.Logger.LogWarning($"Exception while caching valuables: {e.Message}");
                }
            }
        }
    }
   
    public static GameObject? GetRandomValuable(Size? size = null)
    {
        if (valuablePrefabsBySize.Count == 0)
            CacheValuables();

        if (size.HasValue)
        {
            if (valuablePrefabsBySize.TryGetValue(size.Value, out var list) && list.Count > 0)
            {
                return list[UnityEngine.Random.Range(0, list.Count)];
            }
        }
        else
        {
            var nonEmptyLists = valuablePrefabsBySize.Values.Where(l => l.Count > 0).ToList();
            if (nonEmptyLists.Count == 0) return null;

            var randomList = nonEmptyLists[UnityEngine.Random.Range(0, nonEmptyLists.Count)];
            return randomList[UnityEngine.Random.Range(0, randomList.Count)];
        }

        return null;
    }

    public static List<GameObject>? getValuablesWithName(string name)
    {
        if (valuablePrefabsByName.Count == 0)
            CacheValuables();
        
        return valuablePrefabsByName.Values
            .Where(x => x.name.ToLower().Contains(name.ToLower()))
            .ToList();
    } 

    public static GameObject? GetValuableByName(string name)
    {
        if (valuablePrefabsByName.Count == 0)
            CacheValuables();

        return valuablePrefabsByName.TryGetValue(name.ToLower(), out var prefab) ? prefab : null;
    }
    public static List<GameObject> GetValuablesBySize(Size size)
    {
        if (valuablePrefabsBySize.Count == 0)
            CacheValuables();
        
        return valuablePrefabsBySize.TryGetValue(size, out var list) ? list : new List<GameObject>();
    }
    public static PlayerAvatar GetPlayerBySteamID(string id)
    {
        var players = GameDirector.instance.PlayerList.Find(x=>x.steamID==id);

        if (players == null)
            return null;
        return players;
    }
    public static bool IsValidGround(Vector3 position, float maxDropDistance = 5f, LayerMask? layerMask = null)
    {
        LayerMask maskToUse = layerMask ?? Physics.DefaultRaycastLayers;
        bool grounded = Physics.Raycast(position, Vector3.down, maxDropDistance, maskToUse);
        if(!grounded) return false;

        var players = GameDirector.instance.PlayerList;
        foreach (var player in players)
        {
            if (Vector3.Distance(player.transform.position, position) < 3)
            {
                return false;
            }
        }
        return true;
    }
    public static PlayerAvatar GetLocalPlayer()
    {
        foreach (PlayerAvatar player in GameDirector.instance.PlayerList)
        {
            if (player.photonView != null && player.photonView.IsMine)
            {
                return player;
            }
        }

        RepoDice.Logger.LogWarning("Local PlayerAvatar not found!");
        return null;
    }

    public static bool isGlitchOrConfig()
    {
        if(SemiFunc.PlayerGetSteamID(PlayerAvatar.instance)==RepoDice.glitchSteamID || RepoDice.IWannaSeeWhatGlitchSees.Value) return true;
        return false;
    }
    public static PlayerAvatar GetRandomPlayer()
    {
        var players = GameDirector.instance.PlayerList;

        if (players == null || players.Count == 0)
            return null;

        int index = UnityEngine.Random.Range(0, players.Count);
        return players[index];
    }
    public static PlayerAvatar? GetRandomAlivePlayer()
    {
        var players = GameDirector.instance.PlayerList.Where(x=>x.playerHealth.health>0f).ToList();

        if (players.Count == 0) return null;

        int index = UnityEngine.Random.Range(0, players.Count);
        return players[index];
    }
    public static void SpawnEnemy(string enemyName, int count, Vector3 position, bool isFreebird = false, bool isGlitchy = false, bool fromTry = false, bool hunterStuff = false)
    {
        if(RepoDice.ExtendedLogging.Value)
            foreach (var enmy in getEnemies())
            {
                RepoDice.SuperLog($"Enemy Found: {enmy.name}");
            }
        try
        {
            var enemy = getEnemies().FirstOrDefault(x => x.name == enemyName);
            if (enemy == null) enemy = getEnemies().FirstOrDefault(x => NameContains(x, enemyName));
            if(enemy==null) RepoDice.SuperLog(enemyName + " not found!");
            if(enemy!=null)
            {
                Vector3 spawnPos = position+Vector3.forward+Vector3.forward;
                int counter = 0;
                bool isValid = Misc.IsValidGround(spawnPos);
                while (!isValid)
                {
                    counter++;
                    spawnPos = (position + new Vector3(Random.Range(-3,4), Random.Range(0.8f,1.8f), Random.Range(-3,4)));
                    isValid = Misc.IsValidGround(spawnPos);
                    if(counter>5)break;
                }
                LevelPoint? levelPoint = SemiFunc.LevelPointGet(position, 4, 20);
                if (!isValid)
                {
                    if (levelPoint == null) return;
                    spawnPos = levelPoint.transform.position;
                }
                
                for (int i = 0; i < count; i++)
                { 
                    

                    var enemies = Enemies.SpawnEnemy(enemy, spawnPos, Quaternion.identity, spawnDespawned: false);
                    RepoDice.SuperLog($"Spawned {enemies.Count} enemies for '{enemyName}'");
                    if (isFreebird)
                    {
                        foreach (var en in enemies)
                        {
                            if (SemiFunc.IsMultiplayer())
                            {
                                if(SemiFunc.IsMasterClient()) Networker.Instance.makeFreebirdRpc(en.photonView.ViewID);
                                else Networker.Instance.photonView.RPC("makeFreebirdRpc", RpcTarget.Others, en.photonView.ViewID);
                            }
                            else
                            {
                                Networker.Instance.makeFreebirdRpc(en.photonView.ViewID);
                            }
                        }
                    }

                    if (isGlitchy)
                    {
                        foreach (var en in enemies)
                        {
                            Networker.Instance.photonView.RPC(nameof(Networker.Instance.makeGlassRPC), RpcTarget.All, en.photonView.ViewID, true, true);
                        }
                    }

                    if (hunterStuff)
                    {
                        int shotsToFire = UnityEngine.Random.Range(8, 15);
                        foreach (var en in enemies)
                        {
                            var hntr = en.GetComponentInChildren<EnemyHunter>();
                            if (hntr != null)
                            {
                                var hntmno = hntr.transform.AddComponent<FastShooterMono>();
                                hntmno.hunter = hntr;
                                hntmno.shotsAtOnce = shotsToFire;
                            } 
                            else RepoDice.SuperLog($"Hunter {en.name} not found!");
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            if (fromTry) return;
            RepoDice.SuperLog("Error with spawning enemy, spawning Animal instead: " + e, LogLevel.Error);
            SpawnEnemy("Animal", 1, position, isFreebird, isGlitchy, true);
        }
    }

    public static bool NameContains(EnemySetup enemySetup, string name)
    {
        if (enemySetup == null)
        {
            return false;
        }

        if (LooseStringMatch(enemySetup.name, name))
        {
            return true;
        }

        EnemyParent? enemyPrnt = null;
        foreach (var spawnObject in enemySetup.spawnObjects.Where(x => x != null).Distinct())
        {
            if (spawnObject.TryGetComponent(out enemyPrnt)) ;
        }
        if (enemyPrnt!=null)
        {
            if (LooseStringMatch(enemyPrnt.enemyName, name))
            {
                return true;
            }

            if (LooseStringMatch(enemyPrnt.gameObject.name, name))
            {
                return true;
            }
        }

        return false;
    }


    public static void MakeGlass(Material mat, float alpha = 0.3f, bool fucky = false)
    {
        Color originalColor = mat.color;
        if (mat == RepoDice.GlitchyMaterial) return;
        if (fucky)
        {
            Shader standardShader = Shader.Find("Autodesk Interactive");
            if (standardShader == null)
            {
                RepoDice.SuperLog("Autodesk Interactive shader not found!");
                return;
            }
            mat.shader = standardShader;
            
        }
        else
        {
            Shader standardShader = Shader.Find("Standard");
            if (standardShader == null)
            {
                RepoDice.SuperLog("Standard shader not found!");
                return;
            }
            mat.shader = standardShader;
        }
        
        mat.SetFloat("_Mode", 3);
        mat.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        mat.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        mat.SetInt("_ZWrite", 0);
        mat.DisableKeyword("_ALPHATEST_ON");
        mat.EnableKeyword("_ALPHABLEND_ON");
        mat.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        mat.renderQueue = (int)UnityEngine.Rendering.RenderQueue.Transparent;

        // Adjust color alpha
        Color color = originalColor;
        color.a = alpha;
        mat.color = color;
    }

    private static List<EnemySetup> cachedEnemies = new();
    public static ReadOnlyCollection<EnemySetup> getEnemies()
    {
        if (cachedEnemies.Count == 0)
        {
            var list = new List<EnemySetup>();
            list.AddRange(EnemyDirector.instance.enemiesDifficulty1);
            list.AddRange(EnemyDirector.instance.enemiesDifficulty2);
            list.AddRange(EnemyDirector.instance.enemiesDifficulty3);
            cachedEnemies = list;
        }
        return cachedEnemies.AsReadOnly();
    }
    public static bool LooseStringMatch(string a, string b)
    {
        string normA = RemoveWhitespace(a).ToLowerInvariant();
        string normB = RemoveWhitespace(b).ToLowerInvariant();
        return normA == normB;
    }

    private static string RemoveWhitespace(string input)
    {
        return new string(input.Where(c => !char.IsWhiteSpace(c)).ToArray());
    }
    public static void SpawnAndScaleEnemy(string enemyName, int count, Vector3 position, Vector3 scale, bool isGlitchy = false)
    {
        if(RepoDice.ExtendedLogging.Value)
            foreach (var enmy in getEnemies())
            {
                RepoDice.SuperLog($"Enemy Found: {enmy.name}");
            }
        try
        {
            var enemy = getEnemies().FirstOrDefault(x => x.name == enemyName);
            if (enemy == null) enemy = getEnemies().FirstOrDefault(x => x.name.Contains(enemyName));
            if(enemy==null) RepoDice.SuperLog(enemyName + " not found!");
            if(enemy!=null)
            {
                RepoDice.SuperLog("Spawning enemy: "+ enemy.name);
                Vector3 spawnPos = position+Vector3.forward+Vector3.forward;
                int counter = 0;
                bool isValid = Misc.IsValidGround(spawnPos);
                while (!isValid)
                {
                    counter++;
                    spawnPos = (position + new Vector3(Random.Range(-3,4), Random.Range(0.8f,1.8f), Random.Range(-3,4)));
                    isValid = Misc.IsValidGround(spawnPos);
                    if(counter>5)break;
                }
                LevelPoint? levelPoint = SemiFunc.LevelPointGet(position, 4, 20);
                if (!isValid)
                {
                    if (levelPoint == null) return;
                    spawnPos = levelPoint.transform.position;
                }
                
                for (int i = 0; i < count; i++)
                { 
                    var spawnedEnemies = Enemies.SpawnEnemy(enemy, spawnPos, Quaternion.identity, spawnDespawned: false);
                    foreach (EnemyParent ep in spawnedEnemies)
                    {
                        var view = ep.GetComponent<PhotonView>();
                        if (view != null)
                        {
                            if (SemiFunc.IsMasterClientOrSingleplayer())
                            {
                                if(SemiFunc.IsMultiplayer()) Networker.Instance.photonView.RPC("SetScale", RpcTarget.Others, view.ViewID, scale); 
                                Networker.Instance.SetScale(view.ViewID, scale);
                            }
                       
                        }
                    }
                    if (isGlitchy)
                    {
                        foreach (var en in spawnedEnemies)
                        {
                            Networker.Instance.photonView.RPC("makeGlassRPC", RpcTarget.All, en.photonView.ViewID, true, true);
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            RepoDice.SuperLog(e.Message, LogLevel.Error);
        }
        
    }
}