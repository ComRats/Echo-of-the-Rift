using UnityEngine;

/// <summary>
/// ScriptableObject представляющий предмет в системе инвентаря.
/// Содержит базовую информацию, настройки стакирования и логику совместимости с экипировкой.
/// </summary>
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class Item : ScriptableObject
{
    [Header("Basic Info")]
    public int Id;
    public string _itemName;
    [SerializeField] [TextArea(2, 4)] private string _description;
    [SerializeField] private Sprite _icon;
    
    [Header("Stack Settings")]
    [SerializeField] private int _maxStackSize = 64;
    [SerializeField] private bool _isStackable = true;
    
    [Header("Item Type")]
    [SerializeField] private ItemType _itemType;
    [SerializeField] private ItemRarity _rarity = ItemRarity.Common;
    
    [Header("Equipment Subtype")]
    [SerializeField] private EquipmentSubtype _equipmentSubtype = EquipmentSubtype.None;
    
    // Публичные свойства для контролируемого доступа к данным
    public int ID => Id;
    public string ItemName => _itemName;
    public string Description => _description;
    public Sprite Icon => _icon;
    public int MaxStackSize => _maxStackSize;
    public bool IsStackable => _isStackable;
    public ItemType ItemType => _itemType;
    public ItemRarity Rarity => _rarity;
    public EquipmentSubtype EquipmentSubtype => _equipmentSubtype;
    
    /// <summary>
    /// Проверяет совместимость предмета с категорией слота экипировки.
    /// Используется системой drag & drop для валидации размещения предметов.
    /// </summary>
    /// <param name="category">Категория слота экипировки</param>
    /// <returns>True, если предмет можно разместить в слоте данной категории</returns>
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
    /// Виртуальный метод использования предмета.
    /// Переопределяется в наследниках для специфичного поведения.
    /// </summary>
    public virtual void Use()
    {
        Debug.Log($"Using {_itemName}");
    }
    
    /// <summary>
    /// Генерирует текст всплывающей подсказки для UI.
    /// Включает название, описание и тип экипировки если применимо.
    /// </summary>
    /// <returns>Отформатированная строка для отображения в tooltip</returns>
    public virtual string GetTooltip()
    {
        string tooltip = $"<b>{_itemName}</b>\n{_description}";
        
        // Добавляем информацию о типе экипировки
        if (_equipmentSubtype != EquipmentSubtype.None)
        {
            tooltip += $"\n<i>Type: {GetEquipmentSubtypeDisplayName()}</i>";
        }
        
        return tooltip;
    }
    
    /// <summary>
    /// Возвращает локализованное название подтипа экипировки для отображения в UI
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
/// Основные категории предметов в игре
/// </summary>
public enum ItemType
{
    Consumable,     // Расходуемые предметы (зелья, еда)
    Equipment,      // Общая экипировка
    Material,       // Материалы для крафта
    Tool,          // Инструменты
    Weapon,        // Оружие
    Armor          // Броня и защитное снаряжение
}

/// <summary>
/// Уровни редкости предметов, влияющие на цвет отображения и ценность
/// </summary>
public enum ItemRarity
{
    Common,        // Обычные предметы (серый/белый)
    Uncommon,      // Необычные предметы (зеленый)
    Rare,          // Редкие предметы (синий)
    Epic,          // Эпические предметы (фиолетовый)
    Legendary      // Легендарные предметы (оранжевый)
}

/// <summary>
/// Подтипы экипировки для более точной категоризации слотов
/// </summary>
public enum EquipmentSubtype
{
    None,           // Для неэкипируемых предметов
    
    // Подтипы брони
    Helmet,         // Шлемы, шапки
    ChestArmor,     // Нагрудники, рубашки
    LegArmor,       // Штаны, поножи
    Boots,          // Ботинки, сапоги
    Gloves,         // Перчатки
    
    // Подтипы экипировки
    Shield,         // Щиты
    Ring,           // Кольца
    Amulet,         // Амулеты, ожерелья
    Accessory,      // Прочие аксессуары
    Belt,           // Пояса
    Cloak           // Плащи, накидки
}