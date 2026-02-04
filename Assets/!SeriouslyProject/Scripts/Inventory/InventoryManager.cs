using UnityEngine;
using System;
using System.Collections.Generic;
using EchoRift.SaveLoadSystem;

[Serializable]
public class InventorySaver
{
    public InventorySlotData[] inventorySlots;
    public InventorySlotData[] equipmentSlots;

    public InventorySaver()
    {
        // Конструктор по умолчанию для сериализации
    }

    public InventorySaver(int inventoryCount, int equipmentCount)
    {
        inventorySlots = new InventorySlotData[inventoryCount];
        for (int i = 0; i < inventoryCount; i++)
        {
            inventorySlots[i] = new InventorySlotData();
        }
        
        equipmentSlots = new InventorySlotData[equipmentCount];
        for (int i = 0; i < equipmentCount; i++)
        {
            equipmentSlots[i] = new InventorySlotData();
        }
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
    [Header("Inventory")]
    public InventorySlot[] inventorySlots;
    
    [Header("Equipment")]
    public InventorySlot[] equipmentSlots;
    
    public GameObject inventoryItemPrefab;

    private void Start()
    {
        LoadInventory();
    }

    // private void OnDestroy()
    // {
    //     SaveInventory();
    // }

    #region Inventory

    public bool AddItem(string itemName, int amount = 1)
    {
        ItemData item = FindItemDataByName(itemName);
        
        if (item == null)
        {
            Debug.LogWarning($"Предмет с именем '{itemName}' не найден в Resources/Items!");
            return false;
        }
        
        return AddItemInternal(item, amount);
    }

    public bool RemoveItem(string itemName, int amount = 1)
    {
        if (amount <= 0)
        {
            Debug.LogWarning("Количество для удаления должно быть больше 0!");
            return false;
        }

        int remainingToRemove = amount;

        // Проходим по всем слотам инвентаря
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();

            if (itemInSlot != null && itemInSlot.itemData.itemName == itemName)
            {
                if (itemInSlot.count >= remainingToRemove)
                {
                    // В этом слоте достаточно предметов
                    itemInSlot.count -= remainingToRemove;
                    
                    if (itemInSlot.count <= 0)
                    {
                        Destroy(itemInSlot.gameObject);
                    }
                    else
                    {
                        itemInSlot.RefreshCount();
                    }
                    
                    return true;
                }
                else
                {
                    // Забираем все предметы из этого слота и продолжаем искать
                    remainingToRemove -= itemInSlot.count;
                    Destroy(itemInSlot.gameObject);
                }
            }
        }

        // Проверяем слоты экипировки
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            InventorySlot slot = equipmentSlots[i];
            DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();

            if (itemInSlot != null && itemInSlot.itemData.itemName == itemName)
            {
                if (itemInSlot.count >= remainingToRemove)
                {
                    itemInSlot.count -= remainingToRemove;
                    
                    if (itemInSlot.count <= 0)
                    {
                        Destroy(itemInSlot.gameObject);
                    }
                    else
                    {
                        itemInSlot.RefreshCount();
                    }
                    
                    return true;
                }
                else
                {
                    remainingToRemove -= itemInSlot.count;
                    Destroy(itemInSlot.gameObject);
                }
            }
        }

        // Если мы здесь, значит не хватило предметов
        Debug.LogWarning($"Недостаточно предметов '{itemName}' для удаления. Требуется: {amount}, не хватает: {remainingToRemove}");
        return false;
    }

    public int GetItemCount(string itemName)
    {
        int totalCount = 0;

        // Считаем в инвентаре
        foreach (InventorySlot slot in inventorySlots)
        {
            DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();
            if (itemInSlot != null && itemInSlot.itemData.itemName == itemName)
            {
                totalCount += itemInSlot.count;
            }
        }

        // Считаем в экипировке
        foreach (InventorySlot slot in equipmentSlots)
        {
            DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();
            if (itemInSlot != null && itemInSlot.itemData.itemName == itemName)
            {
                totalCount += itemInSlot.count;
            }
        }

        return totalCount;
    }

    private bool AddItemInternal(ItemData item, int amount = 1)
    {
        if (item.isStackable)
        {
            for (int i = 0; i < inventorySlots.Length; i++)
            {
                InventorySlot slot = inventorySlots[i];
                DraggableItem itemInSlot = slot.GetComponentInChildren<DraggableItem>();

                if (itemInSlot != null &&
                    itemInSlot.itemData == item &&
                    itemInSlot.count < item.maxStackSize)
                {
                    int spaceAvailable = item.maxStackSize - itemInSlot.count;
                    
                    int addedAmount = Mathf.Min(amount, spaceAvailable);
                    
                    itemInSlot.count += addedAmount;
                    itemInSlot.RefreshCount();
                    
                    amount -= addedAmount;
                    
                    if (amount <= 0) return true;
                }
            }
        }

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            InventorySlot slot = inventorySlots[i];
            
            if (slot.transform.childCount == 0)
            {
                int spawnAmount = item.isStackable ? Mathf.Min(amount, item.maxStackSize) : 1;
                SpawnItemInSlot(item, slot, spawnAmount);
                
                amount -= spawnAmount;
                
                if (amount <= 0) return true;
            }
        }

        Debug.Log("Инвентарь полон!");
        return false;
    }

    private void SpawnItemInSlot(ItemData item, InventorySlot slot, int amount)
    {
        GameObject newItemGo = Instantiate(inventoryItemPrefab, slot.transform);
        DraggableItem draggable = newItemGo.GetComponent<DraggableItem>();
        draggable.InitialiseItem(item, amount);
    }

    private ItemData FindItemDataByName(string itemName)
    {
        ItemData[] allItems = Resources.LoadAll<ItemData>("Items");
        
        foreach (ItemData item in allItems)
        {
            if (item != null && item.itemName == itemName)
            {
                return item;
            }
        }
        
        return null;
    }

    #endregion

    #region Save/Load

    public void SaveInventory()
    {
        InventorySaver saver = new InventorySaver(inventorySlots.Length, equipmentSlots.Length);
        
        // Сохраняем инвентарь
        for (int i = 0; i < inventorySlots.Length; i++)
        {
            DraggableItem itemInSlot = inventorySlots[i].GetComponentInChildren<DraggableItem>();
            if (itemInSlot != null)
            {
                saver.inventorySlots[i] = new InventorySlotData(itemInSlot.itemData.itemName, itemInSlot.count);
            }
            else
            {
                saver.inventorySlots[i] = new InventorySlotData("", 0);
            }
        }
        
        // Сохраняем экипировку
        for (int i = 0; i < equipmentSlots.Length; i++)
        {
            DraggableItem itemInSlot = equipmentSlots[i].GetComponentInChildren<DraggableItem>();
            if (itemInSlot != null)
            {
                saver.equipmentSlots[i] = new InventorySlotData(itemInSlot.itemData.itemName, itemInSlot.count);
            }
            else
            {
                saver.equipmentSlots[i] = new InventorySlotData("", 0);
            }
        }
        
        SaveLoadSystem.Save("inventoryData", saver, GlobalLoader.GAME_DIRECTORY);
    }
    
    public void LoadInventory()
    {
        if (!SaveLoadSystem.Exists("inventoryData"))
        {
            return;
        }
        
        InventorySaver saver = SaveLoadSystem.Load<InventorySaver>("inventoryData", GlobalLoader.GAME_DIRECTORY);

        ClearInventory();

        // Загружаем инвентарь
        if (saver.inventorySlots != null)
        {
            for (int i = 0; i < inventorySlots.Length && i < saver.inventorySlots.Length; i++)
            {
                if (!string.IsNullOrEmpty(saver.inventorySlots[i].itemName) && saver.inventorySlots[i].count > 0)
                {
                    ItemData itemData = FindItemDataByName(saver.inventorySlots[i].itemName);
                    if (itemData != null)
                    {
                        SpawnItemInSlot(itemData, inventorySlots[i], saver.inventorySlots[i].count);
                    }
                    else
                    {
                        Debug.LogWarning($"Предмет '{saver.inventorySlots[i].itemName}' не найден при загрузке!");
                    }
                }
            }
        }
        
        // Загружаем экипировку
        if (saver.equipmentSlots != null)
        {
            for (int i = 0; i < equipmentSlots.Length && i < saver.equipmentSlots.Length; i++)
            {
                if (!string.IsNullOrEmpty(saver.equipmentSlots[i].itemName) && saver.equipmentSlots[i].count > 0)
                {
                    ItemData itemData = FindItemDataByName(saver.equipmentSlots[i].itemName);
                    if (itemData != null)
                    {
                        SpawnItemInSlot(itemData, equipmentSlots[i], saver.equipmentSlots[i].count);
                    }
                    else
                    {
                        Debug.LogWarning($"Предмет '{saver.equipmentSlots[i].itemName}' не найден при загрузке!");
                    }
                }
            }
        }
    }
    
    private void ClearInventory()
    {
        // Очищаем инвентарь
        foreach (InventorySlot slot in inventorySlots)
        {
            if (slot.transform.childCount > 0)
            {
                Transform child = slot.transform.GetChild(0);
                Destroy(child.gameObject);
            }
        }
        
        // Очищаем экипировку
        foreach (InventorySlot slot in equipmentSlots)
        {
            if (slot.transform.childCount > 0)
            {
                Transform child = slot.transform.GetChild(0);
                Destroy(child.gameObject);
            }
        }
    }

    #endregion
}