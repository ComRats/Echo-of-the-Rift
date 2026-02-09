using UnityEngine;

public class ShopTestButtons : MonoBehaviour
{
    [SerializeField] private ShopManager shopManager;
    [SerializeField] private KeyCode toggleKey = KeyCode.B;

    private void Update()
    {
        if (Input.GetKeyDown(toggleKey))
        {
            if (shopManager != null)
            {
                shopManager.ToggleShop();
            }
        }
    }
}