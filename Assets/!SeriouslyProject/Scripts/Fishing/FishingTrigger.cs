using System.Linq;
using EchoRift;
using Sirenix.OdinInspector;
using UnityEngine;
using Zenject;

public class FishingTrigger : MonoBehaviour
{
    [SerializeField] private Fishing fishing;
    [SerializeField] private Vector3 keyMassageOffset;
    [SerializeField, Range(0, 30)] private int spriteIndex;
    [ValueDropdown("GetSpriteNames")]
    [SerializeField] private SpriteCollection sprites;

    [Inject] private MainUI mainUI;
    [Inject] private GameSettings gameSettings;
    
    private Player currentPlayer;
    private bool playerInside = false;
    private bool lastUIState;
    private GameObject buttonUI;

    private void Start()
    {
        sprites = mainUI.spriteCollection;
        lastUIState = mainUI.isOpenUI;

        if (transform.childCount > 0)
            buttonUI = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out currentPlayer))
        {
            playerInside = true;
            if (!mainUI.isOpenUI && fishing != null && fishing.HasFishRemaining)
                ShowButtonPrompt(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            playerInside = false;
            currentPlayer = null;
            ShowButtonPrompt(false);
            
            // Если рыбалка не активна, разблокируем UI
            if (fishing == null || !fishing.IsFishing)
            {
                mainUI.canOpenUI = true;
            }
        }
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
                ShowButtonPrompt(false);
            }
            else if (fishing != null && !fishing.IsFishing && fishing.HasFishRemaining)
            {
                ShowButtonPrompt(true);
            }
            else
            {
                ShowButtonPrompt(false);
            }
        }

        // Блокируем взаимодействие при открытом UI
        if (mainUI.isOpenUI) return;

        if (Input.GetKeyDown(gameSettings.useButton) && fishing != null && !fishing.IsFishing)
        {
            if (!fishing.HasFishRemaining)
            {
                fishing.StartFishingProcess(this);
                return;
            }

            Debug.Log("Рыбалка началась");

            ShowButtonPrompt(false);
            
            fishing.StartFishingProcess(this);
            
            if (buttonUI != null)
                buttonUI.SetActive(false);
        }
    }

    private void ShowButtonPrompt(bool show)
    {
        if (sprites != null && sprites.sprites != null)
        {
            int idx = gameSettings.GetSpriteIndex(gameSettings.useButton);
            if (idx < sprites.sprites.Count)
                GameMassage.ButtonMassage(gameObject, show, sprites.sprites[idx], keyMassageOffset);
        }
    }

    private string[] GetSpriteNames()
    {
        if (sprites == null || sprites.sprites == null)
            return new string[0];

        return sprites.sprites
            .Select((s, i) => $"{i}: {(s != null ? s.name : "<empty>")}")
            .ToArray();
    }

    public void ShowButtonAfterFishing()
    {
        if (playerInside && fishing != null && fishing.HasFishRemaining)
        {
            ShowButtonPrompt(true);
            if (buttonUI != null)
                buttonUI.SetActive(true);
        }
        else
        {
            ShowButtonPrompt(false);
            if (buttonUI != null)
                buttonUI.SetActive(false);
        }
    }
}
