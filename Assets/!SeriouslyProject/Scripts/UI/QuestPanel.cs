using PixelCrushers;
using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class QuestPanel : MonoBehaviour, IMessageHandler
{
    [SerializeField] private TextMeshProUGUI questNameText;
    [SerializeField] private List<string> quests = new List<string>();

    private void Awake()
    {
        QuestLog.SetQuestState("LostInTheAges", QuestState.Active);
        Debug.Log("[QuestPanel] Скрипт проснулся в Awake.");
    }

    private IEnumerator Start()
    {
        Debug.Log("[QuestPanel] Start запущен. Ждем DialogueManager...");

        while (!DialogueManager.hasInstance)
        {
            yield return null;
        }

        Debug.Log("[QuestPanel] DialogueManager найден. Ждем базу данных...");

        while (DialogueManager.masterDatabase == null)
        {
            yield return null;
        }

        yield return new WaitForEndOfFrame();

        Debug.Log("[QuestPanel] Инициализация списка квестов.");
        RefreshInitialList();
    }

    private void RefreshInitialList()
    {
        string[] activeQuests = QuestLog.GetAllQuests(QuestState.Active);

        if (activeQuests != null)
        {
            quests = activeQuests.ToList();
            Debug.Log($"[QuestPanel] Найдено активных квестов: {quests.Count}");

            foreach (var q in quests)
            {
                Debug.Log($"[QuestPanel] Активный квест: {q}");
            }
        }

        UpdateQuestDisplay();
    }

    private void OnEnable()
    {
        MessageSystem.AddListener(this, "OnQuestStateChange", "");
        Debug.Log("[QuestPanel] MessageSystem Listener добавлен.");
    }

    private void OnDisable()
    {
        MessageSystem.RemoveListener(this, "OnQuestStateChange", "");
    }

    public void OnMessage(MessageArgs messageArgs)
    {
        // ВНИМАНИЕ: Проверяем ЛЮБОЕ сообщение для теста
        Debug.Log($"[QuestPanel] Пришло сообщение: {messageArgs.message} с параметром: {messageArgs.parameter}");

        if (messageArgs.message == "OnQuestStateChange")
        {
            HandleQuestChange(messageArgs.parameter);
        }
    }

    private void HandleQuestChange(string questName)
    {
        if (string.IsNullOrEmpty(questName)) return;

        bool isActive = QuestLog.IsQuestActive(questName);
        Debug.Log($"[QuestPanel] Изменение квеста {questName}. Активен: {isActive}");

        if (isActive && !quests.Contains(questName))
        {
            quests.Add(questName);
        }
        else if (!isActive && quests.Contains(questName))
        {
            quests.Remove(questName);
        }

        UpdateQuestDisplay();
    }

    private void UpdateQuestDisplay()
    {
        if (quests != null && quests.Count > 0)
        {
            string questToShow = quests[quests.Count - 1];
            questNameText.text = QuestLog.GetQuestTitle(questToShow);
        }
        else
        {
            questNameText.text = "Нет активных квестов";
        }
    }
}