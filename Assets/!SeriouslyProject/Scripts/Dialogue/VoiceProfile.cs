using UnityEngine;

namespace EchoRift.Dialogue
{
    /// <summary>
    /// ScriptableObject с настройками голоса NPCw
    /// </summary>
    [CreateAssetMenu(fileName = "VoiceProfile", menuName = "EchoRift/Voice Profile")]
    public class VoiceProfile : ScriptableObject
    {
        [Tooltip("Короткие звуки (30-100мс) для воспроизведения при печати текста")]
        public AudioClip[] clips;

        [Tooltip("Минимальный pitch")]
        [Range(0.5f, 2f)] public float pitchMin = 0.9f;

        [Tooltip("Максимальный pitch")]
        [Range(0.5f, 2f)] public float pitchMax = 1.1f;

        [Tooltip("Громкость")]
        [Range(0f, 1f)] public float volume = 1f;

        [Tooltip("Воспроизводить звук каждые N символов (1 = каждый символ)")]
        public int playEveryNChars = 1; 

        [Tooltip("Пропускать пробелы и знаки препинания")]
        public bool skipWhitespaceAndPunctuation = true;
    }
}
