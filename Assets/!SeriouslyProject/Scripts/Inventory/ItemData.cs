using UnityEngine;
using Sirenix.OdinInspector;

[System.Flags]
public enum ItemType 
{ 
    None = 0,
    Subject = 1 << 0,
    Food = 1 << 1,
    Potion = 1 << 2,
    Weapon = 1 << 3,
    Armor = 1 << 4,
    Amulet = 1 << 5,
    Helmet = 1 << 6
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
[HideMonoScript]
public class ItemData : ScriptableObject
{
    [TitleGroup("Информация о предмете", "Основные настройки", TitleAlignments.Centered)]
    [HorizontalGroup("Информация о предмете/Main", 100)]
    [VerticalGroup("Информация о предмете/Main/Left")]
    [PreviewField(100, ObjectFieldAlignment.Center)]
    [HideLabel]
    [Tooltip("SVG иконка предмета (импортированная как Sprite из Vector Graphics)")]
    public Sprite icon;

    [VerticalGroup("Информация о предмете/Main/Right")]
    [LabelText("Название")]
    [LabelWidth(120)]
    [GUIColor(0.9f, 1f, 0.9f)]
    [Required("Название обязательно!")]
    public string itemName;

    [VerticalGroup("Информация о предмете/Main/Right")]
    [LabelText("Игровой ID")]
    [LabelWidth(120)]
    [Required("Игровой ID обязателен!")]
    public string itemGameName;

    [VerticalGroup("Информация о предмете/Main/Right")]
    [PropertySpace(10)]
    [LabelText("Тип предмета")]
    [LabelWidth(120)]
    [EnumToggleButtons]
    public ItemType itemType;

    [VerticalGroup("Стоимость продажи предмета")]
    [PropertySpace(10)]
    [LabelText("Стоимость предмета")]
    [LabelWidth(180)]
    public int itemPrice;

    [TitleGroup("Описание")]
    [HideLabel]
    [TextArea(4, 12)]
    [InfoBox("Это описание будет показано игроку при наведении", 
             InfoMessageType.Info, VisibleIf = "@string.IsNullOrEmpty(description)")]
    public string description;
    
    [TitleGroup("Настройки стака")]
    [HorizontalGroup("Настройки стака/Stack")]
    [ToggleLeft]
    [LabelText("Можно складывать")]
    public bool isStackable = true;

    [HorizontalGroup("Настройки стака/Stack")]
    [ShowIf("isStackable")]
    [LabelText("Макс. размер")]
    [LabelWidth(85)]
    [MinValue(1)]
    [MaxValue(999)]
    public int maxStackSize = 10;

    [ShowIf("isStackable")]
    [ShowInInspector, ReadOnly]
    [HideLabel]
    [ProgressBar(1, 999, ColorGetter = "GetStackColor", Height = 20)]
    private int StackVisual => maxStackSize;

    [TitleGroup("Эффекты использования")]
    [ShowIf("@itemType.HasFlag(ItemType.Food) || itemType.HasFlag(ItemType.Potion)")]
    [LabelText("Восстановление HP")]
    [LabelWidth(150)]
    [MinValue(0)]
    public int healthRestore = 0;

    [TitleGroup("Эффекты использования")]
    [ShowIf("@itemType.HasFlag(ItemType.Food) || itemType.HasFlag(ItemType.Potion)")]
    [LabelText("Восстановление маны")]
    [LabelWidth(150)]
    [MinValue(0)]
    public int manaRestore = 0;

    [TitleGroup("Бонусы экипировки")]
    [ShowIf("@itemType.HasFlag(ItemType.Weapon) || itemType.HasFlag(ItemType.Armor) || itemType.HasFlag(ItemType.Helmet) || itemType.HasFlag(ItemType.Amulet)")]
    [LabelText("Бонус урона")]
    [LabelWidth(150)]
    public int bonusDamage = 0;

    [TitleGroup("Бонусы экипировки")]
    [ShowIf("@itemType.HasFlag(ItemType.Weapon) || itemType.HasFlag(ItemType.Armor) || itemType.HasFlag(ItemType.Helmet) || itemType.HasFlag(ItemType.Amulet)")]
    [LabelText("Бонус маг. урона")]
    [LabelWidth(150)]
    public int bonusMagicDamage = 0;

    [TitleGroup("Бонусы экипировки")]
    [ShowIf("@itemType.HasFlag(ItemType.Weapon) || itemType.HasFlag(ItemType.Armor) || itemType.HasFlag(ItemType.Helmet) || itemType.HasFlag(ItemType.Amulet)")]
    [LabelText("Бонус брони")]
    [LabelWidth(150)]
    public int bonusArmor = 0;

    [TitleGroup("Бонусы экипировки")]
    [ShowIf("@itemType.HasFlag(ItemType.Weapon) || itemType.HasFlag(ItemType.Armor) || itemType.HasFlag(ItemType.Helmet) || itemType.HasFlag(ItemType.Amulet)")]
    [LabelText("Бонус макс. HP")]
    [LabelWidth(150)]
    public int bonusMaxHealth = 0;

    [TitleGroup("Бонусы экипировки")]
    [ShowIf("@itemType.HasFlag(ItemType.Weapon) || itemType.HasFlag(ItemType.Armor) || itemType.HasFlag(ItemType.Helmet) || itemType.HasFlag(ItemType.Amulet)")]
    [LabelText("Бонус макс. маны")]
    [LabelWidth(150)]
    public int bonusMaxMana = 0;

    [TitleGroup("Бонусы экипировки")]
    [ShowIf("@itemType.HasFlag(ItemType.Weapon) || itemType.HasFlag(ItemType.Armor) || itemType.HasFlag(ItemType.Helmet) || itemType.HasFlag(ItemType.Amulet)")]
    [LabelText("Бонус лечения")]
    [LabelWidth(150)]
    public int bonusHeal = 0;

    [TitleGroup("Бонусы экипировки")]
    [ShowIf("@itemType.HasFlag(ItemType.Weapon) || itemType.HasFlag(ItemType.Armor) || itemType.HasFlag(ItemType.Helmet) || itemType.HasFlag(ItemType.Amulet)")]
    [LabelText("Бонус удачи")]
    [LabelWidth(150)]
    public int bonusLucky = 0;

    public bool IsEquipable()
    {
        return (itemType & (ItemType.Weapon | ItemType.Armor | ItemType.Helmet | ItemType.Amulet)) != 0;
    }

    private Color GetStackColor(int value)
    {
        return Color.Lerp(Color.green, Color.red, value / 999f);
    }
}