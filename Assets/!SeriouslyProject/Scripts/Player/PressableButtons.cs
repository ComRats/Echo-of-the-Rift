using UnityEngine;

public class ClickbleButtons : MonoBehaviour
{
    [SerializeField] private PlayerUI playerUI;
    [SerializeField] private KeyCode openInvenoryKey = KeyCode.E;

    private void Start()
    {
        playerUI = FindObjectOfType<PlayerUI>();
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
            GameObject playerUIbackGround = playerUI.transform.GetChild(0).gameObject;
            playerUIbackGround.SetActive(!playerUIbackGround.activeInHierarchy);
            playerUI.OpenPlayerUI();
        }
    }
}
