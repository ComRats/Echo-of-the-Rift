using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class ShopItemEntry
{
    public ItemData item;
    public int stock = -1; // -1 = бесконечный запас

    [HideInInspector]
    public int currentStock;

    public ShopItemEntry()
    {
        stock = -1;
    }

    public ShopItemEntry(ItemData item, int stock = -1)
    {
        this.item = item;
        this.stock = stock;
        this.currentStock = stock;
    }

    public bool HasStock => stock == -1 || currentStock > 0;

    public void ResetStock()
    {
        currentStock = stock;
    }

    public bool TryDecreaseStock(int amount = 1)
    {
        if (stock == -1) return true;

        if (currentStock >= amount)
        {
            currentStock -= amount;
            return true;
        }
        return false;
    }
}

[CreateAssetMenu(fileName = "New Shop", menuName = "Shop/Shop Data")]
public class ShopData : ScriptableObject
{
    [Header("Информация о магазине")]
    public string shopName = "Магазин";

    [Header("Товары")]
    public List<ShopItemEntry> items = new List<ShopItemEntry>();

    [Header("Настройки цен")]
    [Range(0.1f, 1f)]
    [Tooltip("Множитель цены продажи (0.5 = 50% от цены покупки)")]
    public float sellPriceMultiplier = 0.5f;

    public int GetBuyPrice(ItemData item)
    {
        return item.itemPrice;
    }

    public int GetSellPrice(ItemData item)
    {
        return Mathf.Max(1, Mathf.RoundToInt(item.itemPrice * sellPriceMultiplier));
    }

    public void ResetAllStock()
    {
        foreach (var entry in items)
        {
            entry.ResetStock();
        }
    }
}