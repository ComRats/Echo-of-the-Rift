using System;
using System.Collections.Generic;
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
}