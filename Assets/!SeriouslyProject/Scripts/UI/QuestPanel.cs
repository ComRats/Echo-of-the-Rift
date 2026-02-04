using PixelCrushers.DialogueSystem;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class QuestPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI questNameText;

    private void OnEnable()
    {
        if (DialogueManager.hasInstance)
        {
            DialogueManager.instance.receivedUpdateTracker += RefreshQuestDisplay;

            // Проверяем, инициализирована ли среда Lua и загружена ли база
            // Если база еще не готова, RefreshQuestDisplay просто ничего не сделает
            RefreshQuestDisplay();
        }
    }

    private void RefreshQuestDisplay()
    {
        // ГЛАВНАЯ ПРОВЕРКА: Если базы нет в Lua, выходим, чтобы не спамить ошибками
        if (!DialogueManager.hasInstance || DialogueManager.masterDatabase == null)
        {
            RefreshQuestDisplay();
            return;
        }

        // Теперь безопасно лезем в QuestLog
        string[] activeQuests = QuestLog.GetAllQuests(QuestState.Active);

        if (activeQuests != null && activeQuests.Length > 0)
        {
            string latestQuest = activeQuests[activeQuests.Length - 1];
            questNameText.text = QuestLog.GetQuestTitle(latestQuest);
        }
        else
        {
            questNameText.text = "Нет активных заданий";
        }
    }
}