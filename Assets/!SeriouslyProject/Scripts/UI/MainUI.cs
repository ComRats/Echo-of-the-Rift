using AudioManager.Core;
using AudioManager.Locator;
using PixelCrushers.DialogueSystem;
using Sirenix.OdinInspector;
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
            playerUIbackGround.SetActive(false);
        }

        service ??= ServiceLocator.GetService();

        contextMenu = FindObjectOfType<InventoryContextMenu>();
        musicManager = FindObjectOfType<MusicTransitionManager>();

        if (playerUI != null)
        {
            playerUI.onQuestTongueSelected = OnQuestTongueSelectedFromTab;
            playerUI.onQuestTongueDeselected = OnQuestTongueDeselectedFromTab;
            playerUI.onGuideTongueSelected = () => playerUI.mobGuide.UpdateMobsGrid();
        }
    }

    private void OnQuestTongueSelectedFromTab()
    {
        if (questLogWindow == null) return;
        if (questLogWindow.mainPanel != null && !questLogWindow.mainPanel.gameObject.activeInHierarchy)
            questLogWindow.mainPanel.panelState = PixelCrushers.UIPanel.PanelState.Closed;
        if (!questLogWindow.isOpen)
            questLogWindow.Open();
    }

    private void OnQuestTongueDeselectedFromTab()
    {
        if (questLogWindow == null) return;
        if (questLogWindow.isOpen)
            questLogWindow.Close();
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

            if (pauseMenu != null && pauseMenu.isActive)
                return;

            ToggleQuestLog();
        }
    }
    [Button]
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
        CursorManager.Show();
    }

    public void HideCursor()
    {
        isCursorVisible = false;
        CursorManager.Hide();
    }

    public void ToggleQuestLog()
    {
        if (playerUIbackGround == null) return;

        service ??= ServiceLocator.GetService();

        bool isOpen = questLogWindow != null && questLogWindow.isOpen;

        if (isOpen)
        {
            CloseQuestLog();
            service = ServiceLocator.GetService();
            service.PlayOneShot("OpenUI_R"); 
        }
        else
        {
            OpenQuestLog();
            service = ServiceLocator.GetService();
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

        if (questLogWindow != null && !questLogWindow.isOpen)
            questLogWindow.Open();

        GameTimer.PauseGame();

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
        if (questLogWindow != null && questLogWindow.isOpen)
            questLogWindow.Close();
        GameTimer.ResumeGame();

        if (contextMenu != null)
        {
            contextMenu.Hide();
        }

        if (musicManager != null)
        {
            musicManager.RestoreMusic();
        }
    }

    public void OpenInventory()
    {
        if (playerUIbackGround == null || !canOpenUI) return;
        
        if (pauseMenu != null && pauseMenu.isActive) return;

        bool inBattle = FindObjectOfType<FightManager>() != null;

        if (!inBattle) ShowCursor();
        service = ServiceLocator.GetService();
        service.PlayOneShot("OpenUI");
        playerUIbackGround.SetActive(true);
        isOpenUI = true;
        playerUI.OpenPlayerUI();
        
        UpdateCharacterSlots();
        
        if (!inBattle) GameTimer.PauseGame();

        if (musicManager != null)
        {
            musicManager.DuckMusic();
        }
    }

    private void UpdateCharacterSlots()
    {
        if (debugMode)
            Debug.Log("[MainUI] UpdateCharacterSlots called");
        
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

        bool inBattle = FindObjectOfType<FightManager>() != null;

        if (!inBattle) HideCursor();

        service = ServiceLocator.GetService();
        service.PlayOneShot("OpenUI_R");
        playerUIbackGround.SetActive(false);
        isOpenUI = false;
        if (!inBattle) GameTimer.ResumeGame();

        if (questLogWindow != null && questLogWindow.isOpen)
            questLogWindow.Close();

        if (contextMenu != null)
        {
            contextMenu.Hide();
        }

        if (musicManager != null)
        {
            musicManager.RestoreMusic();
        }
    }

    public void Hide()
    {
        canvas.enabled = false;
        pauseMenu.enabled = false;
        playerUI.enabled = false;
        tonguesCanvas.enabled = false;
    }

    public void Show()
    {
        canvas.enabled = true;
        pauseMenu.enabled = true;
        playerUI.enabled = true;
        tonguesCanvas.enabled = true;
    }

    public void ResetUIState()
    {
        if (playerUIbackGround != null)
            playerUIbackGround.SetActive(false);

        isOpenUI = false;
        canOpenUI = true;
    }
}
