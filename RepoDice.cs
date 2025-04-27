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
using RepoDice.Visual;
using UnityEngine;
using UnityEngine.InputSystem;

namespace RepoDice;

//Yes I know my coding is a bit weird 

[BepInPlugin("Slayer6409.EmergencyDiceREPO", "Emergency Dice REPO", "1.0.6")]
[BepInDependency(REPOLib.MyPluginInfo.PLUGIN_GUID)]
[BepInDependency("bulletbot.moreupgrades", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("WesleysEnemies", BepInDependency.DependencyFlags.SoftDependency)]
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
    public static ConfigEntry<float> throwSpeed;
    public static ConfigEntry<bool> DebugMenuClosesAfter;
    public static ConfigEntry<bool> Bald;
    public static ConfigEntry<bool> IWannaSeeWhatGlitchSees;
    public static ConfigEntry<bool> SpoilerMode;
    public static ConfigEntry<bool> Copyright;
    public static ConfigEntry<bool> muteFreebird;
    public static ConfigEntry<bool> removeSaint;
    public static ConfigEntry<bool> glitchedRespawn;
    public static ConfigEntry<bool> glitchyMode;
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

    public static readonly string slayerSteamID = "76561198077184650",
        machoSteamID = "76561198216220844",
        glitchSteamID = "76561198984467725",
        lizzieSteamID = "76561199094139351";
    internal Harmony? Harmony { get; set; }
    public static Sprite WarningExclamation, WarningDeath, WarningLuck, WarningGlitch;
    public static GameObject Gambler, Saint, Sacrificer, RainbowDice;
    public static Item GlitchedDice;
    public static Material GlitchyMaterial, savedMaterial;

    public static GameObject JumpscareCanvasPrefab,
        JumpscareOBJ;

    public static List<string> RegisteredDiceNames = new List<string>();
    public static Jumpscare JumpscareScript;
    
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
        sounds.Add("Bald", LoadedAssets.LoadAsset<AudioClip>("Glitchimnotbald"));
        sounds.Add("Bald2", LoadedAssets.LoadAsset<AudioClip>("GlitchGodfuckingdammit"));
        sounds.Add("purr", LoadedAssets.LoadAsset<AudioClip>("purr"));

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
        WarningGlitch = LoadedAssets.LoadAsset<Sprite>("glitch");
        GlitchyMaterial = LoadedAssets.LoadAsset<Material>("GlitchedDie");
        JumpscareCanvasPrefab = LoadedAssets.LoadAsset<GameObject>("JumpscareCanvas");
        JumpscareCanvasPrefab.AddComponent<Jumpscare>();

        #region Dice

        Gambler = LoadedAssets.LoadAsset<GameObject>("Gambler");
        RegisteredDiceNames.Add("Gambler");
        Gambler.AddComponent<Gambler>();
        REPOLib.Modules.Valuables.RegisterValuable(Gambler);
        Saint = LoadedAssets.LoadAsset<GameObject>("Saint");
        RegisteredDiceNames.Add("Saint");
        Saint.AddComponent<Saint>();
        REPOLib.Modules.Valuables.RegisterValuable(Saint);
        Sacrificer = LoadedAssets.LoadAsset<GameObject>("Sacrificer");
        RegisteredDiceNames.Add("Sacrificer");
        Sacrificer.AddComponent<Sacrificer>();
        REPOLib.Modules.Valuables.RegisterValuable(Sacrificer);
        
        RainbowDice = LoadedAssets.LoadAsset<GameObject>("RainbowDice");
        RegisteredDiceNames.Add("RainbowDice");
        RainbowDice.AddComponent<Rainbow>();
        REPOLib.Modules.Valuables.RegisterValuable(RainbowDice);
        
        GlitchedDice = LoadedAssets.LoadAsset<Item>("GlitchedDie");
        RegisteredDiceNames.Add("GlitchedDie_ItemDie");
        GlitchedDice.prefab.AddComponent<GlitchedDie>();
        REPOLib.Modules.Items.RegisterItem(GlitchedDice);


        
        #endregion
       
        networker = LoadedAssets.LoadAsset<GameObject>("Dice Manager");
        networker.AddComponent<Networker>();
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

    public static void SuperLog(string message, LogLevel level = LogLevel.Info)
    {
        if (ExtendedLogging.Value)
        {
            Logger.Log(level, message);
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
        IWannaSeeWhatGlitchSees = Config.Bind<bool>(
            "Client Side",
            "I Wanna See What Glitch Sees",
            false,
            "Shows you what Glitch Sees");
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
        throwSpeed = Config.Bind<float>(
            "Dice",
            "Throw Speed",
            8f,
            "Sets how hard you have to throw the dice to trigger the roll");
        SpoilerMode = Config.Bind<bool>(
            "Dice",
            "Spoiler Mode",
            false,
            "If set to true, when it reads out your roll, it will just say what you rolled instead of a funny hint");
        Copyright = Config.Bind<bool>(
            "Client Side",
            "Copyright Free",
            false,
            "Changes Copyright Audio");
        muteFreebird = Config.Bind<bool>(
            "Client Side",
            "Mute Freebird",
            false,
            "Mutes Freebird enemies just keeping them fast");
        removeSaint = Config.Bind<bool>(
            "Dice",
            "Saint Roll",
            false,
            "Removes the Select Effect from Saint, making it just a great roll");
        glitchedRespawn = Config.Bind<bool>(
            "Dice",
            "Glitched Die Respawns",
            false,
            "Makes the glitched die respawn on each level when bought");
        glitchyMode = Config.Bind<bool>(
            "Client Side",
            "Glitchy Mode",
            true,
            "Makes certain things a bit \"Glitchy\" will not disable the enemies or dice ones though");
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
        if (superDebug)
        {
            DieBehaviour.AllEffects.Add(effect);
        }
        else
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
            DieBehaviour.AllEffects.Add(effect);
        }
    }
}
public class BundleDumper : MonoBehaviour
{
    public void DumpBundles()
    {
        foreach (var bundle in AssetBundle.GetAllLoadedAssetBundles())
        {
            Debug.Log($"[Bundle] {bundle.name}");
            string[] assets = bundle.GetAllAssetNames();
            foreach (string asset in assets)
            {
                Debug.Log($"    [Asset] {asset}");
            }
        }
    }
}