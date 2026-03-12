using UnityEngine;

public class PlayerEvents : MonoBehaviour
{
    public void CloseUI()
    {
        GlobalLoader.Instance.mainUI.CloseInventory();
        GlobalLoader.Instance.mainUI.pauseMenu.ClosePauseMenu();
    }
}
