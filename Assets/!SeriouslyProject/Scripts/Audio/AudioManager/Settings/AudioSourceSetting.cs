using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Sirenix.OdinInspector;

namespace AudioManager.Settings
{
    [CreateAssetMenu(fileName = "AudioSourceSettings", menuName = "AudioManager/AudioSourceSettings", order = 1)]
    public class AudioSourceSetting : ScriptableObject
    {

        [ListDrawerSettings(ShowIndexLabels = true)]
        [Searchable]
        public List<AudioClips> audioClips = new List<AudioClips>();
    }

    [System.Serializable]
    public class AudioClips
    {
        [TitleGroup("Base Settings")]
        public string soundName;
        public AudioClip audioClip;

        [BoxGroup("Mixer & Playback")]
        public AudioMixerGroup mixerGroup;
        [BoxGroup("Mixer & Playback")]
        public bool loop = false;

        [BoxGroup("Audio Properties")]
        [Range(0f, 1f)]
        public float volume = 1f;

        [BoxGroup("Audio Properties")]
        [Range(-3f, 3f)]
        public float pitch = 1f;

        [Space(10)]
        [Title("Spatial Settings (3D)")]
        [Range(0f, 1f)]
        public float spatialBlend = 0f;

        // --- Исправленные атрибуты Odin ---
        // Используем полное имя Sirenix.OdinInspector.ShowIf, чтобы избежать конфликта с AudioManager

        [Sirenix.OdinInspector.ShowIf("@this.spatialBlend > 0")]
        [BoxGroup("3D Parameters")]
        [Range(0f, 5f)]
        public float dopplerLevel = 1f;

        [Sirenix.OdinInspector.ShowIf("@this.spatialBlend > 0")]
        [BoxGroup("3D Parameters")]
        [Range(0f, 360f)]
        public float spreadAngle = 0f;

        [Sirenix.OdinInspector.ShowIf("@this.spatialBlend > 0")]
        [BoxGroup("3D Parameters")]
        public AudioRolloffMode volumeRolloff = AudioRolloffMode.Logarithmic;

        [Sirenix.OdinInspector.ShowIf("@this.spatialBlend > 0")]
        [BoxGroup("3D Parameters")]
        public float minDistance = 1f;

        [Sirenix.OdinInspector.ShowIf("@this.spatialBlend > 0")]
        [BoxGroup("3D Parameters")]
        public float maxDistance = 500f;

        [HideInInspector]
        public AudioSource source;
    }
}