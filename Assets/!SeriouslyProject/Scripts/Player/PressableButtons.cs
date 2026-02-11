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
        // Блокируем открытие инвентаря во время паузы
        if (GameTimer.IsPaused)
        {
            return;
        }

        if (Input.GetKeyDown(settings.openInvenoryKey) && mainUI.canOpenUI)
        {
            if (playerUI == null)
            {
                Debug.LogError("PlayerUI �� ��� ������������ ����� Zenject!");
                return;
            }

            GameObject playerUIbackGround = playerUI.transform.GetChild(0).gameObject;
            
            // Открываем инвентарь только если он закрыт
            if (!playerUIbackGround.activeInHierarchy)
            {
                playerUIbackGround.SetActive(true);
                mainUI.isOpenUI = true;
                
                // Останавливаем время при открытии инвентаря
                GameTimer.PauseGame();
                
                Debug.LogWarning("mainUI.isOpenUI " + mainUI.isOpenUI);
            }
        }
    }
}
