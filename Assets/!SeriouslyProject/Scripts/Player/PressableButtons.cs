using UnityEngine;
using Zenject;

public class PressableButtons : MonoBehaviour
{
    [Inject] private PlayerUI playerUI;
    [Inject] private MainUI mainUI;
    [Inject] private GameSettings settings;

    private void Update()
    {
        OpenPlayerIU();
    }

    private void OpenPlayerIU()
    {
        if (Input.GetKeyDown(settings.openInvenoryKey) && mainUI.canOpenUI)
        {
            if (playerUI == null)
            {
                Debug.LogError("PlayerUI не был инжектирован через Zenject!");
                return;
            }

            GameObject playerUIbackGround = playerUI.transform.GetChild(0).gameObject;
            playerUIbackGround.SetActive(!playerUIbackGround.activeInHierarchy);
            //playerUI.OpenPlayerUI();
            mainUI.isOpenUI = !mainUI.isOpenUI;
            Debug.LogWarning("mainUI.isOpenUI " + mainUI.isOpenUI);
        }
    }
}
