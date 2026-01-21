using AudioManager.Core;
using AudioManager.Settings;
using System.Collections.Generic;
using UnityEngine;

namespace AudioManager.Helper
{
    public static class SettingsHelper
    {
        public static void SetupSounds(out IDictionary<string, AudioSourceWrapper> sounds, AudioSourceSetting[] settings, GameObject gameObject)
        {
            sounds = new Dictionary<string, AudioSourceWrapper>();

            if (settings is null)
            {
                return;
            }

            CreateAndRegisterSound(sounds, settings, gameObject);
        }

        public static void CreateAndRegisterSound(IDictionary<string, AudioSourceWrapper> sounds, AudioSourceSetting[] settings, GameObject gameObject)
        {
            foreach (var settingGroup in settings)
            {
                if (settingGroup is null || settingGroup.audioClips is null) continue;

                foreach (var clipData in settingGroup.audioClips)
                {
                    if (clipData is null) continue;
                    CreateAndRegisterSound(sounds, clipData, gameObject);
                }
            }
        }

        public static void CreateAndRegisterSound(IDictionary<string, AudioSourceWrapper> sounds, AudioClips setting, GameObject gameObject)
        {
            AudioHelper.AttachAudioSource(out setting.source, gameObject, setting.audioClip, setting.mixerGroup, setting.loop, setting.volume, setting.pitch, setting.spatialBlend, setting.dopplerLevel, setting.spreadAngle, setting.volumeRolloff, setting.minDistance, setting.maxDistance);

            if (IsSoundRegistered(sounds, setting.soundName))
            {
                return;
            }

            AudioSourceWrapper wrapper = new AudioSourceWrapper(setting.source);

            wrapper.soundTypeIndex = (int)setting.type;

            RegisterSound(sounds, (setting.soundName, wrapper));
        }

        public static bool IsSoundRegistered(IDictionary<string, AudioSourceWrapper> sounds, string soundName)
        {
            return sounds.ContainsKey(soundName);
        }

        public static void RegisterSound(IDictionary<string, AudioSourceWrapper> sounds, (string soundName, AudioSourceWrapper soundSource) keyValuePair)
        {
            sounds.Add(keyValuePair.soundName, keyValuePair.soundSource);
        }
    }
}