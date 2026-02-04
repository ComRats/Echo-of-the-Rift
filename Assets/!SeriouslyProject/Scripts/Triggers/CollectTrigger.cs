using EchoRift;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using Zenject;

public class CollectTrigger : BaseTrigger
{
    [SerializeField] private Vector3 keyMassageOffset;
    [SerializeField] private Vector3 textMassageOffset;
    [SerializeField, Range(0, 30)] private int spriteIndex = 14;
    [SerializeField] private string questCode = "QuestName";
    [SerializeField] private QuestState needQuestState = QuestState.Active;
    [SerializeField] private string collectTextHelper;
    [SerializeField] private string itemNameToCollect;
    [SerializeField] private bool autoDestroing = false;

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
            if (AppliedStateQuest(questCode, needQuestState))
            {
                ShowButtonPrompt(true);
            }
            playerInside = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            playerInside = false;
            ShowButtonPrompt(false);
        }
    }

    private void Update()
    {
        TryCollectItem();
    }

    private void TryCollectItem() 
    {
        if (Input.GetKey(gameSettings.useButton) &&
            AppliedStateQuest(questCode, needQuestState))
        {
            mainUI.inventoryManager?.AddItem(itemNameToCollect);
            if (autoDestroing) gameObject.SetActive(false);
        }
    }

    private void ShowButtonPrompt(bool show)
    {
        if (sprites != null && sprites.sprites != null && spriteIndex < sprites.sprites.Count)
        {
            GameMassage.ButtonMassageWithText(gameObject, show,
                sprites.sprites[spriteIndex], collectTextHelper, keyMassageOffset, textMassageOffset, textColor: Color.yellow);
        }
    }

    private bool AppliedStateQuest(string questCode, QuestState state = QuestState.Unassigned)
    {
        if (string.IsNullOrEmpty(questCode))
        {
            Debug.LogError(string.IsNullOrEmpty(questCode));
            return false;
        }

        if (state == QuestState.Unassigned)
            return false;

        QuestState currentState = QuestLog.GetQuestState(questCode);

        return currentState == state;
    }
}
