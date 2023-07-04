using HarmonyLib;
using Kitchen.Components;
using System.Reflection;
using UnityEngine;

namespace KitchenMusicPicker.Patches
{
    [HarmonyPatch]
    static class MusicSource_Patch
    {
        static MethodInfo m_PlayClip = typeof(MusicSource).GetMethod("PlayClip", BindingFlags.NonPublic | BindingFlags.Instance);

        [HarmonyPatch(typeof(MusicSource), nameof(MusicSource.PlayNewTrack))]
        [HarmonyPrefix]
        static bool Prefix(bool play_menu, ref MusicSource __instance, ref MusicState ___State, ref int ___CurrentTrack, ref SoundSource ___SourceBackground)
        {
            int trackSetID = Main.PrefManager?.Get<int>(Main.MUSIC_THEME_ID) ?? -1;
            if (play_menu || trackSetID == -1 || ___SourceBackground == null)
                return true;
            ___State = MusicState.Loop;
            ___CurrentTrack = Mathf.Clamp(trackSetID, 0, __instance.SoundTracks.Count - 1);
            MusicTrackSet trackSet = __instance.SoundTracks[___CurrentTrack];
            m_PlayClip?.Invoke(__instance, new object[] { trackSet.Intro });
            ___SourceBackground.TransitionTime = 1f;
            ___SourceBackground.Stop();
            return false;
        }
    }
}
