using UnityEngine;
using System;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "InventoryData", menuName = "Inventory/Inventory Data")]
public class InventoryData : ScriptableObject
{
    [SerializeField] private List<InventorySlotData> inventorySlots = new List<InventorySlotData>();
    [SerializeField] private List<InventorySlotData> equipmentSlots = new List<InventorySlotData>();

    public IReadOnlyList<InventorySlotData> InventorySlots => inventorySlots;
    public IReadOnlyList<InventorySlotData> EquipmentSlots => equipmentSlots;

    public void Initialize(int inventorySize, int equipmentSize)
    {
        inventorySlots.Clear();
        equipmentSlots.Clear();

        for (int i = 0; i < inventorySize; i++)
            inventorySlots.Add(new InventorySlotData());

        for (int i = 0; i < equipmentSize; i++)
            equipmentSlots.Add(new InventorySlotData());
    }

    public void SetInventorySlot(int index, string itemName, int count)
    {
        if (!IsValidIndex(index, inventorySlots)) return;
        inventorySlots[index] = new InventorySlotData(itemName, count);
    }

    public void SetEquipmentSlot(int index, string itemName, int count)
    {
        if (!IsValidIndex(index, equipmentSlots)) return;
        equipmentSlots[index] = new InventorySlotData(itemName, count);
    }

    public void ClearInventorySlot(int index)
    {
        if (!IsValidIndex(index, inventorySlots)) return;
        inventorySlots[index] = new InventorySlotData();
    }

    public void ClearEquipmentSlot(int index)
    {
        if (!IsValidIndex(index, equipmentSlots)) return;
        equipmentSlots[index] = new InventorySlotData();
    }

    public int FindItem(string itemName)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].itemName == itemName && inventorySlots[i].count > 0)
                return i;
        }
        return -1;
    }

    public int FindItemInEquipment(string itemName)
    {
        for (int i = 0; i < equipmentSlots.Count; i++)
        {
            if (equipmentSlots[i].itemName == itemName && equipmentSlots[i].count > 0)
                return i;
        }
        return -1;
    }

    public bool HasItem(string itemName)
    {
        return FindItem(itemName) >= 0 || FindItemInEquipment(itemName) >= 0;
    }

    public int GetItemCount(string itemName)
    {
        int total = 0;

        foreach (var slot in inventorySlots)
        {
            if (slot.itemName == itemName)
                total += slot.count;
        }

        foreach (var slot in equipmentSlots)
        {
            if (slot.itemName == itemName)
                total += slot.count;
        }

        return total;
    }

    public int FindEmptyInventorySlot()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (string.IsNullOrEmpty(inventorySlots[i].itemName) || inventorySlots[i].count <= 0)
                return i;
        }
        return -1;
    }

    public int FindStackableSlot(string itemName, int maxStackSize)
    {
        for (int i = 0; i < inventorySlots.Count; i++)
        {
            if (inventorySlots[i].itemName == itemName && inventorySlots[i].count < maxStackSize)
                return i;
        }
        return -1;
    }

    public void Clear()
    {
        for (int i = 0; i < inventorySlots.Count; i++)
            inventorySlots[i] = new InventorySlotData();

        for (int i = 0; i < equipmentSlots.Count; i++)
            equipmentSlots[i] = new InventorySlotData();
    }

    public InventorySaver CreateSaveData()
    {
        var saver = new InventorySaver(inventorySlots.Count, equipmentSlots.Count);

        for (int i = 0; i < inventorySlots.Count; i++)
            saver.inventorySlots[i] = new InventorySlotData(inventorySlots[i].itemName, inventorySlots[i].count);

        for (int i = 0; i < equipmentSlots.Count; i++)
            saver.equipmentSlots[i] = new InventorySlotData(equipmentSlots[i].itemName, equipmentSlots[i].count);

        return saver;
    }

    public void LoadFromSaveData(InventorySaver saver)
    {
        if (saver == null) return;

        if (saver.inventorySlots != null)
        {
            for (int i = 0; i < inventorySlots.Count && i < saver.inventorySlots.Length; i++)
            {
                inventorySlots[i] = new InventorySlotData(
                    saver.inventorySlots[i].itemName,
                    saver.inventorySlots[i].count
                );
            }
        }

        if (saver.equipmentSlots != null)
        {
            for (int i = 0; i < equipmentSlots.Count && i < saver.equipmentSlots.Length; i++)
            {
                equipmentSlots[i] = new InventorySlotData(
                    saver.equipmentSlots[i].itemName,
                    saver.equipmentSlots[i].count
                );
            }
        }
    }

    private bool IsValidIndex<T>(int index, List<T> list)
    {
        return index >= 0 && index < list.Count;
    }
}