using AudioManager.Core;
using AudioManager.Locator;
using AudioManager.Logger;
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
        if (Input.GetKeyDown(gameSettings.openInvenoryKey) && canOpenUI)
        {
            ToggleInventory();
        }
    }

    public void ToggleInventory()
    {
        if (playerUIbackGround == null) return;

        bool isOpen = playerUIbackGround.activeSelf;

        service ??= ServiceLocator.GetService();

        service.PlayOneShot("OpenUI");

        if (isOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
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
    }

    public void Show()
    {
        //Debug.LogWarning("Show IU");

        canvas.enabled = true;
        pauseMenu.enabled = true;
        playerUI.enabled = true;
    }
}
