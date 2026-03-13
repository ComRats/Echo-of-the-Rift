using AudioManager.Core;
using AudioManager.Locator;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using Zenject;

public class MainUI : MonoBehaviour
{
    public PauseMenu pauseMenu;
    public PlayerUI playerUI;
    public Canvas canvas;
    public ScreenFader screenFader;
    public GameObject starPanel;
    public SpriteCollection spriteCollection;
    public InventoryManager inventoryManager;
    public FishingUI fishingUI;
    public ShopUI shopUI;
    public Canvas tonguesCanvas;
    public StandardUIQuestLogWindow questLogWindow;
    public TeamManager teamManager;

    public bool canOpenUI = true;
    public bool isOpenUI = false;
    public bool isCursorVisible = true;

    [SerializeField] private bool debugMode = false;
    [Inject] private GameSettings gameSettings;

    private GameObject playerUIbackGround;
    private InventoryContextMenu contextMenu;
    private IAudioManager service;
    private MusicTransitionManager musicManager;

    private void Awake()
    {
        if (playerUI != null && playerUI.transform.childCount > 0)
        {
            playerUIbackGround = playerUI.transform.GetChild(0).gameObject;
        }

        service ??= ServiceLocator.GetService();

        contextMenu = FindObjectOfType<InventoryContextMenu>();
        musicManager = FindObjectOfType<MusicTransitionManager>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(gameSettings.openInvenoryKey))
        {
            if (shopUI != null && shopUI.IsShopMode)
            {
                shopUI.CloseShop();
                return;
            }

            if (canOpenUI)
            {
                ToggleInventory();
            }
        }

        if (Input.GetKeyDown(gameSettings.questWindowKey) && canOpenUI)
        {
            if (shopUI != null && shopUI.IsShopMode)
                return;

            ToggleQuestLog();
        }
    }

    public void ToggleCursorVisible()
    {
        if (!isCursorVisible)
        {
            ShowCursor();
        }
        else HideCursor();
    }

    public void ShowCursor()
    {
        isCursorVisible = true;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideCursor()
    {
        isCursorVisible = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void ToggleQuestLog()
    {
        if (playerUIbackGround == null) return;

        bool isOpen = playerUIbackGround.activeSelf;

        service ??= ServiceLocator.GetService();

        if (isOpen)
        {
            CloseQuestLog();
            service.PlayOneShot("OpenUI_R"); 
        }
        else
        {
            OpenQuestLog();
            service.PlayOneShot("OpenUI");
        }
    }

    public void ToggleInventory()
    {
        if (playerUIbackGround == null) return;

        bool isOpen = playerUIbackGround.activeSelf;

        if (isOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }

    public void OpenQuestLog()
    {
        if (playerUIbackGround == null || !canOpenUI) return;

        ShowCursor();

        playerUIbackGround.SetActive(true);
        isOpenUI = true;
        playerUI.OpenPlayerUI(3);
        questLogWindow.Open();
        GameTimer.PauseGame();

        // Приглушаем музыку
        if (musicManager != null)
        {
            musicManager.DuckMusic();
        }
    }

    public void CloseQuestLog()
    {
        if (playerUIbackGround == null) return;

        HideCursor();

        playerUIbackGround.SetActive(false);
        isOpenUI = false;
        questLogWindow.Close();
        GameTimer.ResumeGame();

        if (contextMenu != null)
        {
            contextMenu.Hide();
        }

        // Восстанавливаем громкость музыки
        if (musicManager != null)
        {
            musicManager.RestoreMusic();
        }
    }

    public void OpenInventory()
    {
        if (playerUIbackGround == null || !canOpenUI) return;
        
        // Если открыто меню паузы, не открываем инвентарь
        if (pauseMenu != null && pauseMenu.isActive) return;

        ShowCursor();
        service.PlayOneShot("OpenUI");
        playerUIbackGround.SetActive(true);
        isOpenUI = true;
        playerUI.OpenPlayerUI();
        
        // Обновляем слоты персонажей при открытии инвентаря
        UpdateCharacterSlots();
        
        GameTimer.PauseGame();

        // Приглушаем музыку
        if (musicManager != null)
        {
            musicManager.DuckMusic();
        }
    }

    private void UpdateCharacterSlots()
    {
        if (debugMode)
            Debug.Log("[MainUI] UpdateCharacterSlots called");
        
        // Используем существующий TeamManager для обновления слотов
        if (teamManager != null)
        {
            if (debugMode)
                Debug.Log("[MainUI] Calling teamManager.UpdateTeamUI()");
            teamManager.UpdateTeamUI();
        }
        else
        {
            Debug.LogWarning("[MainUI] TeamManager is null!");
        }
    }

    public void CloseInventory()
    {
        if (playerUIbackGround == null) return;

        HideCursor();

        service.PlayOneShot("OpenUI_R");
        playerUIbackGround.SetActive(false);
        isOpenUI = false;
        GameTimer.ResumeGame();

        if (contextMenu != null)
        {
            contextMenu.Hide();
        }

        // Восстанавливаем громкость музыки
        if (musicManager != null)
        {
            musicManager.RestoreMusic();
        }
    }

    public void Hide()
    {
        //Debug.LogWarning("Hide UI");

        canvas.enabled = false;
        pauseMenu.enabled = false;
        playerUI.enabled = false;
        tonguesCanvas.enabled = false;
    }

    public void Show()
    {
        //Debug.LogWarning("Show IU");

        canvas.enabled = true;
        pauseMenu.enabled = true;
        playerUI.enabled = true;
        tonguesCanvas.enabled = true;
    }
}
