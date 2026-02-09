using UnityEngine;
using System;

public class ShopManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private ShopData shopData;
    [SerializeField] private InventoryData playerInventoryData;

    [Header("References")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private ShopContextMenu shopContextMenu;
    [SerializeField] private ItemDescriptionDisplay descriptionDisplay;

    [Header("Shop UI")]
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private ShopSlot[] shopSlots;
    [SerializeField] private GameObject shopItemPrefab;

    [Header("Player Inventory UI in Shop")]
    [SerializeField] private PlayerShopSlot[] playerShopSlots;
    [SerializeField] private GameObject playerShopItemPrefab;

    public ShopData ShopData => shopData;
    public InventoryData PlayerInventoryData => playerInventoryData;
    public bool IsOpen { get; private set; }

    public event Action OnShopOpened;
    public event Action OnShopClosed;
    public event Action<ItemData> OnItemBought;
    public event Action<ItemData> OnItemSold;

    private void Awake()
    {
        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }
    }

    private void Start()
    {
        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();

        if (playerWallet == null && inventoryManager != null)
            playerWallet = inventoryManager.Wallet;

        if (playerInventoryData == null && inventoryManager != null)
            playerInventoryData = inventoryManager.Data;
    }

    #region Shop Open/Close

    public void OpenShop()
    {
        if (IsOpen) return;

        // �������������� ������ ��������� ����� ���������
        if (inventoryManager != null)
        {
            inventoryManager.SyncFromUI();
        }

        IsOpen = true;

        if (shopPanel != null)
        {
            shopPanel.SetActive(true);
        }

        RefreshShopItems();
        RefreshPlayerItems();

        OnShopOpened?.Invoke();
        Debug.Log($"������� '{shopData.shopName}' ������");
    }

    public void CloseShop()
    {
        if (!IsOpen) return;

        IsOpen = false;

        if (shopPanel != null)
        {
            shopPanel.SetActive(false);
        }

        if (shopContextMenu != null)
        {
            shopContextMenu.Hide();
        }

        // ��������� UI ��������� ���������
        if (inventoryManager != null)
        {
            inventoryManager.RefreshUIFromData();
        }

        HideItemDescription();
        OnShopClosed?.Invoke();

        Debug.Log("������� ������");
    }

    public void ToggleShop()
    {
        if (IsOpen)
            CloseShop();
        else
            OpenShop();
    }

    #endregion

    #region Buy/Sell

    public bool BuyItem(ShopItem shopItem)
    {
        if (shopItem == null || shopItem.ItemData == null)
        {
            Debug.LogWarning("��� �������� ��� �������");
            return false;
        }

        ItemData item = shopItem.ItemData;
        int price = shopData.GetBuyPrice(item);

        // ��������� ������
        if (!playerWallet.HasEnoughCoins(price))
        {
            Debug.Log($"������������ ����� ��� ������� {item.itemGameName}. �����: {price}");
            return false;
        }

        // ��������� ������� � ��������
        if (!shopItem.ShopEntry.HasStock)
        {
            Debug.Log($"{item.itemGameName} ��� � �������");
            return false;
        }

        // ������� �������� � InventoryData
        if (!TryAddToPlayerInventory(item, 1))
        {
            Debug.Log("��������� �����!");
            return false;
        }

        // ��������� ������ � ��������� �����
        playerWallet.TrySpendCoins(price);
        shopItem.ShopEntry.TryDecreaseStock(1);
        shopItem.UpdateVisuals();

        // ��������� UI ��������� ������ � ��������
        RefreshPlayerItems();

        OnItemBought?.Invoke(item);
        Debug.Log($"������ {item.itemGameName} �� {price} �����");

        return true;
    }

    public bool SellItem(PlayerShopItem playerItem)
    {
        if (playerItem == null || playerItem.ItemData == null)
        {
            Debug.LogWarning("��� �������� ��� �������");
            return false;
        }

        ItemData item = playerItem.ItemData;
        int sellPrice = shopData.GetSellPrice(item);
        int slotIndex = playerItem.SlotIndex;

        // ������� �� InventoryData
        if (!TryRemoveFromPlayerInventory(slotIndex, 1))
        {
            Debug.LogWarning("�� ������� ������� ������� �� ���������");
            return false;
        }

        // ��������� ������
        playerWallet.AddCoins(sellPrice);

        // ��������� UI
        RefreshPlayerItems();

        OnItemSold?.Invoke(item);
        Debug.Log($"������ {item.itemGameName} �� {sellPrice} �����");

        return true;
    }

    public bool SellItemFromInventory(DraggableItem draggableItem)
    {
        if (draggableItem == null || draggableItem.itemData == null)
        {
            Debug.LogWarning("��� �������� ��� �������");
            return false;
        }

        ItemData item = draggableItem.itemData;
        int sellPrice = shopData.GetSellPrice(item);

        // ������� ������� �� UI ���������
        if (inventoryManager != null)
        {
            // ������� ��������������� � InventoryData
            inventoryManager.SyncFromUI();

            // ������� ������� �� DraggableItem
            if (draggableItem.count > 1)
            {
                draggableItem.count--;
                draggableItem.RefreshCount();
            }
            else
            {
                Destroy(draggableItem.gameObject);
            }

            // ��������� ������
            playerWallet.AddCoins(sellPrice);

            // ��������� InventoryData �� UI
            inventoryManager.SyncFromUI();

            // ��������� UI �������� ������ � ��������
            RefreshPlayerItems();

            OnItemSold?.Invoke(item);
            Debug.Log($"������ {item.itemGameName} �� {sellPrice} �����");

            return true;
        }

        return false;
    }

    #endregion

    #region Inventory Data Operations

    private bool TryAddToPlayerInventory(ItemData item, int amount)
    {
        if (playerInventoryData == null) return false;

        int remaining = amount;

        // ������� �������� �������� � ������������ �����
        if (item.isStackable)
        {
            var slots = playerInventoryData.InventorySlots;
            for (int i = 0; i < slots.Count && remaining > 0; i++)
            {
                if (slots[i].itemName == item.itemName && slots[i].count < item.maxStackSize)
                {
                    int space = item.maxStackSize - slots[i].count;
                    int toAdd = Mathf.Min(remaining, space);

                    playerInventoryData.SetInventorySlot(i, item.itemName, slots[i].count + toAdd);
                    remaining -= toAdd;
                }
            }
        }

        // ����� � ������ �����
        while (remaining > 0)
        {
            int emptyIndex = playerInventoryData.FindEmptyInventorySlot();
            if (emptyIndex < 0)
            {
                return remaining < amount; // �������� ��������
            }

            int toAdd = item.isStackable ? Mathf.Min(remaining, item.maxStackSize) : 1;
            playerInventoryData.SetInventorySlot(emptyIndex, item.itemName, toAdd);
            remaining -= toAdd;
        }

        return true;
    }

    private bool TryRemoveFromPlayerInventory(int slotIndex, int amount)
    {
        if (playerInventoryData == null) return false;

        var slots = playerInventoryData.InventorySlots;
        if (slotIndex < 0 || slotIndex >= slots.Count) return false;

        var slot = slots[slotIndex];
        if (string.IsNullOrEmpty(slot.itemName) || slot.count < amount) return false;

        int newCount = slot.count - amount;

        if (newCount <= 0)
        {
            playerInventoryData.ClearInventorySlot(slotIndex);
        }
        else
        {
            playerInventoryData.SetInventorySlot(slotIndex, slot.itemName, newCount);
        }

        return true;
    }

    private ItemData FindItemDataByName(string itemName)
    {
        if (inventoryManager != null)
        {
            return inventoryManager.FindItemDataByName(itemName);
        }

        // Fallback - ���� � Resources
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        foreach (ItemData item in allItems)
        {
            if (item != null && item.itemName == itemName)
                return item;
        }
        return null;
    }

    #endregion

    #region UI Refresh

    public void RefreshShopItems()
    {
        if (shopData == null || shopSlots == null) return;

        foreach (ShopSlot slot in shopSlots)
        {
            slot.Clear();
        }

        int slotIndex = 0;
        foreach (ShopItemEntry entry in shopData.items)
        {
            if (slotIndex >= shopSlots.Length) break;
            if (entry.item == null) continue;

            CreateShopItem(entry, shopSlots[slotIndex]);
            slotIndex++;
        }
    }

    public void RefreshPlayerItems()
    {
        if (playerInventoryData == null || playerShopSlots == null) return;

        // ������� ������ ��������
        foreach (PlayerShopSlot slot in playerShopSlots)
        {
            slot.Clear();
        }

        // ������ �������� �� InventoryData
        var inventorySlots = playerInventoryData.InventorySlots;
        int uiSlotIndex = 0;

        for (int i = 0; i < inventorySlots.Count && uiSlotIndex < playerShopSlots.Length; i++)
        {
            var slotData = inventorySlots[i];

            if (string.IsNullOrEmpty(slotData.itemName) || slotData.count <= 0)
            {
                uiSlotIndex++;
                continue;
            }

            ItemData itemData = FindItemDataByName(slotData.itemName);
            if (itemData != null)
            {
                CreatePlayerShopItem(itemData, i, slotData.count, playerShopSlots[uiSlotIndex]);
            }

            uiSlotIndex++;
        }
    }

    private void CreateShopItem(ShopItemEntry entry, ShopSlot slot)
    {
        if (shopItemPrefab == null)
        {
            Debug.LogError("Shop Item Prefab �� ��������!");
            return;
        }

        GameObject itemObj = Instantiate(shopItemPrefab);
        ShopItem shopItem = itemObj.GetComponent<ShopItem>();

        if (shopItem != null)
        {
            shopItem.Initialize(entry, this, shopContextMenu);
            slot.SetItem(shopItem);
        }
    }

    private void CreatePlayerShopItem(ItemData itemData, int dataIndex, int count, PlayerShopSlot slot)
    {
        if (playerShopItemPrefab == null)
        {
            Debug.LogError("Player Shop Item Prefab �� ��������!");
            return;
        }

        GameObject itemObj = Instantiate(playerShopItemPrefab);
        PlayerShopItem playerItem = itemObj.GetComponent<PlayerShopItem>();

        if (playerItem != null)
        {
            playerItem.Initialize(itemData, dataIndex, count, this, shopContextMenu);
            slot.SetItem(playerItem);
        }
    }

    #endregion

    #region Description

    public void ShowItemDescription(ItemData item, bool isBuying)
    {
        if (descriptionDisplay == null || item == null) return;

        descriptionDisplay.ShowShopItem(item);
    }

    public void ShowPlayerItemDescription(ItemData item)
    {
        if (descriptionDisplay == null || item == null) return;

        // ���������� � ����� �������
        int sellPrice = shopData.GetSellPrice(item);
        ShowCustomDescription(item, sellPrice, "���� �������");
    }

    private void ShowCustomDescription(ItemData item, int price, string priceLabel)
    {
        if (descriptionDisplay == null) return;

        // ���������� ������������ ����� ��� ������ ���� �����
        descriptionDisplay.ShowShopItemWithCustomPrice(item, price, priceLabel);
    }

    public void HideItemDescription()
    {
        if (descriptionDisplay != null)
        {
            descriptionDisplay.Hide();
        }
    }

    #endregion
}