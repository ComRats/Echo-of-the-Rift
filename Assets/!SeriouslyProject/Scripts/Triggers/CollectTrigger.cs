using EchoRift;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System;

public class CollectTrigger : BaseTrigger
{
    [Serializable]
    public class CollectEvent
    {
        public string questCode;
        public QuestState needQuestState = QuestState.Active;
        public string requiredItem; // Пока не используется в коде ниже, но оставлено для инспектора
        public string itemNameToCollect;
        public string collectTextHelper;
    }

    [SerializeField] private Vector3 keyMassageOffset;
    [SerializeField] private Vector3 textMassageOffset;
    [SerializeField, Range(0, 30)] private int spriteIndex = 14;
    [SerializeField] private bool autoDestroingAfterAll = false;
    [SerializeField] private List<CollectEvent> eventQueue = new List<CollectEvent>();

    private int currentStepIndex = 0;
    private bool playerInside = false;
    private SpriteCollection sprites;

    [Inject] private MainUI mainUI;
    [Inject] private GameSettings gameSettings;

    private void Start()
    {
        sprites = mainUI.spriteCollection;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            playerInside = true;
            UpdatePrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            playerInside = false;
            ShowButtonPrompt(false, "");
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(gameSettings.useButton))
        {
            TryExecuteCurrentEvent();
        }
    }

    private void UpdatePrompt()
    {
        if (currentStepIndex < eventQueue.Count)
        {
            var ev = eventQueue[currentStepIndex];
            if (CanExecute(ev))
            {
                ShowButtonPrompt(true, ev.collectTextHelper);
            }
            else
            {
                Debug.Log($"[CollectTrigger] Условия шага {currentStepIndex} НЕ выполнены. Ждем квест: {ev.questCode}");
                ShowButtonPrompt(false, "");
            }
        }
    }

    private bool CanExecute(CollectEvent ev)
    {
        // Проверка квеста
        bool questOk = string.IsNullOrEmpty(ev.questCode) || QuestLog.GetQuestState(ev.questCode) == ev.needQuestState;

        // Проверка предмета (закомментирована, пока нет HasItem)
        bool itemOk = true;
        /*
        if (!string.IsNullOrEmpty(ev.requiredItem)) {
            itemOk = mainUI.inventoryManager.HasItem(ev.requiredItem);
        }
        */

        return questOk && itemOk;
    }

    private void TryExecuteCurrentEvent()
    {
        if (currentStepIndex >= eventQueue.Count) return;

        var currentEvent = eventQueue[currentStepIndex];

        if (CanExecute(currentEvent))
        {
            Debug.Log($"[CollectTrigger] Выполнен шаг {currentStepIndex}. Предмет: {currentEvent.itemNameToCollect}");

            if (!string.IsNullOrEmpty(currentEvent.itemNameToCollect))
            {
                mainUI.inventoryManager?.AddItem(currentEvent.itemNameToCollect);
            }

            currentStepIndex++;

            if (currentStepIndex >= eventQueue.Count)
            {
                Debug.Log("[CollectTrigger] Очередь полностью завершена");
                ShowButtonPrompt(false, "");
                if (autoDestroingAfterAll) gameObject.SetActive(false);
            }
            else
            {
                UpdatePrompt();
            }

            DialogueManager.SendUpdateTracker();
        }
        else
        {
            Debug.Log($"[CollectTrigger] Нельзя выполнить шаг {currentStepIndex}. Условия не соблюдены.");
        }
    }

    private void ShowButtonPrompt(bool show, string text)
    {
        GameMassage.ButtonMassageWithText(gameObject, false, null, "", Vector3.zero, Vector3.zero);

        if (show && sprites != null && sprites.sprites != null && spriteIndex < sprites.sprites.Count)
        {
            GameMassage.ButtonMassageWithText(
                gameObject,
                true,
                sprites.sprites[spriteIndex],
                text,
                keyMassageOffset,
                textMassageOffset,
                textColor: Color.yellow
            );
        }
    }
}