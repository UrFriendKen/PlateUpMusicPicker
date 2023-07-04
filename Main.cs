using HarmonyLib;
using Kitchen.Components;
using KitchenData;
using KitchenMods;
using PreferenceSystem;
using PreferenceSystem.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace KitchenMusicPicker
{
    public class Main : IModInitializer
    {
        public const string MOD_GUID = $"IcedMilo.PlateUp.{MOD_NAME}";
        public const string MOD_NAME = "Music Picker";
        public const string MOD_VERSION = "0.1.0";

        Harmony _harmony;

        internal const string MUSIC_THEME_ID = "musicTheme";
        internal static PreferenceSystemManager PrefManager;

        public Main()
        {
            _harmony = new Harmony(MOD_GUID);
            _harmony.PatchAll(Assembly.GetExecutingAssembly());
        }

        public void PostActivate(KitchenMods.Mod mod)
        {
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");

        }

        public void PreInject()
        {
            if (PrefManager == null)
            {
                MusicSource musicSource = GameObject.FindObjectOfType<MusicController>()?.Source;
                if (musicSource?.SoundTracks != null)
                {
                    List<int> ids = new List<int>()
                    {
                        -1
                    };
                    List<string> names = new List<string>()
                    {
                        "Disabled"
                    };
                    Dictionary<string, int> settingCount = new Dictionary<string, int>();
                    for (int i = 0; i < musicSource.SoundTracks.Count; i++)
                    {
                        RestaurantSetting setting = musicSource.SoundTracks[i].Setting;
                        int settingID = setting?.ID ?? 0;
                        
                        string name;
                        if (musicSource.SoundTracks[i].IsMenuTrack)
                            name = "Menu";
                        else if (settingID == 0)
                            name = "Default";
                        else
                            name = setting.Name.IsNullOrEmpty() ? setting.name : setting.Name;

                        if (!settingCount.TryGetValue(name, out int count))
                        {
                            count = 0;
                        }
                        settingCount[name] = count + 1;

                        ids.Add(i);
                        names.Add($"{name}{$" {(settingCount[name] == 1 ? string.Empty : settingCount[name])}"}");
                    }


                    PrefManager = new PreferenceSystemManager(MOD_GUID, MOD_NAME);
                    PrefManager
                        .AddLabel("Music Theme")
                        .AddOption<int>(
                            MUSIC_THEME_ID,
                            0,
                            ids.ToArray(),
                            names.ToArray())
                        .AddSpacer()
                        .AddSpacer();
                    PrefManager.RegisterMenu(PreferenceSystemManager.MenuType.PauseMenu);
                }
            }
        }

        public void PostInject()
        {
        }

        #region Logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
}
