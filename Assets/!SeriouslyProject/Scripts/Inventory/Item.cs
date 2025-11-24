using UnityEngine;

/// <summary>
/// ScriptableObject, представляющий предмет в инвентаре.
/// </summary>
[CreateAssetMenu(fileName = "Новый предмет", menuName = "Инвентарь/Предмет")]
public class Item : ScriptableObject
{
    [Header("Основная информация")]
    [SerializeField] private int _id;
    [SerializeField] private string _itemName;
    [SerializeField] [TextArea(2, 4)] private string _description;
    [SerializeField] private Sprite _icon;
    
    [Header("Настройки стаков")]
    [SerializeField] private int _maxStackSize = 64;
    [SerializeField] private bool _isStackable = true;
    
    [Header("Тип предмета")]
    [SerializeField] private ItemType _itemType;
    [SerializeField] private ItemRarity _rarity = ItemRarity.Common;
    
    [Header("Подтип экипировки")]
    [SerializeField] private EquipmentSubtype _equipmentSubtype = EquipmentSubtype.None;
    
    public int Id => _id;
    public string ItemName => _itemName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public int MaxStackSize => _maxStackSize;
    public bool IsStackable => _isStackable;
    public ItemType ItemType => _itemType;
    public ItemRarity Rarity => _rarity;
    public EquipmentSubtype EquipmentSubtype => _equipmentSubtype;
    
    /// <summary>
    /// Проверяет, совместим ли предмет с указанной категорией слота.
    /// </summary>
    public bool IsCompatibleWithSlotCategory(EquipmentSlotCategory category)
    {
        switch (category)
        {
            case EquipmentSlotCategory.Any:
                return _itemType == ItemType.Equipment || _itemType == ItemType.Weapon || _itemType == ItemType.Armor;
                
            case EquipmentSlotCategory.Helmet:
                return _equipmentSubtype == EquipmentSubtype.Helmet;
                
            case EquipmentSlotCategory.Chest:
                return _equipmentSubtype == EquipmentSubtype.ChestArmor;
                
            case EquipmentSlotCategory.Legs:
                return _equipmentSubtype == EquipmentSubtype.LegArmor || _equipmentSubtype == EquipmentSubtype.Boots;
                
            case EquipmentSlotCategory.Weapon:
                return _itemType == ItemType.Weapon;
                
            case EquipmentSlotCategory.Shield:
                return _equipmentSubtype == EquipmentSubtype.Shield;
                
            case EquipmentSlotCategory.Accessory:
                return _equipmentSubtype == EquipmentSubtype.Ring || 
                       _equipmentSubtype == EquipmentSubtype.Amulet || 
                       _equipmentSubtype == EquipmentSubtype.Accessory;
                       
            default:
                return false;
        }
    }
    
    /// <summary>
    /// Использовать предмет.
    /// </summary>
    public virtual void Use()
    {
        Debug.Log($"Использован предмет: {_itemName}");
    }
    
    /// <summary>
    /// Генерирует текст для всплывающей подсказки.
    /// </summary>
    public virtual string GetTooltip()
    {
        var builder = new System.Text.StringBuilder();
        builder.Append($"<b>{_itemName}</b>\n");
        builder.Append($"<color=grey><i>{_rarity}</i></color>\n\n");
        builder.Append($"{_description}\n\n");

        if (_itemType == ItemType.Equipment || _itemType == ItemType.Weapon || _itemType == ItemType.Armor)
        {
            builder.Append($"Type: {GetEquipmentSubtypeDisplayName()}\n");
        }

        return builder.ToString();
    }
    
    /// <summary>
    /// Возвращает отображаемое имя подтипа экипировки.
    /// </summary>
    public string GetEquipmentSubtypeDisplayName()
    {
        switch (_equipmentSubtype)
        {
            case EquipmentSubtype.Helmet: return "Helmet";
            case EquipmentSubtype.ChestArmor: return "Chest Armor";
            case EquipmentSubtype.LegArmor: return "Leg Armor";
            case EquipmentSubtype.Boots: return "Boots";
            case EquipmentSubtype.Shield: return "Shield";
            case EquipmentSubtype.Ring: return "Ring";
            case EquipmentSubtype.Amulet: return "Amulet";
            case EquipmentSubtype.Accessory: return "Accessory";
            case EquipmentSubtype.Gloves: return "Gloves";
            case EquipmentSubtype.Belt: return "Belt";
            case EquipmentSubtype.Cloak: return "Cloak";
            case EquipmentSubtype.None:
            default:
                return _itemType.ToString();
        }
    }
}

/// <summary>
/// Категории предметов.
/// </summary>
public enum ItemType
{
    Consumable,     // Расходуемый
    Equipment,      // Экипировка
    Material,       // Материал
    Tool,           // Инструмент
    Weapon,         // Оружие
    Armor           // Броня
}

/// <summary>
/// Редкость предмета.
/// </summary>
public enum ItemRarity
{
    Common,        // Обычный
    Uncommon,      // Необычный
    Rare,          // Редкий
    Epic,          // Эпический
    Legendary      // Легендарный
}

/// <summary>
/// Подтипы экипировки.
/// </summary>
public enum EquipmentSubtype
{
    None,           // Нет
    
    Helmet,         // Шлем
    ChestArmor,     // Нагрудник
    LegArmor,       // Поножи
    Boots,          // Ботинки
    Gloves,         // Перчатки
    
    Shield,         // Щит
    Ring,           // Кольцо
    Amulet,         // Амулет
    Accessory,      // Аксессуар
    Belt,           // Пояс
    Cloak           // Плащ
}