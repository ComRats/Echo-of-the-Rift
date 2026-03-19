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
        //Debug.LogWarning("ShowCursorEvent");
    }

    public void HideCursor()
    {
        // Не скрываем курсор если открыт магазин
        if (GlobalLoader.Instance?.mainUI?.shopUI != null && GlobalLoader.Instance.mainUI.shopUI.IsShopMode)
        {
            Debug.LogWarning("HideCursorEvent skipped - shop is open");
            return;
        }

        CursorManager.Hide();
        //Debug.LogWarning("HideCursorEvent");
    }
}
