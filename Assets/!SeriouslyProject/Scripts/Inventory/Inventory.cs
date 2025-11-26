using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

/// <summary>
/// Управляет инвентарем игрока, включая обычные слоты и слоты экипировки.
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Настройки инвентаря")]
    [SerializeField] private int _inventorySize = 36;
    [SerializeField] private int _equipmentSize = 4;

    private InventorySlot[] _slots;           // Array of regular slots
    private InventorySlot[] _equipmentSlots;  // Array of equipment slots

    /// <summary>
    /// Вызывается при изменении содержимого слота. Параметр - индекс слота.
    /// </summary>
    public event Action<int> OnSlotChanged;
    /// <summary>
    /// Вызывается при любом изменении в инвентаре.
    /// </summary>
    public event Action OnInventoryChanged;

    /// <summary>
    /// Количество обычных слотов.
    /// </summary>
    public int Size => _inventorySize;
    /// <summary>
    /// Количество слотов экипировки.
    /// </summary>
    public int EquipmentSize => _equipmentSize;

    /// <summary>
    /// Доступ только для чтения к основным слотам инвентаря.
    /// </summary>
    public IReadOnlyList<InventorySlot> Slots => _slots;
    /// <summary>
    /// Доступ только для чтения к слотам экипировки.
    /// </summary>
    public IReadOnlyList<InventorySlot> EquipmentSlots => _equipmentSlots;

    private void Awake()
    {
        InitializeInventory();
    }

    /// <summary>
    /// Создает все слоты инвентаря при запуске.
    /// </summary>
    private void InitializeInventory()
    {
        _slots = new InventorySlot[_inventorySize];
        for (int i = 0; i < _inventorySize; i++)
        {
            _slots[i] = new InventorySlot();
        }

        _equipmentSlots = new InventorySlot[_equipmentSize];
        for (int i = 0; i < _equipmentSize; i++)
        {
            _equipmentSlots[i] = new InventorySlot();
        }
        
        Debug.Log($"[Inventory] Initialized with {_inventorySize} regular slots and {_equipmentSize} equipment slots.");
    }

    /// <summary>
    /// Возвращает обычный слот инвентаря по индексу.
    /// </summary>
    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= _inventorySize)
        {
            Debug.LogWarning($"Inventory slot index {index} is out of range!");
            return null;
        }
        return _slots[index];
    }

    /// <summary>
    /// Возвращает слот экипировки по индексу.
    /// </summary>
    public InventorySlot GetEquipmentSlot(int index)
    {
        if (index < 0 || index >= _equipmentSize)
        {
            Debug.LogWarning($"Equipment slot index {index} is out of range!");
            return null;
        }
        return _equipmentSlots[index];
    }

    /// <summary>
    /// Добавляет предмет в инвентарь.
    /// </summary>
    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
        {
            Debug.LogWarning($"[Inventory] AddItem: Invalid item or quantity.");
            return false;
        }

        int remainingQuantity = quantity;

        // First, try to stack with existing items.
        if (item.IsStackable)
        {
            foreach (var slot in _slots)
            {
                if (slot.CanAddItem(item))
                {
                    remainingQuantity = slot.AddItem(item, remainingQuantity);
                    if (remainingQuantity <= 0) break;
                }
            }
        }
        
        // If items still remain, try to place them in empty slots.
        if (remainingQuantity > 0)
        {
            foreach (var slot in _slots)
            {
                if (slot.IsEmpty())
                {
                    remainingQuantity = slot.AddItem(item, remainingQuantity);
                    if (remainingQuantity <= 0) break;
                }
            }
        }

        bool itemsAdded = remainingQuantity < quantity;
        if (itemsAdded)
        {
            Debug.Log($"[Inventory] Added {quantity - remainingQuantity} of '{item.ItemName}'.");
            NotifyInventoryChanged();
        }
        else
        {
            Debug.LogWarning($"[Inventory] Failed to add '{item.ItemName}'. Inventory may be full.");
        }

        return itemsAdded;
    }

    /// <summary>
    /// Экипирует предмет в указанный слот.
    /// </summary>
    public bool EquipItem(int equipmentIndex, Item item)
    {
        if (equipmentIndex < 0 || equipmentIndex >= _equipmentSize)
        {
            Debug.LogWarning($"[Inventory] EquipItem: Invalid equipment index: {equipmentIndex}");
            return false;
        }
        if (item == null)
        {
            Debug.LogWarning("[Inventory] EquipItem: Item is null.");
            return false;
        }

        _equipmentSlots[equipmentIndex].Clear();
        _equipmentSlots[equipmentIndex].AddItem(item, 1);

        NotifyInventoryChanged();
        return true;
    }

    /// <summary>
    /// Снимает предмет со слота экипировки.
    /// </summary>
    public bool UnequipItem(int equipmentIndex)
    {
        if (equipmentIndex < 0 || equipmentIndex >= _equipmentSize)
        {
            Debug.LogWarning($"[Inventory] UnequipItem: Invalid equipment index: {equipmentIndex}");
            return false;
        }

        var slot = _equipmentSlots[equipmentIndex];
        if (slot.IsEmpty())
        {
            return false;
        }

        Debug.Log($"[Inventory] Unequipping '{slot.Item.ItemName}' from slot {equipmentIndex}.");
        Item item = slot.Item;
        slot.Clear();

        AddItem(item, 1); // Return the item to inventory
        NotifyInventoryChanged();
        return true;
    }

    /// <summary>
    /// Удаляет указанное количество предметов из определенного слота.
    /// </summary>
    public bool RemoveItem(int slotIndex, int quantity = 1)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySize) return false;

        InventorySlot slot = _slots[slotIndex];
        if (slot.IsEmpty()) return false;

        int removedAmount = slot.RemoveItem(quantity);
        if (removedAmount > 0)
        {
            NotifySlotChanged(slotIndex);
            NotifyInventoryChanged();
            return true;
        }
        return false;
    }

    /// <summary>
    /// Удаляет указанное количество определенного предмета из всего инвентаря.
    /// </summary>
    public bool RemoveItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;

        int remaining = quantity;
        for (int i = 0; i < _inventorySize; i++)
        {
            if (!_slots[i].IsEmpty() && _slots[i].Item == item)
            {
                int removed = _slots[i].RemoveItem(remaining);
                remaining -= removed;
                NotifySlotChanged(i);

                if (remaining <= 0)
                {
                    NotifyInventoryChanged();
                    return true;
                }
            }
        }

        NotifyInventoryChanged();
        return remaining < quantity; // True if at least some were removed
    }

    /// <summary>
    /// Проверяет, содержит ли инвентарь определенное количество предмета.
    /// </summary>
    public bool Contains(Item item, int quantity = 1) => CountItem(item) >= quantity;

    /// <summary>
    /// Подсчитывает общее количество определенного предмета в инвентаре.
    /// </summary>
    public int CountItem(Item item)
    {
        int count = 0;
        for (int i = 0; i < _inventorySize; i++)
        {
            if (!_slots[i].IsEmpty() && _slots[i].Item == item)
                count += _slots[i].Quantity;
        }
        return count;
    }

    /// <summary>
    /// Очищает весь инвентарь.
    /// </summary>
    public void ClearInventory()
    {
        // Clear regular slots
        for (int i = 0; i < _inventorySize; i++)
        {
            _slots[i].Clear();
            NotifySlotChanged(i);
        }

        // Clear equipment slots
        for (int i = 0; i < _equipmentSize; i++)
            _equipmentSlots[i].Clear();

        NotifyInventoryChanged();
    }

    // Methods to notify subscribers of changes
    private void NotifySlotChanged(int slotIndex) => OnSlotChanged?.Invoke(slotIndex);
    private void NotifyInventoryChanged() => OnInventoryChanged?.Invoke();

    #region SAVE / LOAD

    /// <summary>
    /// Сохраняет текущее состояние инвентаря в файл.
    /// </summary>
    public void SaveInventory(string saveFileName)
    {
        Debug.Log($"[Inventory] Saving inventory to '{saveFileName}'...");
        var inventoryData = new InventoryData();

        foreach (var slot in _slots)
        {
            inventoryData.slots.Add(slot.IsEmpty() ? null : new InventorySlotData(slot.Item.Id, slot.Quantity));
        }

        foreach (var slot in _equipmentSlots)
        {
            inventoryData.equipmentSlots.Add(slot.IsEmpty() ? null : new InventorySlotData(slot.Item.Id, slot.Quantity));
        }

        SaveLoadSystem.Save(saveFileName, inventoryData);
        Debug.Log("[Inventory] Inventory saved successfully.");
    }

    /// <summary>
    /// Загружает состояние инвентаря из файла.
    /// </summary>
    public void LoadInventory(string saveFileName, ItemDatabase itemDatabase)
    {
        Debug.Log($"[Inventory] Loading inventory from '{saveFileName}'...");
        var inventoryData = SaveLoadSystem.Load<InventoryData>(saveFileName);

        if (inventoryData == null)
        {
            Debug.LogWarning($"[Inventory] No save file found named '{saveFileName}'. Initializing empty inventory.");
            InitializeInventory(); // Ensure inventory is fresh if no data is loaded
            return;
        }

        // Load regular slots
        for (int i = 0; i < _slots.Length && i < inventoryData.slots.Count; i++)
        {
            _slots[i].Clear();
            var slotData = inventoryData.slots[i];
            if (slotData != null)
            {
                Item item = itemDatabase.GetItemById(slotData.itemId);
                if (item != null)
                {
                    _slots[i].AddItem(item, slotData.quantity);
                }
            }
        }

        // Load equipment slots
        for (int i = 0; i < _equipmentSlots.Length && i < inventoryData.equipmentSlots.Count; i++)
        {
            _equipmentSlots[i].Clear();
            var slotData = inventoryData.equipmentSlots[i];
            if (slotData != null)
            {
                Item item = itemDatabase.GetItemById(slotData.itemId);
                if (item != null)
                {
                    _equipmentSlots[i].AddItem(item, slotData.quantity);
                }
            }
        }

        Debug.Log("[Inventory] Inventory loaded successfully.");
        NotifyInventoryChanged();
    }

    [Serializable]
    private class InventoryData
    {
        public List<InventorySlotData> slots = new List<InventorySlotData>();
        public List<InventorySlotData> equipmentSlots = new List<InventorySlotData>();
    }

    [Serializable]
    private class InventorySlotData
    {
        public int itemId;
        public int quantity;

        public InventorySlotData(int id, int qty)
        {
            itemId = id;
            quantity = qty;
        }
    }

    #endregion
}
