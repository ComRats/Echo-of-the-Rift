using UnityEngine;

public class MainUI : MonoBehaviour
{
    public PauseMenu pauseMenu;
    public PlayerUI playerUI;
    public InventoryTester inventoryTester;
    public Inventory inventory;
    public Canvas canvas;
    public ScreenFader screenFader;
    public GameObject starPanel;
    public SpriteCollection spriteCollection;

    public bool canOpenUI = true;

    public void Hide()
    {
        //Debug.LogWarning("Hide UI");

        canvas.enabled = false;
        pauseMenu.enabled = false;
        playerUI.enabled = false;
        inventoryTester.enabled = false;
    }

    public void Show()
    {
        //Debug.LogWarning("Show IU");

        canvas.enabled = true;
        pauseMenu.enabled = true;
        playerUI.enabled = true;
        inventoryTester.enabled = true;
    }
}
