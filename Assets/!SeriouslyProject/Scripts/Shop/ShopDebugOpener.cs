using UnityEngine;
using EchoRift.Shop;

public class ShopDebugOpener : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private KeyCode openShopKey = KeyCode.B;
    
    [Header("References")]
    [SerializeField] private ShopUI shopUI;
    [SerializeField] private ShopData testShopData;

    [Header("Info")]
    [SerializeField] private bool showDebugMessages = true;

    private void Awake()
    {
        if (shopUI == null)
        {
            shopUI = FindObjectOfType<ShopUI>();
            if (shopUI != null)
            {
                Debug.Log("[ShopDebugOpener] ShopUI найден автоматически");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(openShopKey))
        {
            ToggleShop();
        }
    }

    public void ToggleShop()
    {
        if (shopUI == null)
        {
            Debug.LogError("[ShopDebugOpener] ShopUI не найден!");
            return;
        }

        if (shopUI.IsShopMode)
        {
            shopUI.CloseShop();
            if (showDebugMessages)
            {
                Debug.Log($"[ShopDebugOpener] Магазин закрыт (нажата клавиша {openShopKey})");
            }
        }
        else
        {
            if (testShopData == null)
            {
                Debug.LogError("[ShopDebugOpener] Не назначен TestShopData! Создайте ShopData и назначьте его в инспекторе.");
                return;
            }

            shopUI.OpenShop(testShopData);
            if (showDebugMessages)
            {
                Debug.Log($"[ShopDebugOpener] Магазин открыт (нажата клавиша {openShopKey}): {testShopData.shopName}");
            }
        }
    }

    private void OnValidate()
    {
        if (testShopData == null)
        {
            Debug.LogWarning("[ShopDebugOpener] Не назначен TestShopData! Создайте ShopData (ПКМ → Create → Shop → Shop Data) и назначьте его.");
        }
    }
}
