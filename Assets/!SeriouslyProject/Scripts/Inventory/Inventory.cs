using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;


/// <summary>
/// Система инвентаря с обычными слотами и слотами экипировки
/// </summary>
public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int _inventorySize = 36;         // обычные слоты для предметов
    [SerializeField] private int _equipmentSize = 4;          // слоты экипировки (шлем, броня, оружие, ботинки)

    private InventorySlot[] _slots;           // массив обычных слотов
    private InventorySlot[] _equipmentSlots;  // массив слотов экипировки

    // События для уведомления UI об изменениях
    public event Action<int> OnSlotChanged;
    public event Action OnInventoryChanged;

    public int Size => _inventorySize;
    public int EquipmentSize => _equipmentSize;

    public InventorySlot[] Slots => _slots;
    public InventorySlot[] EquipmentSlots => _equipmentSlots;

    private void Awake()
    {
        InitializeInventory();
    }

    /// <summary>
    /// Создает все слоты инвентаря при запуске
    /// </summary>
    private void InitializeInventory()
    {
        // Создаем обычные слоты
        _slots = new InventorySlot[_inventorySize];
        for (int i = 0; i < _inventorySize; i++)
            _slots[i] = new InventorySlot();

        // Создаем слоты экипировки
        _equipmentSlots = new InventorySlot[_equipmentSize];
        for (int i = 0; i < _equipmentSize; i++)
            _equipmentSlots[i] = new InventorySlot();

        Debug.Log($"Inventory initialized with {_inventorySize} slots + {_equipmentSize} equipment slots");
    }

    /// <summary>
    /// Получить обычный слот по индексу
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
    /// Получить слот экипировки по индексу
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
    /// Добавляет предмет в инвентарь. Сначала пытается добавить в существующие стаки, затем в пустые слоты
    /// </summary>
    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;
        int remainingQuantity = quantity;

        // Если предмет можно складывать в стаки, ищем существующие стаки
        if (item.IsStackable)
        {
            for (int i = 0; i < _inventorySize; i++)
            {
                if (!_slots[i].IsEmpty() && _slots[i].Item == item)
                {
                    remainingQuantity = _slots[i].AddItem(item, remainingQuantity);
                    NotifySlotChanged(i);
                    if (remainingQuantity <= 0)
                    {
                        NotifyInventoryChanged();
                        return true;
                    }
                }
            }
        }

        // Если остались предметы, кладем в пустые слоты
        for (int i = 0; i < _inventorySize; i++)
        {
            if (_slots[i].IsEmpty())
            {
                remainingQuantity = _slots[i].AddItem(item, remainingQuantity);
                NotifySlotChanged(i);
                if (remainingQuantity <= 0)
                {
                    NotifyInventoryChanged();
                    return true;
                }
            }
        }

        NotifyInventoryChanged();
        return remainingQuantity < quantity; // вернет true, если хотя бы часть добавилась
    }

    /// <summary>
    /// Экипирует предмет в указанный слот экипировки
    /// </summary>
    public bool EquipItem(int equipmentIndex, Item item)
    {
        if (equipmentIndex < 0 || equipmentIndex >= _equipmentSize) return false;
        if (item == null) return false;

        _equipmentSlots[equipmentIndex].Clear();
        _equipmentSlots[equipmentIndex].AddItem(item, 1);

        NotifyInventoryChanged();
        return true;
    }

    /// <summary>
    /// Снимает экипировку и возвращает ее в обычный инвентарь
    /// </summary>
    public bool UnequipItem(int equipmentIndex)
    {
        if (equipmentIndex < 0 || equipmentIndex >= _equipmentSize) return false;

        var slot = _equipmentSlots[equipmentIndex];
        if (slot.IsEmpty()) return false;

        Item item = slot.Item;
        slot.Clear();

        AddItem(item, 1); // возвращаем предмет в инвентарь
        NotifyInventoryChanged();
        return true;
    }

    /// <summary>
    /// Удаляет указанное количество предметов из конкретного слота
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
    /// Удаляет указанное количество определенного предмета из всего инвентаря
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
        return remaining < quantity; // true, если хотя бы что-то удалили
    }

    /// <summary>
    /// Проверяет, есть ли в инвентаре нужное количество предмета
    /// </summary>
    public bool Contains(Item item, int quantity = 1) => CountItem(item) >= quantity;

    /// <summary>
    /// Подсчитывает общее количество определенного предмета в инвентаре
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
    /// Очищает весь инвентарь и экипировку
    /// </summary>
    public void ClearInventory()
    {
        // Очищаем обычные слоты
        for (int i = 0; i < _inventorySize; i++)
        {
            _slots[i].Clear();
            NotifySlotChanged(i);
        }

        // Очищаем слоты экипировки
        for (int i = 0; i < _equipmentSize; i++)
            _equipmentSlots[i].Clear();

        NotifyInventoryChanged();
    }

    // Методы для уведомления подписчиков об изменениях
    private void NotifySlotChanged(int slotIndex) => OnSlotChanged?.Invoke(slotIndex);
    private void NotifyInventoryChanged() => OnInventoryChanged?.Invoke();

    #region SAVE / LOAD

    // Сохранение инвентаря
    public void SaveInventory()
    {
        var data = new InventoryData
        {
            slots = new List<InventorySlotData>(),
            equipmentSlots = new List<InventorySlotData>()
        };

        // обычные слоты
        foreach (var slot in _slots)
            data.slots.Add(!slot.IsEmpty() ? new InventorySlotData { itemId = slot.Item.Id, quantity = slot.Quantity } : null);

        // слоты экипировки
        foreach (var slot in _equipmentSlots)
            data.equipmentSlots.Add(!slot.IsEmpty() ? new InventorySlotData { itemId = slot.Item.Id, quantity = slot.Quantity } : null);

        string fileName = $"inventorySave_{SceneManager.GetActiveScene().name}";
        SaveLoadSystem.Save(fileName, data);

        Debug.Log("Inventory saved!");
    }

    // Загрузка инвентаря
    public void LoadInventory(ItemDatabase itemDatabase)
    {
        string fileName = $"inventorySave_{SceneManager.GetActiveScene().name}";
        var data = SaveLoadSystem.Load<InventoryData>(fileName);
        if (data == null)
        {
            Debug.LogWarning("No inventory save found!");
            return;
        }

        // загружаем обычные слоты
        for (int i = 0; i < _slots.Length; i++)
        {
            _slots[i].Clear();
            if (i < data.slots.Count && data.slots[i] != null)
            {
                Item item = itemDatabase.GetItemById(data.slots[i].itemId);
                if (item != null)
                    _slots[i].AddItem(item, data.slots[i].quantity);
            }
        }

        // загружаем слоты экипировки
        for (int i = 0; i < _equipmentSlots.Length; i++)
        {
            _equipmentSlots[i].Clear();
            if (i < data.equipmentSlots.Count && data.equipmentSlots[i] != null)
            {
                Item item = itemDatabase.GetItemById(data.equipmentSlots[i].itemId);
                if (item != null)
                    _equipmentSlots[i].AddItem(item, data.equipmentSlots[i].quantity);
            }
        }

        Debug.Log("Inventory loaded!");
    }

    [Serializable]
    private class InventoryData
    {
        public List<InventorySlotData> slots;
        public List<InventorySlotData> equipmentSlots;
    }

    [Serializable]
    private class InventorySlotData
    {
        public int itemId;
        public int quantity;
    }

    #endregion

    
}
