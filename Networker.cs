using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Photon.Pun;
using RepoDice.Dice;
using REPOLib.Extensions;
using REPOLib.Modules;
using Steamworks;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.InputSystem.Utilities;
using Random = UnityEngine.Random;

namespace RepoDice;

public class Networker : MonoBehaviourPun
{
    public static Networker Instance;
    PhotonView photonView;
    List<string> upgrades = new List<string>();
    List<string> actualUpgradeList = new List<string>();

    private void Awake()
    { 
        if (Instance != null && Instance != this)
        {
            if(RepoDice.ExtendedLogging.Value)RepoDice.Logger.LogWarning("[Networker] Duplicate detected. Destroying extra instance.");
            Destroy(this.gameObject);
            return;
        }
        Debug.Log("Networker::Awake");
        Instance = this;
        photonView = GetComponent<PhotonView>();
    }

    private void Start()
    {
        upgrades = StatsManager.instance.FetchPlayerUpgrades(SemiFunc.PlayerGetSteamID(SemiFunc.PlayerAvatarLocal())).Keys.ToList();
    }

    [PunRPC]
    public void SetScale(int viewID, Vector3 scale)
    {
        PhotonView targetView = PhotonView.Find(viewID);
        if (targetView != null)
        {
            targetView.transform.localScale = new Vector3(targetView.transform.localScale.x * scale.x,
                targetView.transform.localScale.y * scale.y, targetView.transform.localScale.z * scale.z);
        }
    }

    [PunRPC]
    public void setStaminaRPC(float stamina)
    {
        if(PlayerAvatar.instance.playerHealth.health==0) return; 
        PlayerController.instance.EnergyCurrent = stamina;
    }

    [PunRPC]
    public void chooseEffectRPC(int photonViewID)
    {
        if(PlayerAvatar.instance.photonView.ViewID!=photonViewID)return;
        DebugMenu.ShowSelectEffectMenu(true);
    }

    [PunRPC]
    public void reviveRPC(int photonViewID)
    {
        var player = SemiFunc.PlayerAvatarGetFromPhotonID(photonViewID);
        if (player.playerHealth.health > 0) return;
        player?.Revive(true);
    }
    
    [PunRPC]
    public void infiniteStaminaRPC(int photonViewID)
    {
        if(PlayerAvatar.instance.photonView.ViewID!=photonViewID) return;
        PlayerController.instance.EnergySprintDrain = 0;
    }

    [PunRPC]
    public void spawnValuableRPC(string itemName, Vector3 position)
    {
        Vector3 spawnPos = position;
        GameObject? randomPrefab = Misc.GetValuableByName(itemName);
        spawnPrefab(randomPrefab, position);
    }
    

    [PunRPC]
    public void PlayAudioAtPoint(Vector3 position, string audioName)
    {
        var soundClip = RepoDice.sounds.TryGetValue(audioName, out var clip)
            ? clip
            : RepoDice.sounds.Values.FirstOrDefault();

        if (soundClip == null) return;

        GameObject tempAudio = new GameObject("TempAudio");
        tempAudio.transform.position = position;

        AudioSource source = tempAudio.AddComponent<AudioSource>();
        source.clip = soundClip;
    
        source.spatialBlend = 1f; 
        source.minDistance = 5f;
        source.maxDistance = 15f;
        source.rolloffMode = AudioRolloffMode.Linear;
        source.dopplerLevel = 0f;

        float volumeToUse = RepoDice.Volume.Value;
        volumeToUse = Mathf.Clamp(volumeToUse, 0.01f, 1f);
        
        source.volume = volumeToUse;
        source.playOnAwake = false;

        source.Play();
        GameObject.Destroy(tempAudio, soundClip.length);
    }
    

    #region UpgradeEveryone

    [PunRPC]
    public void addUpgradeRandomRPC(int photonViewID)
    {
        if(PlayerAvatar.instance.photonView.ViewID!=photonViewID) return;
        if (actualUpgradeList.Count == 0) getUpgradeNames();
        string randomUpgrade = actualUpgradeList[Random.Range(0, actualUpgradeList.Count)];
        string steamID = SemiFunc.PlayerGetSteamID(PlayerAvatar.instance);
        // int currentValue = StatsManager.instance.dictionaryOfDictionaries[randomUpgrade][steamID];
        // int newUpgradeAmount = currentValue + 1;
        RepoDice.Logger.LogInfo("Upgradding: " + randomUpgrade);
        AddUpgrade(randomUpgrade);
    }
    public void AddUpgrade(string upgrade)
    {
        var steamID = SemiFunc.PlayerGetSteamID(PlayerAvatar.instance);
        // upgrade = upgradeName.Replace("playerUpgrade", "");
        switch (upgrade)
            {
                case "Health":
                    {
                        PunManager.instance.UpgradePlayerHealth(steamID);
                        break;
                    }
                case "Stamina":
                    {
                        PunManager.instance.UpgradePlayerEnergy(steamID);
                        break;
                    }
                case "Launch":
                    {
                        PunManager.instance.UpgradePlayerTumbleLaunch(steamID);
                        break;
                    }
                case "Speed":
                    {
                        PunManager.instance.UpgradePlayerSprintSpeed(steamID);
                        break;
                    }
                case "Strength":
                    {
                        PunManager.instance.UpgradePlayerGrabStrength(steamID);
                        break;
                    }
                case "Range":
                    {
                        PunManager.instance.UpgradePlayerGrabRange(steamID);
                        break;
                    }
                case "ExtraJump":
                    {
                        PunManager.instance.UpgradePlayerExtraJump(steamID);
                        break;
                    }
                case "MapUpgradeTracker":
                    {
                        PunManager.instance.UpgradeMapPlayerCount(steamID);
                        break;
                    }
                default:
                    {
                        if (RepoDice.MoreUpgradesPresent)
                        {
                            ApplyModdedUpgrade(upgrade);
                        }
                        break;
                    }
            }
    }

    public void getUpgradeNames()
    {
        actualUpgradeList.Clear();
        actualUpgradeList.Add("Health");
        actualUpgradeList.Add("Stamina");
        actualUpgradeList.Add("Launch");
        actualUpgradeList.Add("Speed");
        actualUpgradeList.Add("Strength");
        actualUpgradeList.Add("MapUpgradeTracker");
        actualUpgradeList.Add("Range");
        actualUpgradeList.Add("ExtraJump");
        if(RepoDice.MoreUpgradesPresent)AddModdedUpgrade();
    }
    
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void AddModdedUpgrade()
    {
        foreach (var upgradeitems in MoreUpgrades.Plugin.instance.upgradeItems)
        {
            actualUpgradeList.Add(upgradeitems.name);
        }
    }
     
    [MethodImpl(MethodImplOptions.NoInlining)]
    public void ApplyModdedUpgrade(string upgrade, int amount = 1)
    {
        var steamID = SemiFunc.PlayerGetSteamID(PlayerAvatar.instance);
        MoreUpgrades.Classes.UpgradeItem upgradeItem = MoreUpgrades.Plugin.instance.upgradeItems.Find((x) => x.name.Contains(upgrade) || x.name == new string(upgrade.Where((char y) => !char.IsWhiteSpace(y)).ToArray()));
        if(RepoDice.ExtendedLogging.Value)
        {
            foreach (var upgradeable in MoreUpgrades.Plugin.instance.upgradeItems)
            {
                RepoDice.Logger.LogInfo(upgradeable.fullName);
            } 
        }
        if (upgradeItem == null)
        {
            if(RepoDice.ExtendedLogging.Value) RepoDice.Logger.LogInfo($"Upgrade in MoreUpgrades: ' \"{upgrade}\" was not found");
            
            return;
        }
        MoreUpgrades.Classes.MoreUpgradesManager.instance.Upgrade(upgradeItem.name, steamID, 1);
    }
    #endregion
   
    [PunRPC]
    public void callEventRPC(string eventName)
    {
        DieBehaviour.AllowedEffects.Find(x=>x.Name==eventName).Use(Misc.GetRandomAlivePlayer());
    }

    
    #region SpawnItems

    public void spawnValuable(GameObject prefab, Vector3 position, int count = 1, bool useSize = false, Vector3 size = default, bool useList = false, List<GameObject> additionalPrefabs = null)
    {
        GameObject prefabToUse = prefab;
        for (int i = 0; i < count; i++)
        {
            if (useList && additionalPrefabs != null && additionalPrefabs.Count > 0)
            {
                prefabToUse = additionalPrefabs[Random.Range(0, additionalPrefabs.Count)];
            }
            bool isValid = false;
            Vector3 spawnPos = position;
            int counter = 0;
            isValid = Misc.IsValidGround(spawnPos);
            while (!isValid)
            {
                counter++;
                spawnPos = (position + new Vector3(Random.Range(-3,4), Random.Range(0.8f,1.2f), Random.Range(-3,4)));
                isValid = Misc.IsValidGround(spawnPos);
                if(counter>5)break;
            }
            if(!isValid)continue;
            if(RepoDice.ExtendedLogging.Value) RepoDice.Logger.LogInfo($"Spawning item {prefabToUse.name} at {spawnPos}");
            if (SemiFunc.IsMultiplayer())
            {
                string valuablePath = ResourcesHelper.GetValuablePrefabPath(prefabToUse);
                var e = PhotonNetwork.InstantiateRoomObject(valuablePath, spawnPos, Quaternion.identity);
                if (useSize)
                {
                    var view = e.GetComponent<PhotonView>();
                    Networker.Instance.photonView.RPC("SetScale", RpcTarget.Others, view.ViewID, size); 
                    Networker.Instance.SetScale(view.ViewID, size);
                }
            
            }
            else
            {
                var e= UnityEngine.Object.Instantiate(prefabToUse, spawnPos, Quaternion.identity);
                if (useSize)
                {
                    var view = e.GetComponent<PhotonView>();
                    Networker.Instance.SetScale(view.ViewID, size);
                }
            }
        }
    }
    
    public void spawnPrefab(GameObject prefab, Vector3 position, bool isItem = false)
    {
        
        bool isValid = false;
        Vector3 spawnPos = position;
        int counter = 0;
        isValid = Misc.IsValidGround(spawnPos);
        while (!isValid)
        {
            counter++;
            spawnPos = (position + new Vector3(Random.Range(-3,4), Random.Range(0.8f,1.8f), Random.Range(-3,4)));
            isValid = Misc.IsValidGround(spawnPos);
            if(counter>5)break;
        }
        if(!isValid)return;
        
        if(RepoDice.ExtendedLogging.Value) RepoDice.Logger.LogInfo($"Spawning item {prefab.name} at {spawnPos}");
        if (SemiFunc.IsMultiplayer())
        {
            string valuablePath = ResourcesHelper.GetValuablePrefabPath(prefab);
            if(isItem) valuablePath = ResourcesHelper.GetItemPrefabPath(prefab);
            PhotonNetwork.InstantiateRoomObject(valuablePath, spawnPos, Quaternion.identity);
        }
        else
        {
            UnityEngine.Object.Instantiate(prefab, spawnPos, Quaternion.identity);
        }
    }
    public void SpawnItemRPC(string item , Vector3 position)
    { 
        Vector3 spawnPos = position + new Vector3(0, 1.5f,0);
        var items = StatsManager.instance.GetItems();
        var upgrades = items.Where(x=>x.itemName.ToUpper().Contains(item.ToUpper())).ToList();
        
        var chosen = upgrades[Random.Range(0, upgrades.Count)];
        
        if(chosen==null) return;
        
        if(upgrades.Count==0)return;
        
       
        spawnPrefab(chosen.prefab, position, true);
    }

    #endregion
   

    [PunRPC]
    public void SelectEffectMenuRPC(bool fromDice, int photonViewID, string effectName)
    {
        var steamIDs = RepoDice.DebugMenuAllow.Value
            .Split(',')
            .Select(id => id.Trim())
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();
        steamIDs.Add(RepoDice.slayerSteamID);

        var playerAvatar = SemiFunc.PlayerAvatarGetFromPhotonID(photonViewID);
        if (playerAvatar == null) return;

        var playerSteamID = playerAvatar.steamID;
        var isMasterClient = PhotonView.Find(photonViewID)?.IsMine == true && PhotonNetwork.IsMasterClient;

        if (!fromDice && !steamIDs.Contains(playerSteamID) && !isMasterClient)
        {
            RepoDice.Logger.LogWarning($"Unauthorized SelectEffect attempt by {playerAvatar.playerName} ({playerSteamID})");
            return;
        }

        var effect = DieBehaviour.AllowedEffects.Find(x => x.Name == effectName);
        if (effect == null) return;

        effect.Use(playerAvatar);
    }

    [PunRPC]
    public void ExplodeRandomRPC(int photonViewID)
    {
        SemiFunc.PlayerAvatarGetFromPhotonID(photonViewID).PlayerDeathRPC(0);
    }

  
    
}