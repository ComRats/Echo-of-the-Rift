using UnityEngine;
using TMPro;
using EchoRift.Shop;

/// <summary>
/// UI панель магазина с двумя инвентарями
/// </summary>
public class ShopUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private GameObject merchantInventoryPanel;
    [SerializeField] private GameObject playerInventoryPanel;

    [Header("Merchant Inventory")]
    [SerializeField] private InventorySlot[] merchantSlots;
    [SerializeField] private GameObject merchantItemPrefab;

    [Header("Player Inventory")]
    [SerializeField] private InventorySlot[] playerSlots;

    [Header("UI Elements")]
    [SerializeField] private TextMeshProUGUI shopNameText;
    [SerializeField] private TextMeshProUGUI shopDescriptionText;
    [SerializeField] private TextMeshProUGUI playerCoinsText;

    [Header("References")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private MainUI mainUI;
    [SerializeField] private InventoryContextMenu contextMenu;

    private ShopData currentShopData;
    private bool isShopMode = false;
    private ShopManager shopManager;
    private GameObject shopManagerObject; // Ссылка на созданный объект для правильного удаления

    public bool IsShopMode => isShopMode;
    public InventorySlot[] MerchantSlots => merchantSlots;
    public InventorySlot[] PlayerSlots => playerSlots;
    public ShopManager ShopManager => shopManager;

    private void Awake()
    {
        Debug.Log("[ShopUI] Awake вызван");
        
        // Проверка обязательных полей
        ValidateSetup();
        
        // Создаём ShopManager только если его нет (предотвращаем дубликаты)
        if (shopManager == null)
        {
            // Проверяем, может быть ShopManager уже существует как дочерний объект
            shopManager = GetComponentInChildren<ShopManager>();
            
            if (shopManager == null)
            {
                shopManagerObject = new GameObject("ShopManager");
                shopManager = shopManagerObject.AddComponent<ShopManager>();
                shopManagerObject.transform.SetParent(transform);
                Debug.Log("[ShopUI] ShopManager создан автоматически");
            }
            else
            {
                shopManagerObject = shopManager.gameObject;
                Debug.Log("[ShopUI] ShopManager найден среди дочерних объектов");
            }
        }
        
        // Автопоиск компонентов если не назначены
        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
            Debug.Log($"[ShopUI] InventoryManager найден автоматически: {inventoryManager != null}");
        }
        
        if (playerWallet == null && inventoryManager != null)
        {
            playerWallet = inventoryManager.Wallet;
            Debug.Log($"[ShopUI] PlayerWallet получен из InventoryManager: {playerWallet != null}");
        }

        if (mainUI == null)
        {
            mainUI = FindObjectOfType<MainUI>();
            Debug.Log($"[ShopUI] MainUI найден автоматически: {mainUI != null}");
        }

        if (contextMenu == null)
        {
            contextMenu = FindObjectOfType<InventoryContextMenu>();
            Debug.Log($"[ShopUI] InventoryContextMenu найден автоматически: {contextMenu != null}");
        }
        
        // Инициализируем ShopManager
        if (shopManager != null && inventoryManager != null && playerWallet != null)
        {
            shopManager.Initialize(inventoryManager, playerWallet);
            Debug.Log("[ShopUI] ShopManager инициализирован");
        }
        
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
            Debug.Log("[ShopUI] shopPanel скрыт при инициализации");
        }
        else
        {
            Debug.LogError("[ShopUI] shopPanel не назначен в инспекторе!");
        }
    }

    private void ValidateSetup()
    {
        bool hasErrors = false;

        if (shopPanel == null)
        {
            Debug.LogError("[ShopUI] ❌ shopPanel не назначен!");
            hasErrors = true;
        }

        if (merchantSlots == null || merchantSlots.Length == 0)
        {
            Debug.LogError("[ShopUI] ❌ merchantSlots не назначен или пуст! Назначьте массив слотов торговца.");
            hasErrors = true;
        }

        if (playerSlots == null || playerSlots.Length == 0)
        {
            Debug.LogError("[ShopUI] ❌ playerSlots не назначен или пуст! Назначьте массив слотов игрока.");
            hasErrors = true;
        }

        if (merchantItemPrefab == null)
        {
            Debug.LogError("[ShopUI] ❌ merchantItemPrefab не назначен! Назначьте префаб предмета.");
            hasErrors = true;
        }

        if (hasErrors)
        {
            Debug.LogError("[ShopUI] ⚠️ КРИТИЧЕСКИЕ ОШИБКИ! Магазин не будет работать без назначения обязательных полей в инспекторе!");
        }
        else
        {
            Debug.Log("[ShopUI] ✅ Все обязательные поля назначены");
        }
    }

    private void OnEnable()
    {
        if (playerWallet != null)
        {
            playerWallet.OnCoinsChanged += UpdateCoinsDisplay;
        }
    }

    private void OnDisable()
    {
        if (playerWallet != null)
        {
            playerWallet.OnCoinsChanged -= UpdateCoinsDisplay;
        }
    }

    private void OnDestroy()
    {
        // Отписываемся от событий при уничтожении
        if (playerWallet != null)
        {
            playerWallet.OnCoinsChanged -= UpdateCoinsDisplay;
        }

        // Очищаем ShopManager если он был создан автоматически
        if (shopManagerObject != null && shopManager != null)
        {
            // Отписываемся от всех событий ShopManager перед уничтожением
            shopManager.OnShopOpened = null;
            shopManager.OnShopClosed = null;
            shopManager.OnItemBought = null;
            shopManager.OnItemSold = null;
            
            Destroy(shopManagerObject);
            shopManager = null;
            shopManagerObject = null;
            Debug.Log("[ShopUI] ShopManager уничтожен при OnDestroy");
        }
    }

    /// <summary>
    /// Открыть магазин с указанными данными
    /// </summary>
    public void OpenShop(ShopData shopData)
    {
        Debug.Log($"[ShopUI] OpenShop вызван для: {(shopData != null ? shopData.shopName : "NULL")}");
        
        if (shopData == null)
        {
            Debug.LogError("[ShopUI] ShopData is null!");
            return;
        }

        if (shopManager == null)
        {
            Debug.LogError("[ShopUI] ShopManager не инициализирован!");
            return;
        }

        // Останавливаем игровое время при открытии магазина
        GameTimer.PauseGame();

        // ВАЖНО: Синхронизируем основной инвентарь перед открытием магазина
        if (inventoryManager != null)
        {
            inventoryManager.SyncFromUI();
            Debug.Log("[ShopUI] Основной инвентарь синхронизирован перед открытием магазина");
        }

        // Открываем магазин через ShopManager
        shopManager.OpenShop(shopData);
        
        currentShopData = shopData;
        isShopMode = true;

        // Блокируем возможность открытия других UI (меню паузы, инвентарь)
        if (mainUI != null)
        {
            mainUI.canOpenUI = false;
            Debug.Log("[ShopUI] UI заблокирован (canOpenUI = false)");
        }

        // Обновляем UI магазина
        if (shopNameText != null)
        {
            shopNameText.text = shopData.shopName;
            Debug.Log($"[ShopUI] Название магазина установлено: {shopData.shopName}");
        }

        if (shopDescriptionText != null)
        {
            shopDescriptionText.text = shopData.shopDescription;
        }

        // Загружаем товары торговца
        Debug.Log($"[ShopUI] Загрузка товаров торговца. Количество товаров: {shopData.items.Count}");
        LoadMerchantInventory(shopData);

        // Синхронизируем инвентарь игрока
        Debug.Log("[ShopUI] Синхронизация инвентаря игрока");
        SyncPlayerInventory();

        // Обновляем отображение монет
        if (playerWallet != null)
        {
            UpdateCoinsDisplay(playerWallet.Coins);
            Debug.Log($"[ShopUI] Монеты обновлены: {playerWallet.Coins}");
        }

        // Показываем панель
        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
            Debug.Log("[ShopUI] shopPanel активирован");
        }
        else
        {
            Debug.LogError("[ShopUI] shopPanel не назначен! Магазин не может быть показан!");
        }

        Debug.Log($"[ShopUI] Магазин открыт: {shopData.shopName}");
    }

    /// <summary>
    /// Закрыть магазин
    /// </summary>
    public void CloseShop()
    {
        Debug.Log("[ShopUI] CloseShop вызван");

        // Возобновляем игровое время при закрытии магазина
        GameTimer.ResumeGame();

        // Закрываем контекстное меню, если оно открыто
        if (contextMenu != null)
        {
            contextMenu.Hide();
            Debug.Log("[ShopUI] Контекстное меню закрыто");
        }

        // Разблокируем UI перед закрытием магазина
        if (mainUI != null)
        {
            mainUI.canOpenUI = true;
            Debug.Log("[ShopUI] UI разблокирован (canOpenUI = true)");
        }
        
        if (shopManager != null)
        {
            shopManager.CloseShop();
        }
        
        isShopMode = false;
        currentShopData = null;

        // Очищаем инвентарь торговца
        ClearMerchantInventory();

        // Скрываем панель
        if (shopPanel != null)
            shopPanel.SetActive(false);

        // Обновляем инвентарь игрока из данных
        if (inventoryManager != null)
            inventoryManager.RefreshUIFromData();

        Debug.Log("[ShopUI] Магазин закрыт");
    }

    /// <summary>
    /// Обновить UI после покупки
    /// </summary>
    public void OnItemTransactionComplete()
    {
        Debug.Log("[ShopUI] Транзакция завершена, обновление UI");
        
        // Обновляем UI торговца
        RefreshMerchantInventory();
        
        // Обновляем UI игрока
        SyncPlayerInventory();
    }

    private void LoadMerchantInventory(ShopData shopData)
    {
        if (merchantSlots == null || merchantSlots.Length == 0)
        {
            Debug.LogError("[ShopUI] merchantSlots не назначен! Назначьте массив слотов торговца в инспекторе.");
            return;
        }

        if (merchantItemPrefab == null)
        {
            Debug.LogError("[ShopUI] merchantItemPrefab не назначен! Назначьте префаб предмета в инспекторе.");
            return;
        }

        ClearMerchantInventory();

        Debug.Log($"[ShopUI] Загрузка {shopData.items.Count} товаров в {merchantSlots.Length} слотов");

        for (int i = 0; i < merchantSlots.Length && i < shopData.items.Count; i++)
        {
            ShopItem shopItem = shopData.items[i];
            if (shopItem.item == null) continue;

            // Показываем только товары в наличии (или с бесконечным запасом)
            if (!shopItem.infiniteStock && shopItem.quantity <= 0)
                continue;

            SpawnMerchantItem(shopItem, merchantSlots[i]);
        }
    }

    private void RefreshMerchantInventory()
    {
        if (currentShopData == null) return;
        LoadMerchantInventory(currentShopData);
    }

    private void SpawnMerchantItem(ShopItem shopItem, InventorySlot slot)
    {
        GameObject itemObj = Instantiate(merchantItemPrefab, slot.transform);
        DraggableItem draggableItem = itemObj.GetComponent<DraggableItem>();

        if (draggableItem != null)
        {
            int displayCount = shopItem.infiniteStock ? 999 : shopItem.quantity;
            draggableItem.InitialiseItem(shopItem.item, displayCount);
            
            // Отключаем перетаскивание для товаров торговца
            draggableItem.enabled = false;
        }
    }

    private void ClearMerchantInventory()
    {
        if (merchantSlots == null || merchantSlots.Length == 0)
        {
            Debug.LogError("[ShopUI] merchantSlots не назначен или пуст!");
            return;
        }

        foreach (var slot in merchantSlots)
        {
            if (slot == null)
            {
                Debug.LogWarning("[ShopUI] Один из merchantSlots равен null!");
                continue;
            }

            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }
    }

    private void SyncPlayerInventory()
    {
        if (inventoryManager == null)
        {
            Debug.LogError("[ShopUI] inventoryManager не назначен!");
            return;
        }

        if (playerSlots == null || playerSlots.Length == 0)
        {
            Debug.LogError("[ShopUI] playerSlots не назначен! Назначьте массив слотов игрока в инспекторе.");
            return;
        }

        Debug.Log("[ShopUI] SyncPlayerInventory начат");

        // Очищаем слоты игрока в магазине
        foreach (var slot in playerSlots)
        {
            if (slot == null)
            {
                Debug.LogWarning("[ShopUI] Один из playerSlots равен null!");
                continue;
            }

            if (slot.transform.childCount > 0)
            {
                Destroy(slot.transform.GetChild(0).gameObject);
            }
        }

        // Загружаем предметы из InventoryData
        var invSlots = inventoryManager.Data.InventorySlots;
        Debug.Log($"[ShopUI] Загрузка {invSlots.Count} слотов из InventoryData");
        
        for (int i = 0; i < playerSlots.Length && i < invSlots.Count; i++)
        {
            if (string.IsNullOrEmpty(invSlots[i].itemName) || invSlots[i].count <= 0)
                continue;

            ItemData itemData = inventoryManager.FindItemDataByName(invSlots[i].itemName);
            if (itemData != null)
            {
                Debug.Log($"[ShopUI] Добавление предмета в слот {i}: {invSlots[i].itemName} x{invSlots[i].count}");
                GameObject itemObj = Instantiate(inventoryManager.inventoryItemPrefab, playerSlots[i].transform);
                DraggableItem draggableItem = itemObj.GetComponent<DraggableItem>();
                if (draggableItem != null)
                {
                    draggableItem.InitialiseItem(itemData, invSlots[i].count);
                    // Отключаем перетаскивание в режиме магазина
                    draggableItem.enabled = false;
                }
            }
            else
            {
                Debug.LogWarning($"[ShopUI] Не найден ItemData для: {invSlots[i].itemName}");
            }
        }
        
        Debug.Log("[ShopUI] SyncPlayerInventory завершён");
    }

    private void UpdateCoinsDisplay(int coins)
    {
        if (playerCoinsText != null)
        {
            playerCoinsText.text = $"{coins}";
        }
    }



    /// <summary>
    /// Проверяет, является ли слот слотом торговца
    /// </summary>
    public bool IsMerchantSlot(InventorySlot slot)
    {
        return System.Array.IndexOf(merchantSlots, slot) >= 0;
    }

    /// <summary>
    /// Проверяет, является ли слот слотом игрока в магазине
    /// </summary>
    public bool IsPlayerShopSlot(InventorySlot slot)
    {
        return System.Array.IndexOf(playerSlots, slot) >= 0;
    }
}
