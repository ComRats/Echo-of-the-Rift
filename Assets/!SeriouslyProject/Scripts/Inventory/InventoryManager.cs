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

    public bool AddItem(ItemData item, int amount = 1)
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
        
        Debug.LogWarning($"ItemData с именем '{itemName}' не найден!");
        return null;
    }
}