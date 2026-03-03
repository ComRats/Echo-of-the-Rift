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

    public bool canOpenUI = true;
    public bool isOpenUI = false;

    [Inject] private GameSettings gameSettings;

    private GameObject playerUIbackGround;
    private InventoryContextMenu contextMenu;
    private IAudioManager service;

    private void Awake()
    {
        if (playerUI != null && playerUI.transform.childCount > 0)
        {
            playerUIbackGround = playerUI.transform.GetChild(0).gameObject;
        }

        contextMenu = FindObjectOfType<InventoryContextMenu>();
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

        service ??= ServiceLocator.GetService();


        if (isOpen)
        {
            CloseInventory();
            service.PlayOneShot("OpenUI_R");
        }
        else
        {
            OpenInventory();
            service.PlayOneShot("OpenUI");
        }
    }

    public void OpenQuestLog()
    {
        if (playerUIbackGround == null || !canOpenUI) return;

        playerUIbackGround.SetActive(true);
        isOpenUI = true;
        playerUI.OpenPlayerUI(3);
        questLogWindow.Open();
        GameTimer.PauseGame();
    }

    public void CloseQuestLog()
    {
        if (playerUIbackGround == null) return;

        playerUIbackGround.SetActive(false);
        isOpenUI = false;
        questLogWindow.Close();
        GameTimer.ResumeGame();

        if (contextMenu != null)
        {
            contextMenu.Hide();
        }
    }

    public void OpenInventory()
    {
        if (playerUIbackGround == null || !canOpenUI) return;

        playerUIbackGround.SetActive(true);
        isOpenUI = true;
        playerUI.OpenPlayerUI();
        GameTimer.PauseGame();
    }

    public void CloseInventory()
    {
        if (playerUIbackGround == null) return;

        playerUIbackGround.SetActive(false);
        isOpenUI = false;
        GameTimer.ResumeGame();

        if (contextMenu != null)
        {
            contextMenu.Hide();
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
