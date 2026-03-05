using EchoRift;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;
using AudioManager.Core;
using AudioManager.Locator;
using UnityEngine.Events;
using Unity.VisualScripting;

public class CollectTrigger : BaseTrigger
{
    [Serializable]
    public class CollectEvent
    {
        [BoxGroup("General")] public string questCode;
        [BoxGroup("General")] public QuestState needQuestState = QuestState.Active;
        [BoxGroup("General")] public QuestState setStateAfterStep = (QuestState)0;
        [BoxGroup("General")] public string itemNameToCollect;
        [BoxGroup("General")] public string collectTextHelper;
        [BoxGroup("General")] public string getItemText = "�� ��������: ";
        [BoxGroup("General")] public float textVisibleDelay = 2.5f;
        [BoxGroup("Logic")] public bool removeEventAfterStep = true;
        [BoxGroup("Logic")] public bool isRepeatable = false;

        [BoxGroup("Inventory Requirement")] public string itemNameToHas;
        [ShowIf("@!string.IsNullOrEmpty(itemNameToHas)"), BoxGroup("Inventory Requirement")]
        public bool removeItemAfterStep = true;

        [BoxGroup("Audio")] public string musicName;

        [Space, GUIColor(0.4f, 0.8f, 1f)]
        public bool isMinigame;

        [ShowIf("isMinigame"), BoxGroup("Minigame Settings")]
        [Range(0, 1)] public float startFill = 0.1f;
        [ShowIf("isMinigame"), BoxGroup("Minigame Settings")]
        public float clickPower = 0.1f;
        [ShowIf("isMinigame"), BoxGroup("Minigame Settings")]
        public float drainSpeed = 0.05f;
        [ShowIf("isMinigame"), BoxGroup("Minigame Settings")]
        public string minigameStartText = "������ ������� ���!";
    }

    [SerializeField, BoxGroup("View")] private Vector3 keyMassageOffset;
    [SerializeField, BoxGroup("View")] private Vector3 textMassageOffset;
    [SerializeField, BoxGroup("View"), Range(0, 30)] private int spriteIndex = 14;

    [SerializeField] private UnityEvent onTriggerEnter;
    [SerializeField] private List<CollectEvent> eventQueue = new List<CollectEvent>();

    private int currentStepIndex;
    private bool playerInside;
    private bool minigameActive;
    private bool lastUIState;

    private SpriteCollection sprites;
    private ClickBarUI clickBar;
    private FishingUI fishingUI;
    private IAudioManager audioService;

    [Inject] private MainUI mainUI;
    [Inject] private GameSettings gameSettings;

    private void Start()
    {
        sprites = mainUI.spriteCollection;
        clickBar = mainUI.fishingUI.clickBar;
        fishingUI = mainUI.fishingUI;
        lastUIState = mainUI.isOpenUI;
        audioService = ServiceLocator.GetService();
    }

    private void Update()
    {
        if (!playerInside) return;

        // Обновляем видимость промпта только при изменении состояния UI
        if (lastUIState != mainUI.isOpenUI)
        {
            lastUIState = mainUI.isOpenUI;

            if (mainUI.isOpenUI)
            {
                ShowButtonPrompt(false, "");
            }
            else if (!minigameActive)
            {
                UpdatePrompt();
            }
        }

        if (currentStepIndex < eventQueue.Count && CanExecute(eventQueue[currentStepIndex]))
        {
            onTriggerEnter?.Invoke();
        }

        if (mainUI.isOpenUI) return;

        if (minigameActive)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickBar.AddProgress(eventQueue[currentStepIndex].clickPower);
            }
        }
        else if (Input.GetKeyDown(gameSettings.useButton))
        {
            TryExecuteCurrentEvent();
        }
    }

    private void TryExecuteCurrentEvent()
    {
        if (currentStepIndex >= eventQueue.Count) return;

        var ev = eventQueue[currentStepIndex];
        if (!CanExecute(ev)) return;

        if (ev.isMinigame) StartMinigame(ev);
        else FinishCurrentStep();
    }

    private void StartMinigame(CollectEvent ev)
    {
        minigameActive = true;
        mainUI.canOpenUI = false;
        ShowButtonPrompt(false, "");
        fishingUI.ShowMinigameHint(ev.minigameStartText);
        clickBar.Setup(ev.startFill, ev.drainSpeed, FinishCurrentStep);
    }

    private void FinishCurrentStep()
    {
        minigameActive = false;
        mainUI.canOpenUI = true;
        var ev = eventQueue[currentStepIndex];

        // Воспроизведение музыки если указана
        if (!string.IsNullOrEmpty(ev.musicName))
        {
            audioService?.Play(ev.musicName);
        }

        if (!string.IsNullOrEmpty(ev.itemNameToCollect))
        {
            string message = ev.getItemText + mainUI.inventoryManager.FindItemDataByName(ev.itemNameToCollect).itemGameName;
            fishingUI.ShowMinigameHint(message, ev.textVisibleDelay);
            mainUI.inventoryManager?.AddItem(ev.itemNameToCollect);
        }
        else
        {
            fishingUI.HideText();
        }

        if (!string.IsNullOrEmpty(ev.itemNameToHas) && ev.removeItemAfterStep)
        {
            mainUI.inventoryManager?.RemoveItem(ev.itemNameToHas);
        }

        if (!string.IsNullOrEmpty(ev.questCode) && ev.setStateAfterStep != (QuestState)0)
        {
            QuestLog.SetQuestState(ev.questCode, ev.setStateAfterStep);
        }

        if (ev.removeEventAfterStep && !ev.isRepeatable)
        {
            eventQueue.RemoveAt(currentStepIndex);
        }
        else
        {
            currentStepIndex++;
        }

        if (currentStepIndex >= eventQueue.Count)
        {
            ShowButtonPrompt(false, "");
        }
        else
        {
            UpdatePrompt();
        }

        DialogueManager.SendUpdateTracker();
    }

    private bool CanExecute(CollectEvent ev)
    {
        bool questConditionsMet = string.IsNullOrEmpty(ev.questCode) ||
            QuestLog.GetQuestState(ev.questCode) == ev.needQuestState;

        bool inventoryConditionsMet = string.IsNullOrEmpty(ev.itemNameToHas) ||
            (mainUI.inventoryManager?.HasItem(ev.itemNameToHas) ?? false);

        return questConditionsMet && inventoryConditionsMet;
    }

    private void UpdatePrompt()
    {
        if (currentStepIndex >= eventQueue.Count) return;
        var ev = eventQueue[currentStepIndex];
        ShowButtonPrompt(CanExecute(ev), ev.collectTextHelper);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out _))
        {
            playerInside = true;
            if (!mainUI.isOpenUI)
                UpdatePrompt();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out _))
        {
            playerInside = false;
            minigameActive = false;
            mainUI.canOpenUI = true;
            clickBar.Hide();
            ShowButtonPrompt(false, "");
        }
    }

    private void ShowButtonPrompt(bool show, string text)
    {
        GameMassage.ButtonMassageWithText(gameObject, false, null, "", Vector3.zero, Vector3.zero);
        if (show && sprites?.sprites != null && spriteIndex < sprites.sprites.Count)
        {
            GameMassage.ButtonMassageWithText(gameObject, true, sprites.sprites[spriteIndex], text, keyMassageOffset, textMassageOffset, textColor: Color.yellow);
        }
    }
}