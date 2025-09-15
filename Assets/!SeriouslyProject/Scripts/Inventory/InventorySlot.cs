using System;
using UnityEngine;

/// <summary>
/// Представляет один слот инвентаря, который может содержать предмет и его количество.
/// Поддерживает стакирование предметов и базовые операции добавления/удаления.
/// </summary>
[System.Serializable]
public class InventorySlot
{
    [SerializeField] private Item _item;
    [SerializeField] private int _quantity;
    
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
    
    /// <summary>
    /// Проверяет, является ли слот пустым
    /// </summary>
    public bool IsEmpty()
    {
        return _item == null || _quantity <= 0;
    }
    
    /// <summary>
    /// Проверяет, можно ли добавить указанный предмет в этот слот.
    /// Учитывает правила стакирования и лимиты стака.
    /// </summary>
    public bool CanAddItem(Item itemToAdd)
    {
        if (IsEmpty()) 
            return true;
            
        // Можем добавить только если это тот же предмет, он стакируется и есть место
        return _item == itemToAdd && _item.IsStackable && _quantity < _item.MaxStackSize;
    }
    
    /// <summary>
    /// Добавляет предметы в слот с учетом лимитов стакирования.
    /// </summary>
    /// <param name="itemToAdd">Предмет для добавления</param>
    /// <param name="quantityToAdd">Количество для добавления</param>
    /// <returns>Количество предметов, которые не поместились в слот</returns>
    public int AddItem(Item itemToAdd, int quantityToAdd)
    {
        if (!CanAddItem(itemToAdd))
            return quantityToAdd;
        
        if (IsEmpty())
        {
            // Заполняем пустой слот, учитывая максимальный размер стака
            _item = itemToAdd;
            _quantity = Math.Min(quantityToAdd, itemToAdd.MaxStackSize);
            return Math.Max(0, quantityToAdd - itemToAdd.MaxStackSize);
        }
        
        // Добавляем к существующему стаку
        int spaceLeft = _item.MaxStackSize - _quantity;
        int amountToAdd = Math.Min(spaceLeft, quantityToAdd);
        _quantity += amountToAdd;
        
        return quantityToAdd - amountToAdd;
    }
    
    /// <summary>
    /// Удаляет указанное количество предметов из слота.
    /// Автоматически очищает слот, если предметы закончились.
    /// </summary>
    /// <param name="quantityToRemove">Количество предметов для удаления</param>
    /// <returns>Фактически удаленное количество предметов</returns>
    public int RemoveItem(int quantityToRemove)
    {
        if (IsEmpty())
            return 0;
        
        int removedAmount = Math.Min(quantityToRemove, _quantity);
        _quantity -= removedAmount;
        
        // Очищаем слот если предметы закончились
        if (_quantity <= 0)
        {
            _item = null;
            _quantity = 0;
        }
        
        return removedAmount;
    }
    
    /// <summary>
    /// Полностью очищает слот
    /// </summary>
    public void Clear()
    {
        _item = null;
        _quantity = 0;
    }
    
    /// <summary>
    /// Создает точную копию этого слота
    /// </summary>
    public InventorySlot Clone()
    {
        return new InventorySlot(_item, _quantity);
    }
}