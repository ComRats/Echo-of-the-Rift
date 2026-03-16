using UnityEngine;
using TMPro;
using EchoRift.Shop;
using Zenject;

/// <summary>
/// UI панель магазина с двумя инвентарями
/// </summary>
public class ShopUI : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject shopPanel;

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
    [SerializeField] private InventoryContextMenu contextMenu;

    [Inject] private MainUI mainUI;

    private ShopManager shopManager;
    private ShopData currentShopData;
    private bool isShopMode = false;

    public bool IsShopMode => isShopMode;
    public InventorySlot[] MerchantSlots => merchantSlots;
    public InventorySlot[] PlayerSlots => playerSlots;
    public ShopManager ShopManager => shopManager;

    private void Awake()
    {
        // Автопоиск компонентов
        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();

        if (playerWallet == null && inventoryManager != null)
            playerWallet = inventoryManager.Wallet;

        if (contextMenu == null)
            contextMenu = FindObjectOfType<InventoryContextMenu>();

        // Создаём ShopManager
        GameObject shopManagerObj = new GameObject("ShopManager");
        shopManagerObj.transform.SetParent(transform);
        shopManager = shopManagerObj.AddComponent<ShopManager>();

        // Инициализируем ShopManager
        if (inventoryManager != null && playerWallet != null)
        {
            shopManager.Initialize(inventoryManager, playerWallet);
        }
        else
        {
            Debug.LogError("[ShopUI] Не удалось инициализировать ShopManager - отсутствуют InventoryManager или PlayerWallet!");
        }

        if (shopPanel != null)
            shopPanel.SetActive(false);
    }

    private void OnEnable()
    {
        if (playerWallet != null)
            playerWallet.OnCoinsChanged += UpdateCoinsDisplay;
    }

    private void OnDisable()
    {
        if (playerWallet != null)
            playerWallet.OnCoinsChanged -= UpdateCoinsDisplay;
    }

    /// <summary>
    /// Открыть магазин
    /// </summary>
    public void OpenShop(ShopData shopData)
    {
        if (shopData == null || shopManager == null)
        {
            Debug.LogError("[ShopUI] Невозможно открыть магазин - отсутствуют данные!");
            return;
        }

        inventoryManager?.SyncFromUI();

        shopManager.OpenShop(shopData);
        mainUI.ShowCursor();
        currentShopData = shopData;
        isShopMode = true;

        GameTimer.PauseGame();
        if (mainUI != null)
            mainUI.canOpenUI = false;

        if (shopNameText != null)
            shopNameText.text = shopData.shopName;

        if (shopDescriptionText != null)
            shopDescriptionText.text = shopData.shopDescription;

        LoadMerchantInventory();
        SyncPlayerInventory();

        if (playerWallet != null)
            UpdateCoinsDisplay(playerWallet.Coins);

        // Показываем панель
        if (shopPanel != null)
            shopPanel.SetActive(true);
    }

    /// <summary>
    /// Закрыть магазин
    /// </summary>
    public void CloseShop()
    {
        GameTimer.ResumeGame();

        contextMenu?.Hide();

        if (mainUI != null)
            mainUI.canOpenUI = true;

        shopManager?.CloseShop();
        mainUI.HideCursor();

        isShopMode = false;
        currentShopData = null;

        ClearMerchantInventory();

        if (shopPanel != null)
            shopPanel.SetActive(false);

        inventoryManager?.RefreshUIFromData();
        
        // Включаем обратно DraggableItem для всех предметов в основном инвентаре
        EnableDraggableItems();
    }
    
    /// <summary>
    /// Включает DraggableItem компоненты для всех предметов в инвентаре
    /// </summary>
    private void EnableDraggableItems()
    {
        if (inventoryManager == null) return;
        
        // Включаем для слотов инвентаря
        foreach (var slot in inventoryManager.inventorySlots)
        {
            if (slot != null)
            {
                DraggableItem draggable = slot.GetComponentInChildren<DraggableItem>();
                if (draggable != null)
                {
                    draggable.enabled = true;
                }
            }
        }
        
        // Включаем для слотов экипировки
        foreach (var slot in inventoryManager.equipmentSlots)
        {
            if (slot != null)
            {
                DraggableItem draggable = slot.GetComponentInChildren<DraggableItem>();
                if (draggable != null)
                {
                    draggable.enabled = true;
                }
            }
        }
    }

    /// <summary>
    /// Обновить UI после транзакции
    /// </summary>
    public void OnItemTransactionComplete()
    {
        LoadMerchantInventory();
        SyncPlayerInventory();
    }

    private void LoadMerchantInventory()
    {
        if (currentShopData == null || merchantSlots == null || merchantItemPrefab == null)
            return;

        ClearMerchantInventory();

        for (int i = 0; i < merchantSlots.Length && i < currentShopData.items.Count; i++)
        {
            ShopItem shopItem = currentShopData.items[i];
            if (shopItem.item == null)
                continue;

            if (!shopItem.infiniteStock && shopItem.quantity <= 0)
                continue;

            GameObject itemObj = Instantiate(merchantItemPrefab, merchantSlots[i].transform);
            DraggableItem draggableItem = itemObj.GetComponent<DraggableItem>();

            if (draggableItem != null)
            {
                int displayCount = shopItem.infiniteStock ? 999 : shopItem.quantity;
                draggableItem.InitialiseItem(shopItem.item, displayCount);
                draggableItem.enabled = false;
            }
        }
    }

    private void ClearMerchantInventory()
    {
        if (merchantSlots == null)
            return;

        foreach (var slot in merchantSlots)
        {
            if (slot != null && slot.transform.childCount > 0)
                Destroy(slot.transform.GetChild(0).gameObject);
        }
    }

    private void SyncPlayerInventory()
    {
        if (inventoryManager == null || playerSlots == null)
            return;

        // Очищаем слоты
        foreach (var slot in playerSlots)
        {
            if (slot != null && slot.transform.childCount > 0)
                Destroy(slot.transform.GetChild(0).gameObject);
        }

        // Загружаем из данных
        var invSlots = inventoryManager.Data.InventorySlots;

        for (int i = 0; i < playerSlots.Length && i < invSlots.Count; i++)
        {
            if (string.IsNullOrEmpty(invSlots[i].itemName) || invSlots[i].count <= 0)
                continue;

            ItemData itemData = inventoryManager.FindItemDataByName(invSlots[i].itemName);
            if (itemData != null)
            {
                GameObject itemObj = Instantiate(inventoryManager.inventoryItemPrefab, playerSlots[i].transform);
                DraggableItem draggableItem = itemObj.GetComponent<DraggableItem>();
                if (draggableItem != null)
                {
                    draggableItem.InitialiseItem(itemData, invSlots[i].count);
                    draggableItem.enabled = false;
                }
            }
        }
    }

    private void UpdateCoinsDisplay(int coins)
    {
        if (playerCoinsText != null)
            playerCoinsText.text = $"{coins}";
    }

    public bool IsMerchantSlot(InventorySlot slot)
    {
        return System.Array.IndexOf(merchantSlots, slot) >= 0;
    }

    public bool IsPlayerShopSlot(InventorySlot slot)
    {
        return System.Array.IndexOf(playerSlots, slot) >= 0;
    }
}
