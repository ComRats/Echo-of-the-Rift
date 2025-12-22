using UnityEngine;
using Zenject;

public class PressableButtons : MonoBehaviour
{
    [SerializeField] private KeyCode openInventoryKey = KeyCode.E;
    //[SerializeField] private KeyCode openPauseMenuKey = KeyCode.Escape;

    [Inject] private PlayerUI playerUI;

    private void Start()
    {
        //заменить на загрузку из настроек 
        openInventoryKey = KeyCode.E;
    }

    private void Update()
    {
        OpenPlayerIU();
    }

    private void OpenPlayerIU()
    {
        if (Input.GetKeyDown(openInventoryKey))
        {
            if (playerUI == null)
            {
                Debug.LogError("PlayerUI не был инжектирован через Zenject!");
                return;
            }

            GameObject playerUIbackGround = playerUI.transform.GetChild(0).gameObject;
            playerUIbackGround.SetActive(!playerUIbackGround.activeInHierarchy);
            playerUI.OpenPlayerUI();
        }
    }
}
