using EchoRift;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Триггер выполняет Lua-скрипты Dialogue System при входе/выходе игрока.
/// Синтаксис такой же как в поле Script диалога:
///   SetQuestState("MyQuest", "active")
///   ShowAlert("Текст уведомления")
/// </summary>
public class ScriptTrigger : MonoBehaviour
{
    [System.Serializable]
    public class ScriptEntry
    {
        [TextArea(2, 6)]
        public string luaScript;

        [Tooltip("Выполнить только один раз")]
        public bool once = true;

        [HideInInspector]
        public bool executed;
    }

    [Title("On Enter")]
    [SerializeField] private List<ScriptEntry> onEnterScripts = new();
    [SerializeField] private UnityEvent onEnter;

    [Title("On Exit")]
    [SerializeField] private List<ScriptEntry> onExitScripts = new();
    [SerializeField] private UnityEvent onExit;

    [Title("Conversation Settings")]
    [Tooltip("Включить Include Invalid Entries для следующего диалога запущенного с этого триггера. Сбрасывается автоматически после окончания разговора.")]
    [SerializeField] private bool includeInvalidEntries = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Player>(out _)) return;

        if (includeInvalidEntries)
            EnableIncludeInvalid();

        ExecuteScripts(onEnterScripts);
        onEnter?.Invoke();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Player>(out _)) return;

        ExecuteScripts(onExitScripts);
        onExit?.Invoke();
    }

    private void EnableIncludeInvalid()
    {
        DialogueManager.instance.isDialogueEntryValid = IncludeAll;
        DialogueManager.instance.conversationEnded += OnConversationEnded;
    }

    private void OnConversationEnded(Transform actor)
    {
        DialogueManager.instance.isDialogueEntryValid = null;
        DialogueManager.instance.conversationEnded -= OnConversationEnded;
    }

    private bool IncludeAll(DialogueEntry entry) => true;

    private void ExecuteScripts(List<ScriptEntry> scripts)
    {
        foreach (var entry in scripts)
        {
            if (string.IsNullOrWhiteSpace(entry.luaScript)) continue;
            if (entry.once && entry.executed) continue;

            Lua.Run(entry.luaScript, true);
            DialogueManager.SendUpdateTracker();

            entry.executed = true;
        }
    }
}
