using UnityEngine;
using System;
using EchoRift.SaveLoadSystem;
using static EchoRift.SaveLoadSystem.SaveFileNames;

[Serializable]
public class InventorySaver
{
    public InventorySlotData[] inventorySlots;
    public InventorySlotData[] equipmentSlots;
    public int coins;

    public InventorySaver() { }

    public InventorySaver(int inventoryCount, int equipmentCount)
    {
        inventorySlots = new InventorySlotData[inventoryCount];
        equipmentSlots = new InventorySlotData[equipmentCount];
        coins = 0;

        for (int i = 0; i < inventoryCount; i++)
            inventorySlots[i] = new InventorySlotData();

        for (int i = 0; i < equipmentCount; i++)
            equipmentSlots[i] = new InventorySlotData();
    }
}

[Serializable]
public class InventorySlotData
{
    public string itemName;
    public int count;

    public InventorySlotData()
    {
        itemName = "";
        count = 0;
    }

    public InventorySlotData(string name, int itemCount)
    {
        itemName = name;
        count = itemCount;
    }
}

public class InventoryManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private InventoryData inventoryData;

    [Header("Inventory UI")]
    public InventorySlot[] inventorySlots;

    [Header("Equipment UI")]
    public InventorySlot[] equipmentSlots;

    [Header("Wallet UI")]
    [SerializeField] private PlayerWallet playerWallet;

    [Header("Prefab")]
    public GameObject inventoryItemPrefab;

    public InventoryData Data => inventoryData;
    public PlayerWallet Wallet => playerWallet;

    private void Start()
    {
        InitializeData();
        LoadInventory();
    }

    private void InitializeData()
    {
        if (inventoryData == null)
        {
            Debug.LogError("InventoryData не назначен!");
            return;
        }

        inventoryData.Initialize(inventorySlots.Length, equipmentSlots.Length);
    }

    #region Public API

    public bool AddItem(string itemName, int amount = 1)
    {
        ItemData item = FindItemDataByName(itemName);
        if (item == null)
        {
            Debug.LogWarning($"Предмет '{itemName}' не найден в Resources/Items!");
            return false;
        }

        return AddItemInternal(item, amount);
    }

    public bool RemoveItem(string itemName, int amount = 1)
    {
        if (amount <= 0) return false;

        int remaining = amount;

        remaining = RemoveFromSlots(inventorySlots, itemName, remaining, true);
        if (remaining <= 0) return true;

        remaining = RemoveFromSlots(equipmentSlots, itemName, remaining, false);

        if (remaining > 0)
        {
            Debug.LogWarning($"Недостаточно '{itemName}'. Не хватает: {remaining}");
            return false;
        }

        return true;
    }

    public int GetItemCount(string itemName)
    {
        return inventoryData.GetItemCount(itemName);
    }

    public int FindItem(string itemName)
    {
        return inventoryData.FindItem(itemName);
    }

    public bool HasItem(string itemName)
    {
        return inventoryData.HasItem(itemName);
    }

    public bool HasItem(string itemName, int requiredAmount)
    {
        return inventoryData.GetItemCount(itemName) >= requiredAmount;
    }

    /// <summary>
    /// Проверяет, полностью ли заполнен инвентарь (нет пустых слотов)
    /// </summary>
    public bool IsInventoryFull()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.transform.childCount == 0)
                return false;
        }
        return true;
    }

    /// <summary>
    /// Проверяет, можно ли добавить указанное количество предмета
    /// </summary>
    public bool CanAddItem(string itemName, int amount = 1)
    {
        ItemData item = FindItemDataByName(itemName);
        if (item == null)
            return false;

        return HasSpaceForItem(item, amount);
    }

    /// <summary>
    /// Проверяет, есть ли место для добавления предмета
    /// </summary>
    public bool HasSpaceForItem(ItemData item, int amount)
    {
        if (item == null || amount <= 0)
            return false;

        int remaining = amount;

        // Проверяем существующие стаки
        if (item.isStackable)
        {
            foreach (var slot in inventorySlots)
            {
                var slotItem = slot.GetComponentInChildren<DraggableItem>();
                if (slotItem != null && slotItem.itemData == item && slotItem.count < item.maxStackSize)
                {
                    int space = item.maxStackSize - slotItem.count;
                    remaining -= space;
                    if (remaining <= 0)
                        return true;
                }
            }
        }

        // Проверяем пустые слоты
        int emptySlots = 0;
        foreach (var slot in inventorySlots)
        {
            if (slot.transform.childCount == 0)
                emptySlots++;
        }

        if (item.isStackable)
        {
            int slotsNeeded = Mathf.CeilToInt((float)remaining / item.maxStackSize);
            return emptySlots >= slotsNeeded;
        }
        else
        {
            return emptySlots >= remaining;
        }
    }

    /// <summary>
    /// Удаляет предмет из конкретного слота (для исправления бага с использованием)
    /// </summary>
    public bool RemoveItemFromSlot(InventorySlot slot, int amount = 1)
    {
        if (slot == null || amount <= 0)
            return false;

        var item = slot.GetComponentInChildren<DraggableItem>();
        if (item == null)
            return false;

        string itemName = item.itemData.itemName;
        int slotIndex = System.Array.IndexOf(inventorySlots, slot);
        bool isInventorySlot = slotIndex >= 0;

        if (!isInventorySlot)
        {
            slotIndex = System.Array.IndexOf(equipmentSlots, slot);
            if (slotIndex < 0)
                return false;
        }

        if (item.count <= amount)
        {
            Destroy(item.gameObject);

            if (isInventorySlot)
                inventoryData.ClearInventorySlot(slotIndex);
            else
                inventoryData.ClearEquipmentSlot(slotIndex);
        }
        else
        {
            item.count -= amount;
            item.RefreshCount();

            if (isInventorySlot)
                inventoryData.SetInventorySlot(slotIndex, itemName, item.count);
            else
                inventoryData.SetEquipmentSlot(slotIndex, itemName, item.count);
        }

        return true;
    }

    public void SyncFromUI()
    {
        SyncSlotsToData(inventorySlots, true);
        SyncSlotsToData(equipmentSlots, false);
    }

    public void SyncInventorySlot(int index)
    {
        if (index < 0 || index >= inventorySlots.Length) return;

        var item = inventorySlots[index].GetComponentInChildren<DraggableItem>();
        if (item != null && item.itemData != null)
            inventoryData.SetInventorySlot(index, item.itemData.itemName, item.count);
        else
            inventoryData.ClearInventorySlot(index);
    }

    public void SyncEquipmentSlot(int index)
    {
        if (index < 0 || index >= equipmentSlots.Length) return;

        var item = equipmentSlots[index].GetComponentInChildren<DraggableItem>();
        if (item != null && item.itemData != null)
            inventoryData.SetEquipmentSlot(index, item.itemData.itemName, item.count);
        else
            inventoryData.ClearEquipmentSlot(index);
    }

    /// <summary>
    /// Обновляет UI инвентаря из InventoryData (после закрытия магазина)
    /// </summary>
    public void RefreshUIFromData()
    {
        ClearAllSlots();

        var invSlots = inventoryData.InventorySlots;
        for (int i = 0; i < inventorySlots.Length && i < invSlots.Count; i++)
        {
            if (string.IsNullOrEmpty(invSlots[i].itemName) || invSlots[i].count <= 0)
                continue;

            ItemData itemData = FindItemDataByName(invSlots[i].itemName);
            if (itemData != null)
            {
                SpawnItemInSlot(itemData, inventorySlots[i], invSlots[i].count);
            }
        }

        var eqSlots = inventoryData.EquipmentSlots;
        for (int i = 0; i < equipmentSlots.Length && i < eqSlots.Count; i++)
        {
            if (string.IsNullOrEmpty(eqSlots[i].itemName) || eqSlots[i].count <= 0)
                continue;

            ItemData itemData = FindItemDataByName(eqSlots[i].itemName);
            if (itemData != null)
            {
                SpawnItemInSlot(itemData, equipmentSlots[i], eqSlots[i].count);
            }
        }
    }

    #endregion

    #region Internal Logic

    private bool AddItemInternal(ItemData item, int amount)
    {
        int remaining = amount;

        if (item.isStackable)
        {
            remaining = AddToExistingStacks(item, remaining);
            if (remaining <= 0) return true;
        }

        remaining = AddToEmptySlots(item, remaining);

        if (remaining > 0)
        {
            Debug.Log("Инвентарь полон!");
            return false;
        }

        return true;
    }

    private int AddToExistingStacks(ItemData item, int amount)
    {
        for (int i = 0; i < inventorySlots.Length && amount > 0; i++)
        {
            var slotItem = inventorySlots[i].GetComponentInChildren<DraggableItem>();
            if (slotItem == null || slotItem.itemData != item) continue;
            if (slotItem.count >= item.maxStackSize) continue;

            int space = item.maxStackSize - slotItem.count;
            int toAdd = Mathf.Min(amount, space);

            slotItem.count += toAdd;
            slotItem.RefreshCount();
            inventoryData.SetInventorySlot(i, item.itemName, slotItem.count);

            amount -= toAdd;
        }

        return amount;
    }

    private int AddToEmptySlots(ItemData item, int amount)
    {
        for (int i = 0; i < inventorySlots.Length && amount > 0; i++)
        {
            if (inventorySlots[i].transform.childCount > 0) continue;

            int toAdd = item.isStackable ? Mathf.Min(amount, item.maxStackSize) : 1;
            SpawnItemInSlot(item, inventorySlots[i], toAdd);
            inventoryData.SetInventorySlot(i, item.itemName, toAdd);

            amount -= toAdd;
        }

        return amount;
    }

    private int RemoveFromSlots(InventorySlot[] slots, string itemName, int amount, bool isInventory)
    {
        for (int i = 0; i < slots.Length && amount > 0; i++)
        {
            var item = slots[i].GetComponentInChildren<DraggableItem>();
            if (item == null || item.itemData.itemName != itemName) continue;

            if (item.count <= amount)
            {
                amount -= item.count;
                Destroy(item.gameObject);

                if (isInventory)
                    inventoryData.ClearInventorySlot(i);
                else
                    inventoryData.ClearEquipmentSlot(i);
            }
            else
            {
                item.count -= amount;
                item.RefreshCount();

                if (isInventory)
                    inventoryData.SetInventorySlot(i, itemName, item.count);
                else
                    inventoryData.SetEquipmentSlot(i, itemName, item.count);

                amount = 0;
            }
        }

        return amount;
    }

    private void SyncSlotsToData(InventorySlot[] slots, bool isInventory)
    {
        for (int i = 0; i < slots.Length; i++)
        {
            var item = slots[i].GetComponentInChildren<DraggableItem>();

            if (item != null && item.itemData != null)
            {
                if (isInventory)
                    inventoryData.SetInventorySlot(i, item.itemData.itemName, item.count);
                else
                    inventoryData.SetEquipmentSlot(i, item.itemData.itemName, item.count);
            }
            else
            {
                if (isInventory)
                    inventoryData.ClearInventorySlot(i);
                else
                    inventoryData.ClearEquipmentSlot(i);
            }
        }
    }

    private void SpawnItemInSlot(ItemData item, InventorySlot slot, int amount)
    {
        GameObject newItem = Instantiate(inventoryItemPrefab, slot.transform);
        newItem.GetComponent<DraggableItem>().InitialiseItem(item, amount);
    }

    public ItemData FindItemDataByName(string itemName)
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");

        foreach (ItemData item in allItems)
        {
            if (item != null && item.itemName == itemName)
                return item;
        }

        return null;
    }

    #endregion

    #region Save/Load

    public void SaveInventory()
    {
        SyncFromUI();

        InventorySaver saver = inventoryData.CreateSaveData();
        
        // Сохраняем монеты
        if (playerWallet != null)
        {
            saver.coins = playerWallet.Coins;
            Debug.Log($"Инвентарь сохранён. Монет: {playerWallet.Coins}");
        }
        else
        {
            Debug.LogWarning("PlayerWallet не назначен! Монеты не сохранены.");
            saver.coins = 0;
        }
        
        SaveLoadSystem.Save(INVENTORY_DATA, saver, GAME_DIRECTORY);
    }

    public void LoadInventory()
    {
        // Добавь GlobalLoader.GAME_DIRECTORY
        if (!SaveLoadSystem.Exists(INVENTORY_DATA, GAME_DIRECTORY))
        {
            Debug.Log("Сохранение инвентаря не найдено");
            return;
        }
        InventorySaver saver = SaveLoadSystem.Load<InventorySaver>(INVENTORY_DATA, GAME_DIRECTORY);
        if (saver == null)
        {
            Debug.LogWarning("Не удалось загрузить данные инвентаря");
            return;
        }
        inventoryData.LoadFromSaveData(saver);
        
        // Загружаем монеты
        LoadCoins(saver.coins);
        
        RefreshUI();
        Debug.Log($"Инвентарь загружен. Монет: {saver.coins}");
    }

    private void LoadCoins(int coins)
    {
        if (playerWallet != null)
        {
            playerWallet.SetCoins(coins);
            Debug.Log($"Монеты загружены: {coins}");
        }
        else
        {
            Debug.LogWarning("PlayerWallet не назначен! Монеты не загружены. Назначь PlayerWallet в InventoryManager.");
            // Попробуем найти автоматически
            playerWallet = FindObjectOfType<PlayerWallet>();
            if (playerWallet != null)
            {
                playerWallet.SetCoins(coins);
                Debug.Log($"PlayerWallet найден автоматически. Монеты загружены: {coins}");
            }
        }
    }

    private void RefreshUI()
    {
        ClearAllSlots();

        var invSlots = inventoryData.InventorySlots;
        for (int i = 0; i < inventorySlots.Length && i < invSlots.Count; i++)
        {
            if (string.IsNullOrEmpty(invSlots[i].itemName) || invSlots[i].count <= 0) continue;

            ItemData itemData = FindItemDataByName(invSlots[i].itemName);
            if (itemData != null)
                SpawnItemInSlot(itemData, inventorySlots[i], invSlots[i].count);
            else
                Debug.LogWarning($"Предмет '{invSlots[i].itemName}' не найден!");
        }

        var eqSlots = inventoryData.EquipmentSlots;
        for (int i = 0; i < equipmentSlots.Length && i < eqSlots.Count; i++)
        {
            if (string.IsNullOrEmpty(eqSlots[i].itemName) || eqSlots[i].count <= 0) continue;

            ItemData itemData = FindItemDataByName(eqSlots[i].itemName);
            if (itemData != null)
                SpawnItemInSlot(itemData, equipmentSlots[i], eqSlots[i].count);
            else
                Debug.LogWarning($"Предмет '{eqSlots[i].itemName}' не найден!");
        }
    }

    private void ClearAllSlots()
    {
        foreach (var slot in inventorySlots)
        {
            if (slot.transform.childCount > 0)
                Destroy(slot.transform.GetChild(0).gameObject);
        }

        foreach (var slot in equipmentSlots)
        {
            if (slot.transform.childCount > 0)
                Destroy(slot.transform.GetChild(0).gameObject);
        }
    }

    #endregion
}

