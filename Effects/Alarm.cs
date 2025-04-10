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

public class Alarm : IEffect
{
    public string Name => "Alarm";
    public EffectType Outcome => EffectType.Awful;
    public bool ShowDefaultTooltip => true;
    public string Tooltip => "Sorry";
    
    public static Dictionary<string, ConfigEntry<bool>> useSounds = new Dictionary<string, ConfigEntry<bool>>();
    public static ConfigEntry<int> enemyDetectionRange;
    
    public void Use(PlayerAvatar roller)
    {
        if (roller.GetComponent<AlarmAttach>() == null)
        {
            var alatch = roller.gameObject.AddComponent<AlarmAttach>();
            RunManagerPatch.alarms.Add(alatch);
        }
    }
    public static void ConfigStuff()
    {
        useSounds.Add("FireAlarm",RepoDice.BepInExConfig.Bind<bool>(
            "Alarm",
            "FireAlarm",
            false,
            "Allows this sound to play when the alarm is fired"));
        useSounds.Add("AudioTest",RepoDice.BepInExConfig.Bind<bool>(
            "Alarm",
            "AudioTest",
            false,
            "Allows this sound to play when the alarm is fired"));
        useSounds.Add("BANANA",RepoDice.BepInExConfig.Bind<bool>(
            "Alarm",
            "BANANA",
            false,
            "Allows this sound to play when the alarm is fired"));
        useSounds.Add("disconnect",RepoDice.BepInExConfig.Bind<bool>(
            "Alarm",
            "disconnect",
            false,
            "Allows this sound to play when the alarm is fired"));
        useSounds.Add("DoorLeft",RepoDice.BepInExConfig.Bind<bool>(
            "Alarm",
            "DoorLeft",
            false,
            "Allows this sound to play when the alarm is fired"));
        useSounds.Add("DoorRight",RepoDice.BepInExConfig.Bind<bool>(
            "Alarm",
            "DoorRight",
            false,
            "Allows this sound to play when the alarm is fired"));
        useSounds.Add("mah-boi",RepoDice.BepInExConfig.Bind<bool>(
            "Alarm",
            "mah-boi",
            false,
            "Allows this sound to play when the alarm is fired"));
        useSounds.Add("tuturu",RepoDice.BepInExConfig.Bind<bool>(
            "Alarm",
            "tuturu",
            false,
            "Allows this sound to play when the alarm is fired"));
        useSounds.Add("WindowsError",RepoDice.BepInExConfig.Bind<bool>(
            "Alarm",
            "WindowsError",
            false,
            "Allows this sound to play when the alarm is fired"));
        enemyDetectionRange = RepoDice.BepInExConfig.Bind<int>(
            "Alarm",
            "Enemy Detection Range",
            5,
            "How far away the enemies can hear the alarm");
        foreach (var sound in useSounds)
        {
            sound.Value.SettingChanged += (sender, args) => AlarmAttachManager.UpdateAllSoundLists();
        }
        
    }
    
}
public static class AlarmAttachManager
{
    public static readonly List<AlarmAttach> Instances = new();

    public static void UpdateAllSoundLists()
    {
        foreach (var alarm in Instances)
        {
            alarm.UpdateSoundList();
        }
    }
}

public class AlarmAttach : MonoBehaviour
{
    private const int TimerMin = 10;
    private const int TimerMax = 60;

    private float _timer;
    private List<string> _alarmSounds = new List<string>();
    private Vector3 offset = new Vector3(0, 1.5f, 0);
    void Start()
    {
        AlarmAttachManager.Instances.Add(this);
        UpdateSoundList();
    }

    void OnDestroy()
    {
        AlarmAttachManager.Instances.Remove(this);
    }

    public void UpdateSoundList()
    {
        _alarmSounds.Clear();
        foreach (var sound in Alarm.useSounds)
        {
            if(sound.Value.Value) _alarmSounds.Add(sound.Key);
        }
    }

    public void Update()
    {
        if (_timer < 0)
        {
            var alarmPosition = this.transform.position + offset;
            if (_alarmSounds.Count > 0)
            {
                var chosenSound = _alarmSounds[Random.Range(0, _alarmSounds.Count)];
                Networker.Instance.photonView.RPC("PlayAudioAtPoint", RpcTarget.All, alarmPosition, chosenSound);
            }
            _timer = Random.Range(TimerMin, TimerMax+1);
            if (EnemyDirector.instance != null)
            {
                EnemyDirector.instance.SetInvestigate(alarmPosition-offset + Vector3.up * 0.2f, 5f);
            }
        }
        _timer -= Time.deltaTime;
    }

    public void RemoveTimer()
    {
        Destroy(this);
    }
}