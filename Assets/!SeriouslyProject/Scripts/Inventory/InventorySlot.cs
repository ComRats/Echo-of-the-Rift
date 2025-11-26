using System;
using UnityEngine;

/// <summary>
/// Представляет один слот в инвентаре.
/// </summary>
[Serializable]
public class InventorySlot
{
    [SerializeField] private Item _item;
    [SerializeField] private int _quantity;

    public Item Item => _item;
    public int Quantity => _quantity;

    public InventorySlot()
    {
        Clear();
    }

    public InventorySlot(Item item, int quantity)
    {
        this._item = item;
        this._quantity = quantity;
    }

    /// <summary>
    /// Проверяет, пуст ли слот.
    /// </summary>
    public bool IsEmpty() => _item == null || _quantity <= 0;

    /// <summary>
    /// Проверяет, можно ли добавить предмет в этот слот.
    /// </summary>
    public bool CanAddItem(Item itemToAdd)
    {
        if (IsEmpty())
            return true;

        return _item == itemToAdd && _item.IsStackable && _quantity < _item.MaxStackSize;
    }

    /// <summary>
    /// Добавляет предмет в слот.
    /// </summary>
    /// <returns>Остаток, который не удалось добавить.</returns>
    public int AddItem(Item itemToAdd, int quantityToAdd)
    {
        if (itemToAdd == null)
        {
            Debug.LogWarning("[InventorySlot] Attempted to add a null item.");
            return quantityToAdd;
        }

        if (IsEmpty())
        {
            _item = itemToAdd;
        }
        else if (_item != itemToAdd)
        {
            Debug.LogWarning($"[InventorySlot] Cannot add item '{itemToAdd.ItemName}'. The slot already contains '{_item.ItemName}'.");
            return quantityToAdd;
        }

        int spaceLeft = _item.MaxStackSize - _quantity;
        int amountToAdd = Mathf.Min(quantityToAdd, spaceLeft);

        _quantity += amountToAdd;
        
        return quantityToAdd - amountToAdd;
    }

    /// <summary>
    /// Удаляет предмет из слота.
    /// </summary>
    /// <returns>Количество удаленных предметов.</returns>
    public int RemoveItem(int quantityToRemove)
    {
        if (IsEmpty()) return 0;

        int removedAmount = Mathf.Min(quantityToRemove, _quantity);
        _quantity -= removedAmount;

        if (_quantity <= 0)
        {
            Clear();
        }

        return removedAmount;
    }

    /// <summary>
    /// Полностью очищает слот.
    /// </summary>
    public void Clear()
    {
        _item = null;
        _quantity = 0;
    }

    /// <summary>
    /// Создает копию этого слота.
    /// </summary>
    public InventorySlot Clone()
    {
        return new InventorySlot(_item, _quantity);
    }
}