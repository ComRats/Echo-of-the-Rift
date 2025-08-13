using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Basic Info")]
    [SerializeField] private int _id;
    [SerializeField] private string _itemName;
    [SerializeField] [TextArea(2, 4)] private string _description;
    [SerializeField] private Sprite _icon;
    
    [Header("Stack Settings")]
    [SerializeField] private int _maxStackSize = 64;
    [SerializeField] private bool _isStackable = true;
    
    [Header("Item Type")]
    [SerializeField] private ItemType _itemType;
    [SerializeField] private ItemRarity _rarity = ItemRarity.Common;
    
    // Публичные свойства для безопасного доступа
    public int ID => _id;
    public string ItemName => _itemName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public int MaxStackSize => _maxStackSize;
    public bool IsStackable => _isStackable;
    public ItemType ItemType => _itemType;
    public ItemRarity Rarity => _rarity;
    
    // Виртуальные методы для расширения функциональности
    public virtual void Use()
    {
        Debug.Log($"Using {_itemName}");
    }
    
    public virtual string GetTooltip()
    {
        return $"<b>{_itemName}</b>\n{_description}";
    }
}

public enum ItemType
{
    Consumable,
    Equipment,
    Material,
    Tool,
    Weapon,
    Armor
}

public enum ItemRarity
{
    Common,
    Uncommon,
    Rare,
    Epic,
    Legendary
}