using UnityEngine;
using Zenject;

public class PlayerEvents : MonoBehaviour
{
    [Inject] private MainUI mainUIinstance;

    public void CloseUI()
    {
        GlobalLoader.Instance.mainUI.CloseInventory();
        GlobalLoader.Instance.mainUI.pauseMenu.ClosePauseMenu();
    }

    public void ShowCursor()
    {
        CursorManager.Show();
    }

    public void HideCursor()
    {
        if (GlobalLoader.Instance?.mainUI?.shopUI != null && GlobalLoader.Instance.mainUI.shopUI.IsShopMode)
        {
            return;
        }

        CursorManager.Hide();
    }
}
