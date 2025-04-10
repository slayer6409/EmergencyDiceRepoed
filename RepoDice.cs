using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Photon.Pun;
using RepoDice.Dice;
using RepoDice.Effects;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RepoDice;

//Yes I know my coding is a bit weird 

[BepInPlugin("Slayer6409.EmergencyDiceREPO", "Emergency Dice REPO", "1.0.2")]
[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID)]
[BepInDependency("bulletbot.moreupgrades", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("WesleysEnemies", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("Vortex.BigNuke", BepInDependency.DependencyFlags.SoftDependency)]
public class RepoDice : BaseUnityPlugin
{
    internal static RepoDice Instance { get; private set; } = null!;
    public static Dictionary<string, AudioClip> sounds = new Dictionary<string, AudioClip>();
    public static ConfigFile BepInExConfig = null;
    public static GameObject networker;
    public static ConfigEntry<bool> ExtendedLogging;
    public static ConfigEntry<bool> DebugMenuEnabled;
    public static ConfigEntry<string> DebugMenuColor;
    public static ConfigEntry<bool> DebugMenuColorUsesPlayerColor;
    public static ConfigEntry<string> DebugMenuAllow;
    public static ConfigEntry<bool> DebugMenuClosesAfter;
    public static ConfigEntry<bool> Bald;
    public static ConfigEntry<float> Volume;
    private InputAction debugMenuAction;
    private InputAction debugMenuAction2;
    public static GameObject DebugMenuPrefab, SelectMenuPrefab, DebugSubButtonPrefab, DebugMenuButtonPrefab;
    public static bool MoreUpgradesPresent=false;
    public static bool WesleysEnemiesPresent=false;
    public static bool BigNukePresent=false;
    internal new static ManualLogSource Logger => Instance._logger;
    private ManualLogSource _logger => base.Logger;
    
    public static AssetBundle LoadedAssets;

    public static readonly string slayerSteamID = "76561198077184650", machoSteamID = "76561198216220844", glitchSteamID = "76561198984467725";
    internal Harmony? Harmony { get; set; }
    public static Sprite WarningExclamation, WarningDeath, WarningLuck;
    public static GameObject Gambler;

    public static List<string> RegisteredDiceNames = new List<string>();
    
    private void Awake()
    {
        Instance = this;

        this.gameObject.transform.parent = null;
        this.gameObject.hideFlags = HideFlags.HideAndDontSave;

        LoadedAssets =
            AssetBundle.LoadFromFile(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location),
                "repodice"));
        BepInExConfig = Config;//new ConfigFile(Path.Combine(Paths.ConfigPath, "Emergency_Dice_REPO.cfg"), true);

        sounds.Add("FireAlarm", LoadedAssets.LoadAsset<AudioClip>("FireAlarm"));
        sounds.Add("Bell2", LoadedAssets.LoadAsset<AudioClip>("Bell2"));
        sounds.Add("Bad1", LoadedAssets.LoadAsset<AudioClip>("Bad1"));
        sounds.Add("Good2", LoadedAssets.LoadAsset<AudioClip>("Good2"));
        sounds.Add("AudioTest", LoadedAssets.LoadAsset<AudioClip>("AudioTest"));
        sounds.Add("BANANA", LoadedAssets.LoadAsset<AudioClip>("BANANA"));
        sounds.Add("disconnect", LoadedAssets.LoadAsset<AudioClip>("disconnect"));
        sounds.Add("DoorLeft", LoadedAssets.LoadAsset<AudioClip>("DoorLeft"));
        sounds.Add("DoorRight", LoadedAssets.LoadAsset<AudioClip>("DoorRight"));
        sounds.Add("mah-boi", LoadedAssets.LoadAsset<AudioClip>("mah-boi"));
        sounds.Add("tuturu", LoadedAssets.LoadAsset<AudioClip>("tuturu"));
        sounds.Add("WindowsError", LoadedAssets.LoadAsset<AudioClip>("WindowsError"));

        if (Chainloader.PluginInfos.ContainsKey("bulletbot.moreupgrades")) {MoreUpgradesPresent = true; Logger.LogInfo($"More upgrades compatibility enabled!");}
        if (Chainloader.PluginInfos.ContainsKey("WesleysEnemies")) {WesleysEnemiesPresent = true; Logger.LogInfo($"Wesley's Enemies compatibility enabled!");}
        if (Chainloader.PluginInfos.ContainsKey("Vortex.BigNuke")) {BigNukePresent = true; Logger.LogInfo($"BigNuke compatibility enabled!");}
        
        
        DebugMenuPrefab = LoadedAssets.LoadAsset<GameObject>("DebugMenu");
        SelectMenuPrefab = LoadedAssets.LoadAsset<GameObject>("NewSelectMenu");
        DebugMenuButtonPrefab = LoadedAssets.LoadAsset<GameObject>("DebugButton");
        DebugSubButtonPrefab = LoadedAssets.LoadAsset<GameObject>("SubmenuButton");
        WarningExclamation = LoadedAssets.LoadAsset<Sprite>("Warning");
        WarningDeath = LoadedAssets.LoadAsset<Sprite>("death");
        WarningLuck = LoadedAssets.LoadAsset<Sprite>("luck");
        Gambler = LoadedAssets.LoadAsset<GameObject>("Gambler");
        RegisteredDiceNames.Add("Gambler");
        Gambler.AddComponent<Gambler>();
        networker = LoadedAssets.LoadAsset<GameObject>("Dice Manager");
        networker.AddComponent<Networker>();
        REPOLib.Modules.Valuables.RegisterValuable(Gambler);
        REPOLib.Modules.NetworkPrefabs.RegisterNetworkPrefab(networker);
        ConfigGen();
        DieBehaviour.Config();
        Patch();
        if(ExtendedLogging.Value)db();
        debugMenuAction = new InputAction(name: null, InputActionType.Button, "<Keyboard>/numpadMinus");
        debugMenuAction.performed += ctx => DebugMenuStuff();
        debugMenuAction.Enable();
        // debugMenuAction2 = new InputAction(name: null, InputActionType.Button, "<Keyboard>/numpadPlus");
        // debugMenuAction2.performed += ctx => doSpecificRoll();
        // debugMenuAction2.Enable();
        Logger.LogInfo($"{Info.Metadata.GUID} v{Info.Metadata.Version} has loaded!");
    }

    private void db()
    {
        StartCoroutine(delayLogging());
    }

    private IEnumerator delayLogging()
    {
        yield return new WaitForSeconds(5f);
        foreach (var e in Chainloader.PluginInfos)
        {
            Logger.LogInfo($"{e}");
        }
    }

    public void ConfigGen()
    {
        DebugMenuColor = Config.Bind<string>(
            "Select Menu",
            "Main Color",
            "#270051",
            "Sets the color theme of the Select/Debug Menu.");  
        DebugMenuColorUsesPlayerColor = Config.Bind<bool>(
            "Select Menu",
            "Use Player Color",
            false,
            "Makes the Select/Debug Menu use player color, overwrites Main Color.");
        Bald = Config.Bind<bool>(
            "Select Menu",
            "Bald",
            false,
            "Bald Man");
        ExtendedLogging = Config.Bind<bool>(
            "Debug",
            "Extended Logging",
            false,
            "Logs a ton more things");
        DebugMenuEnabled = Config.Bind<bool>(
            "Debug",
            "Debug Menu",
            false,
            "Enables the Debug Menu (Host and Steam ID listed Players)");
        DebugMenuClosesAfter = Config.Bind<bool>(
            "Debug",
            "Debug Menu Closes After",
            true,
            "Makes it to where the debug menu closes after selecting an effect, False to stay open");  
        Volume = Config.Bind<float>(
            "Client Side",
            "Volume",
            0.60f,
            "This sets the volume of any sound effect from this mod, Range 0.01 to 1.");
        DebugMenuAllow = Config.Bind<string>(
            "Debug",
            "SteamIDS",
            "",
            "Put SteamIDs here for people you want to be able to use the debug menu. Host Only");
        Alarm.ConfigStuff();
    }
    
    internal void DebugMenuStuff()
    {
        Vector3 spawnPos = Vector3.zero;
        if(!DebugMenuEnabled.Value && Misc.GetLocalPlayer().steamID != slayerSteamID)return;
        if (SemiFunc.IsMultiplayer())
        {
            if(Misc.GetLocalPlayer().steamID != slayerSteamID && Misc.GetLocalPlayer().steamID != glitchSteamID && !SemiFunc.IsMasterClientOrSingleplayer()) return;
        }
        DebugMenu.ShowSelectEffectMenu();
    }
    internal void Patch()
    {
        Harmony ??= new Harmony(Info.Metadata.GUID);
        Harmony.PatchAll();
    }

    internal void Unpatch()
    {
        Harmony?.UnpatchSelf();
    }
    
    internal static void MainRegisterNewEffect(IEffect effect,bool defaultOff = false, bool superDebug = false)
    {
        ConfigEntry<bool> cfg;
        if (defaultOff)
        {
            cfg = BepInExConfig.Bind<bool>("Allowed Effects",
                effect.Name,
                false,
                effect.Tooltip);
        }
        else
        {
            cfg = BepInExConfig.Bind<bool>("Allowed Effects",
                effect.Name,
                true,
                effect.Tooltip);
        }
        
        DieBehaviour.effectConfigs.Add(cfg);
        //DieBehaviour.favConfigs.Add(fav);
        if (cfg.Value)
            DieBehaviour.AllowedEffects.Add(effect);
       
    }

    // private void Update()
    // {
    //     // Code that runs every frame goes here
    // }
}