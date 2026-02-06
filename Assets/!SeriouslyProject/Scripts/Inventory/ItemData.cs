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

    private Color GetStackColor(int value)
    {
        return Color.Lerp(Color.green, Color.red, value / 999f);
    }
}