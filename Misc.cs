using System;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using REPOLib.Extensions;
using REPOLib.Modules;
using UnityEngine;
using Logger = Photon.Voice.Unity.Logger;

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
                                
                                if(RepoDice.ExtendedLogging.Value) RepoDice.Logger.LogInfo($"Found Valuable Preset: {item.name}");
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
            .Where(x => x.name.Contains(name))
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
    public static void SpawnEnemy(string enemyName, int count, Vector3 spawnPos)
    {
        try
        {
            if(EnemyDirector.instance.TryGetEnemyThatContainsName(enemyName, out EnemySetup enemy))
            {
                for (int i = 0; i < count; i++)
                { 
                    Enemies.SpawnEnemy(enemy, spawnPos+Vector3.forward+Vector3.forward, Quaternion.identity, spawnDespawned: false);
                }
            }
        }
        catch (Exception e)
        {
        }
    }
    public static void SpawnAndScaleEnemy(string enemyName, int count, Vector3 spawnPos, Vector3 scale)
    {
        if (RepoDice.ExtendedLogging.Value)
        {
            foreach (var enmy in EnemyDirector.instance.GetEnemies())
            {
                RepoDice.Logger.LogInfo($"Enemy Found: {enmy.name}");
            }
        }
        try
        {
            if(EnemyDirector.instance.TryGetEnemyThatContainsName(enemyName, out EnemySetup enemy))
            {
                for (int i = 0; i < count; i++)
                { 
                    var spawnedEnemies = Enemies.SpawnEnemy(enemy, spawnPos+Vector3.forward+Vector3.forward, Quaternion.identity, spawnDespawned: false);
                    foreach (EnemyParent ep in spawnedEnemies)
                    {
                        var view = ep.GetComponent<PhotonView>();
                        if (view != null)
                        {
                            if (SemiFunc.IsMasterClientOrSingleplayer())
                            {
                                Networker.Instance.photonView.RPC("SetScale", RpcTarget.Others, view.ViewID, scale); 
                                Networker.Instance.SetScale(view.ViewID, scale);
                            }
                       
                        }
                    }
                }
            }
        }
        catch (Exception e)
        {
            
        }
        
    }
}