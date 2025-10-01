using UnityEngine;

public class ClickbleButtons : MonoBehaviour
{
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private KeyCode openInvenoryKey = KeyCode.E;

    private void Start()
    {
        //заменить на загрузку из настроек 
        openInvenoryKey = KeyCode.E;
    }

    private void Update()
    {
        OpenPlayerIU();
    }

    private void OpenPlayerIU()
    {
        if (Input.GetKeyDown(openInvenoryKey))
        {
            if (playerUI == null)
                playerUI = FindObjectOfType<PlayerUI>();

            GameObject playerUIbackGround = playerUI.transform.GetChild(0).gameObject;
            playerUIbackGround.SetActive(!playerUIbackGround.activeInHierarchy);
            playerUI.OpenPlayerUI();
        }
    }
}
