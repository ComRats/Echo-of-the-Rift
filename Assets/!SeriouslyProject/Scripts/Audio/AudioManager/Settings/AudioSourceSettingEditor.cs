#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

namespace AudioManager.Settings {
    [CustomEditor(typeof(AudioSourceSetting), true)]
    public class AudioSourceSettingEditor : Editor {
        [SerializeField]
        private AudioSource preview;

        private void OnEnable() {
            preview = EditorUtility.CreateGameObjectWithHideFlags("Audio preview", HideFlags.HideAndDontSave, typeof(AudioSource)).GetComponent<AudioSource>();
        }

        private void OnDisable() {
            DestroyImmediate(preview.gameObject);
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            AudioSourceSetting script = (AudioSourceSetting)target;

            if (script.audioClips == null || script.audioClips.Count == 0) return;

            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Preview Controls", EditorStyles.boldLabel);

            for (int i = 0; i < script.audioClips.Count; i++)
            {
                var clipData = script.audioClips[i];
                string displayName = string.IsNullOrEmpty(clipData.soundName) ? $"Element {i}" : clipData.soundName;

                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField(displayName);

                if (GUILayout.Button("Play"))
                {
                    PlayClip(clipData);
                }

                if (GUILayout.Button("Stop"))
                {
                    Stop();
                }
                EditorGUILayout.EndHorizontal();
            }
        }

        private void PlayClip(AudioClips data) {
            if (data == null || data.audioClip == null) return;

            preview.clip = data.audioClip;
            preview.outputAudioMixerGroup = data.mixerGroup;
            preview.loop = data.loop;
            preview.volume = data.volume;
            preview.pitch = data.pitch;
            preview.spatialBlend = data.spatialBlend;
            preview.dopplerLevel = data.dopplerLevel;
            preview.spread = data.spreadAngle;
            preview.rolloffMode = data.volumeRolloff;
            preview.minDistance = data.minDistance;
            preview.maxDistance = data.maxDistance;
            preview.Play();
        }

        private void Stop()
        {
            if (preview != null) preview.Stop();
        }
    }
}
#endif // UNITY_EDITOR
