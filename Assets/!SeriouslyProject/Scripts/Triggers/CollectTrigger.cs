using EchoRift;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using Zenject;
using System.Collections.Generic;
using System;
using Sirenix.OdinInspector;

public class CollectTrigger : BaseTrigger
{
    [Serializable]
    public class CollectEvent
    {
        [BoxGroup("General")] public string questCode;
        [BoxGroup("General")] public QuestState needQuestState = QuestState.Active;
        [BoxGroup("General")] public string itemNameToCollect;
        [BoxGroup("General")] public string collectTextHelper;

        [Space, GUIColor(0.4f, 0.8f, 1f)]
        public bool isMinigame;

        [ShowIf("isMinigame"), BoxGroup("Minigame Settings")]
        [Range(0, 1)] public float startFill = 0.1f;
        [ShowIf("isMinigame"), BoxGroup("Minigame Settings")]
        public float clickPower = 0.1f;
        [ShowIf("isMinigame"), BoxGroup("Minigame Settings")]
        public float drainSpeed = 0.05f;
    }

    [SerializeField, BoxGroup("View")] private Vector3 keyMassageOffset;
    [SerializeField, BoxGroup("View")] private Vector3 textMassageOffset;
    [SerializeField, BoxGroup("View"), Range(0, 30)] private int spriteIndex = 14;

    [SerializeField, Space] private bool autoDestroingAfterAll;
    [SerializeField] private List<CollectEvent> eventQueue = new List<CollectEvent>();

    private int currentStepIndex;
    private bool playerInside;
    private bool minigameActive;

    private SpriteCollection sprites;
    private ClickBarUI clickBar;

    [Inject] private MainUI mainUI;
    [Inject] private GameSettings gameSettings;

    private void Start()
    {
        sprites = mainUI.spriteCollection;
        clickBar = mainUI.fishingUI.clickBar;
    }

    private void Update()
    {
        if (!playerInside) return;

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
        ShowButtonPrompt(false, "");
        clickBar.Setup(ev.startFill, ev.drainSpeed, FinishCurrentStep);
    }

    private void FinishCurrentStep()
    {
        minigameActive = false;
        var ev = eventQueue[currentStepIndex];

        if (!string.IsNullOrEmpty(ev.itemNameToCollect))
            mainUI.inventoryManager?.AddItem(ev.itemNameToCollect);

        currentStepIndex++;

        if (currentStepIndex >= eventQueue.Count)
        {
            ShowButtonPrompt(false, "");
            if (autoDestroingAfterAll) gameObject.SetActive(false);
        }
        else UpdatePrompt();

        DialogueManager.SendUpdateTracker();
    }

    private bool CanExecute(CollectEvent ev) =>
        string.IsNullOrEmpty(ev.questCode) || QuestLog.GetQuestState(ev.questCode) == ev.needQuestState;

    private void UpdatePrompt()
    {
        if (currentStepIndex >= eventQueue.Count) return;
        var ev = eventQueue[currentStepIndex];
        ShowButtonPrompt(CanExecute(ev), ev.collectTextHelper);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out _)) { playerInside = true; UpdatePrompt(); }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out _)) { playerInside = false; minigameActive = false; clickBar.Hide(); ShowButtonPrompt(false, ""); }
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