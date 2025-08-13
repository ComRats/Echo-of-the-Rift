using System;
using UnityEngine;

[System.Serializable]
public class InventorySlot
{
    [SerializeField] private Item _item;
    [SerializeField] private int _quantity;
    
    // Публичные свойства для контролируемого доступа
    public Item Item => _item;
    public int Quantity => _quantity;
    
    public InventorySlot()
    {
        _item = null;
        _quantity = 0;
    }
    
    public InventorySlot(Item item, int quantity)
    {
        _item = item;
        _quantity = quantity;
    }
    
    // Проверяем, пустой ли слот
    public bool IsEmpty()
    {
        return _item == null || _quantity <= 0;
    }
    
    // Проверяем, можем ли добавить предмет в этот слот
    public bool CanAddItem(Item itemToAdd)
    {
        // Если слот пустой, можем добавить любой предмет
        if (IsEmpty()) 
            return true;
            
        // Если предмет тот же и можно стакать
        return _item == itemToAdd && _item.IsStackable && _quantity < _item.MaxStackSize;
    }
    
    // Добавляем предметы в слот, возвращаем количество, которое не поместилось
    public int AddItem(Item itemToAdd, int quantityToAdd)
    {
        if (!CanAddItem(itemToAdd))
            return quantityToAdd;
        
        // Если слот пустой
        if (IsEmpty())
        {
            _item = itemToAdd;
            _quantity = Math.Min(quantityToAdd, itemToAdd.MaxStackSize);
            return Math.Max(0, quantityToAdd - itemToAdd.MaxStackSize);
        }
        
        // Если предмет уже есть в слоте
        int spaceLeft = _item.MaxStackSize - _quantity;
        int amountToAdd = Math.Min(spaceLeft, quantityToAdd);
        _quantity += amountToAdd;
        
        return quantityToAdd - amountToAdd;
    }
    
    // Убираем предметы из слота
    public int RemoveItem(int quantityToRemove)
    {
        if (IsEmpty())
            return 0;
        
        int removedAmount = Math.Min(quantityToRemove, _quantity);
        _quantity -= removedAmount;
        
        // Если слот опустел
        if (_quantity <= 0)
        {
            _item = null;
            _quantity = 0;
        }
        
        return removedAmount;
    }
    
    // Очищаем слот полностью
    public void Clear()
    {
        _item = null;
        _quantity = 0;
    }
    
    // Создаём копию слота
    public InventorySlot Clone()
    {
        return new InventorySlot(_item, _quantity);
    }
}