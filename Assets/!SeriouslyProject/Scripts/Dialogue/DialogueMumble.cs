using UnityEngine;
using PixelCrushers.DialogueSystem;
using EchoRift.Dialogue;

/// <summary>
/// Воспроизводит звуки-заменители речи ("mumble") при печати диалогового текста.
/// Поддерживает разные голоса для разных NPC через VoiceProfile.
/// </summary>
[RequireComponent(typeof(AudioSource))]
public class DialogueMumble : MonoBehaviour
{
    [System.Serializable]
    public struct ActorVoiceEntry
    {
        [Tooltip("Имя актора в Dialogue System (поле Display Name или Actor Name)")]
        public string actorName;
        public VoiceProfile voiceProfile;
    }

    [Header("Голоса")]
    [Tooltip("Голос по умолчанию если актор не найден в списке")]
    [SerializeField] private VoiceProfile defaultVoice;

    [Tooltip("Голоса конкретных NPC")]
    [SerializeField] private ActorVoiceEntry[] actorVoices;

    [Header("Компоненты")]
    [SerializeField] private TextMeshProTypewriterEffect typewriter;

    private AudioSource audioSource;
    private VoiceProfile currentVoice;
    private int charCount;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.playOnAwake = false;

        if (typewriter == null)
            typewriter = GetComponent<TextMeshProTypewriterEffect>();
    }

    private void OnEnable()
    {
        if (typewriter != null)
            typewriter.onCharacter.AddListener(OnCharacter);

        DialogueManager.instance.conversationStarted += OnConversationStarted;
    }

    private void OnDisable()
    {
        if (typewriter != null)
            typewriter.onCharacter.RemoveListener(OnCharacter);

        if (DialogueManager.instance != null)
            DialogueManager.instance.conversationStarted -= OnConversationStarted;
    }

    private void OnConversationStarted(Transform actor)
    {
        charCount = 0;
        RefreshVoiceForCurrentSpeaker();
    }

    /// <summary>
    /// Вызывается Dialogue System когда начинается новая реплика.
    /// Можно вызвать вручную из Sequencer: SendMessage(RefreshVoice)
    /// </summary>
    public void RefreshVoiceForCurrentSpeaker()
    {
        charCount = 0;

        var state = DialogueManager.currentConversationState;
        if (state == null)
        {
            currentVoice = defaultVoice;
            return;
        }

        string speakerName = state.subtitle?.speakerInfo?.nameInDatabase;
        currentVoice = FindVoiceForActor(speakerName);
    }

    private VoiceProfile FindVoiceForActor(string actorName)
    {
        if (!string.IsNullOrEmpty(actorName) && actorVoices != null)
        {
            foreach (var entry in actorVoices)
            {
                if (string.Equals(entry.actorName, actorName, System.StringComparison.OrdinalIgnoreCase))
                    return entry.voiceProfile;
            }
        }
        return defaultVoice;
    }

    private void OnCharacter()
    {
        if (currentVoice == null) return;
        if (currentVoice.clips == null || currentVoice.clips.Length == 0) return;

        if (currentVoice.skipWhitespaceAndPunctuation)
        {
            var subtitleText = DialogueManager.currentConversationState?.subtitle?.formattedText?.text;
            if (!string.IsNullOrEmpty(subtitleText) && charCount < subtitleText.Length)
            {
                char c = subtitleText[charCount];
                if (char.IsWhiteSpace(c) || char.IsPunctuation(c))
                {
                    charCount++;
                    return;
                }
            }
        }

        charCount++;

        if (charCount % currentVoice.playEveryNChars != 0) return;

        var clip = currentVoice.clips[Random.Range(0, currentVoice.clips.Length)];
        audioSource.pitch = Random.Range(currentVoice.pitchMin, currentVoice.pitchMax);
        audioSource.PlayOneShot(clip, currentVoice.volume);
    }
}
