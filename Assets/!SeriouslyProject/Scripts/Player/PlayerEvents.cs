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

    public void ToggleCursorVisible()
    {
        mainUIinstance.ToggleCursorVisible();
    }
}
