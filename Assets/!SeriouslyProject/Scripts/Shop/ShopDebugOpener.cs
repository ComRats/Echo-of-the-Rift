using UnityEngine;
using EchoRift.Shop;

/// <summary>
/// Отладочный скрипт для открытия магазина по нажатию клавиши B
/// Используется для тестирования системы магазина
/// </summary>
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

    private void ToggleShop()
    {
        if (shopUI == null)
        {
            Debug.LogError("[ShopDebugOpener] ShopUI не найден!");
            return;
        }

        // Если магазин открыт - закрываем
        if (shopUI.IsShopMode)
        {
            shopUI.CloseShop();
            if (showDebugMessages)
            {
                Debug.Log($"[ShopDebugOpener] Магазин закрыт (нажата клавиша {openShopKey})");
            }
        }
        // Если магазин закрыт - открываем
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
