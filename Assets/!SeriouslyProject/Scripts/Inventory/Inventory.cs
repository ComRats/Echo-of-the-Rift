using System;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int _inventorySize = 36; // Как в Minecraft: 36 слотов
    
    // Приватный массив слотов
    private InventorySlot[] _slots;
    
    // События для уведомления UI об изменениях
    public event Action<int> OnSlotChanged; // Передаём индекс изменённого слота
    public event Action OnInventoryChanged;  // Общее событие изменения инвентаря
    
    // Публичные свойства для безопасного доступа
    public int Size => _inventorySize;
    public InventorySlot[] Slots => _slots; // Только для чтения извне
    
    private void Awake()
    {
        InitializeInventory();
    }
    
    // Инициализация инвентаря
    private void InitializeInventory()
    {
        _slots = new InventorySlot[_inventorySize];
        
        // Создаём пустые слоты
        for (int i = 0; i < _inventorySize; i++)
        {
            _slots[i] = new InventorySlot();
        }
        
        Debug.Log($"Inventory initialized with {_inventorySize} slots");
    }
    
    // Получить слот по индексу (безопасно)
    public InventorySlot GetSlot(int index)
    {
        if (index < 0 || index >= _inventorySize)
        {
            Debug.LogWarning($"Inventory slot index {index} is out of range!");
            return null;
        }
        
        return _slots[index];
    }
    
    // Добавить предмет в инвентарь (автоматический поиск места)
    public bool AddItem(Item item, int quantity = 1)
    {
        if (item == null || quantity <= 0)
            return false;
        
        int remainingQuantity = quantity;
        
        // Сначала пытаемся добавить к существующим стакам того же предмета
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
                        return true; // Всё поместилось
                    }
                }
            }
        }
        
        // Затем ищем пустые слоты
        for (int i = 0; i < _inventorySize; i++)
        {
            if (_slots[i].IsEmpty())
            {
                remainingQuantity = _slots[i].AddItem(item, remainingQuantity);
                NotifySlotChanged(i);
                
                if (remainingQuantity <= 0)
                {
                    NotifyInventoryChanged();
                    return true; // Всё поместилось
                }
            }
        }
        
        // Если что-то осталось - инвентарь полон
        NotifyInventoryChanged();
        return remainingQuantity < quantity; // Вернём true если хотя бы что-то добавилось
    }
    
    // Убрать предмет из конкретного слота
    public bool RemoveItem(int slotIndex, int quantity = 1)
    {
        if (slotIndex < 0 || slotIndex >= _inventorySize)
            return false;
        
        InventorySlot slot = _slots[slotIndex];
        if (slot.IsEmpty())
            return false;
        
        int removedAmount = slot.RemoveItem(quantity);
        
        if (removedAmount > 0)
        {
            NotifySlotChanged(slotIndex);
            NotifyInventoryChanged();
            return true;
        }
        
        return false;
    }
    
    // Приватные методы для отправки событий
    private void NotifySlotChanged(int slotIndex)
    {
        OnSlotChanged?.Invoke(slotIndex);
    }
    
    private void NotifyInventoryChanged()
    {
        OnInventoryChanged?.Invoke();
    }
}