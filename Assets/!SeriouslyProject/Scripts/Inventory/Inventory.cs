using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int _inventorySize = 36;         // обычные слоты
    [SerializeField] private int _equipmentSize = 4;          // слоты экипировки (например: голова, тело, оружие, ботинки)

    private InventorySlot[] _slots;           // обычные предметы
    private InventorySlot[] _equipmentSlots;  // экипировка

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

    private void InitializeInventory()
    {
        // === обычные слоты ===
        _slots = new InventorySlot[_inventorySize];
        for (int i = 0; i < _inventorySize; i++)
            _slots[i] = new InventorySlot();

        // === экипировка ===
        _equipmentSlots = new InventorySlot[_equipmentSize];
        for (int i = 0; i < _equipmentSize; i++)
            _equipmentSlots[i] = new InventorySlot();

        Debug.Log($"Inventory initialized with {_inventorySize} slots + {_equipmentSize} equipment slots");
    }

    // === Работа с обычными слотами ===
    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= _inventorySize)
        {
            Debug.LogWarning($"Inventory slot index {index} is out of range!");
            return null;
        }
        return _slots[index];
    }

    // === Работа с экипировкой ===
    public InventorySlot GetEquipmentSlot(int index)
    {
        if (index < 0 || index >= _equipmentSize)
        {
            Debug.LogWarning($"Equipment slot index {index} is out of range!");
            return null;
        }
        return _equipmentSlots[index];
    }

    // ✅ Добавить предмет (в инвентарь)
    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0) return false;
        int remainingQuantity = quantity;

        // 1) Добавляем в существующие стаки
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

        // 2) Кладём в пустые слоты
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
        return remainingQuantity < quantity;
    }

    // ✅ Экипировать предмет (например, шлем в слот головы)
    public bool EquipItem(int equipmentIndex, Item item)
    {
        if (equipmentIndex < 0 || equipmentIndex >= _equipmentSize) return false;
        if (item == null) return false;

        _equipmentSlots[equipmentIndex].Clear();
        _equipmentSlots[equipmentIndex].AddItem(item, 1);

        NotifyInventoryChanged();
        return true;
    }

    // ✅ Снять предмет (возвращает его обратно в инвентарь)
    public bool UnequipItem(int equipmentIndex)
    {
        if (equipmentIndex < 0 || equipmentIndex >= _equipmentSize) return false;

        var slot = _equipmentSlots[equipmentIndex];
        if (slot.IsEmpty()) return false;

        Item item = slot.Item;
        slot.Clear();

        AddItem(item, 1); // кладём обратно в инвентарь
        NotifyInventoryChanged();
        return true;
    }

    // ✅ Удаление по индексу
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

    // ✅ Удаление по Item
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
        return remaining < quantity;
    }

    // ✅ Проверка
    public bool Contains(Item item, int quantity = 1) => CountItem(item) >= quantity;

    // ✅ Подсчёт предмета
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

    // ✅ Очистка
    public void ClearInventory()
    {
        for (int i = 0; i < _inventorySize; i++)
        {
            _slots[i].Clear();
            NotifySlotChanged(i);
        }

        for (int i = 0; i < _equipmentSize; i++)
            _equipmentSlots[i].Clear();

        NotifyInventoryChanged();
    }

    private void NotifySlotChanged(int slotIndex) => OnSlotChanged?.Invoke(slotIndex);
    private void NotifyInventoryChanged() => OnInventoryChanged?.Invoke();
}
