using UnityEngine;
using UnityEngine.UI;

public class EquipmentSlotType : MonoBehaviour
{
    [Header("Slot Configuration")]
    [SerializeField] private EquipmentSlotCategory _slotType = EquipmentSlotCategory.Any;
    [SerializeField] private string _slotName = "Equipment Slot";
    [SerializeField] private Sprite _slotIcon; // Иконка-подсказка для пустого слота
    
    [Header("Visual Settings")]
    [SerializeField] private Image _backgroundHint; // Фоновая иконка для пустого слота
    [SerializeField] private Color _slotColor = Color.white;
    [SerializeField] private Color _validDropColor = new Color(0.8f, 1f, 0.8f, 0.5f); // Зеленоватый
    [SerializeField] private Color _invalidDropColor = new Color(1f, 0.8f, 0.8f, 0.3f); // Красноватый
    
    private InventorySlotUI _slotUI;
    private Color _originalHintColor;
    
    public EquipmentSlotCategory SlotType => _slotType;
    public string SlotName => _slotName;
    
    private void Awake()
    {
        _slotUI = GetComponent<InventorySlotUI>();
        SetupSlotAppearance();
    }
    
    private void SetupSlotAppearance()
    {
        // Устанавливаем фоновую иконку для подсказки
        if (_backgroundHint != null && _slotIcon != null)
        {
            _backgroundHint.sprite = _slotIcon;
            _originalHintColor = new Color(_slotColor.r, _slotColor.g, _slotColor.b, 0.3f);
            _backgroundHint.color = _originalHintColor;
        }
    }
    
    // Проверяет, может ли данный предмет быть экипирован в этот слот
   public bool CanEquipItem(Item item)
    {
        if (item == null) return false;
        
        // Проверяем общую совместимость типов
        if (!IsItemEquippable(item)) return false;
        
        // Используем новый метод из класса Item для проверки совместимости
        return item.IsCompatibleWithSlotCategory(_slotType);
    }
    
    // Более гибкая проверка совместимости с конкретным типом слота
    private bool CheckSpecificSlotCompatibility(Item item)
    {
        switch (_slotType)
        {
            case EquipmentSlotCategory.Helmet:
                return IsHelmetItem(item);
                        
            case EquipmentSlotCategory.Chest:
                return IsChestItem(item);
                        
            case EquipmentSlotCategory.Legs:
                return IsLegsItem(item);
                        
            case EquipmentSlotCategory.Weapon:
                return item.ItemType == ItemType.Weapon;
                
            case EquipmentSlotCategory.Shield:
                return IsShieldItem(item);
                       
            case EquipmentSlotCategory.Accessory:
                return IsAccessoryItem(item);
                        
            default:
                return IsItemEquippable(item);
        }
    }
    
    // Методы для проверки конкретных типов предметов
    private bool IsHelmetItem(Item item)
    {
        if (item.ItemType != ItemType.Armor) return false;
        
        string itemName = item.ItemName.ToLower();
        return itemName.Contains("helmet") || 
               itemName.Contains("hat") ||
               itemName.Contains("cap") ||
               itemName.Contains("hood") ||
               itemName.Contains("crown");
    }
    
    private bool IsChestItem(Item item)
    {
        if (item.ItemType != ItemType.Armor) return false;
        
        string itemName = item.ItemName.ToLower();
        return itemName.Contains("chest") || 
               itemName.Contains("armor") ||
               itemName.Contains("shirt") ||
               itemName.Contains("tunic") ||
               itemName.Contains("robe") ||
               itemName.Contains("vest");
    }
    
    private bool IsLegsItem(Item item)
    {
        if (item.ItemType != ItemType.Armor) return false;
        
        string itemName = item.ItemName.ToLower();
        return itemName.Contains("legs") || 
               itemName.Contains("pants") ||
               itemName.Contains("boots") ||
               itemName.Contains("shoes") ||
               itemName.Contains("greaves") ||
               itemName.Contains("trousers");
    }
    
    private bool IsShieldItem(Item item)
    {
        if (item.ItemType != ItemType.Equipment) return false;
        
        string itemName = item.ItemName.ToLower();
        return itemName.Contains("shield") ||
               itemName.Contains("buckler");
    }
    
    private bool IsAccessoryItem(Item item)
    {
        if (item.ItemType != ItemType.Equipment) return false;
        
        string itemName = item.ItemName.ToLower();
        return itemName.Contains("ring") || 
               itemName.Contains("amulet") ||
               itemName.Contains("necklace") ||
               itemName.Contains("pendant") ||
               itemName.Contains("charm") ||
               itemName.Contains("bracelet");
    }
    
    private bool IsItemEquippable(Item item)
    {
        return item.ItemType == ItemType.Equipment || 
               item.ItemType == ItemType.Weapon || 
               item.ItemType == ItemType.Armor;
    }
    
    // Показывает/скрывает фоновую подсказку
    public void ShowSlotHint(bool show)
    {
        if (_backgroundHint != null)
        {
            _backgroundHint.gameObject.SetActive(show);
        }
    }
    
    // Улучшенный метод для подсветки совместимых слотов при перетаскивании
    public void HighlightForItem(Item item, bool highlight)
    {
        if (_backgroundHint == null) return;
        
        if (!highlight)
        {
            // Сбрасываем к исходному цвету
            _backgroundHint.color = _originalHintColor;
            return;
        }
        
        // Определяем цвет подсветки на основе совместимости
        if (item != null && CanEquipItem(item))
        {
            _backgroundHint.color = _validDropColor; // Зеленоватый для совместимых
            Debug.Log($"Slot {_slotName} highlighted GREEN for {item.ItemName} - compatible");
        }
        else
        {
            _backgroundHint.color = _invalidDropColor; // Красноватый для несовместимых
            if (item != null)
            {
                Debug.Log($"Slot {_slotName} highlighted RED for {item.ItemName} - incompatible");
            }
        }
    }
    
    // Метод для получения описания требований слота
    public string GetSlotRequirements()
    {
        switch (_slotType)
        {
            case EquipmentSlotCategory.Any:
                return "Accepts any equippable item";
            case EquipmentSlotCategory.Helmet:
                return "Accepts helmets, hats, and headgear";
            case EquipmentSlotCategory.Chest:
                return "Accepts chest armor, shirts, and torso equipment";
            case EquipmentSlotCategory.Legs:
                return "Accepts leg armor, pants, boots, and footwear";
            case EquipmentSlotCategory.Weapon:
                return "Accepts weapons only";
            case EquipmentSlotCategory.Shield:
                return "Accepts shields and bucklers";
            case EquipmentSlotCategory.Accessory:
                return "Accepts rings, amulets, and accessories";
            default:
                return "Equipment slot";
        }
    }
    
    // Метод для отладки совместимости
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugCompatibility(Item item)
    {
        if (item == null)
        {
            Debug.Log($"Slot {_slotName} ({_slotType}): No item to check");
            return;
        }
        
        bool generalCompatible = IsItemEquippable(item);
        bool specificCompatible = CanEquipItem(item);
        
        Debug.Log($"=== COMPATIBILITY CHECK ===");
        Debug.Log($"Slot: {_slotName} ({_slotType})");
        Debug.Log($"Item: {item.ItemName} ({item.ItemType})");
        Debug.Log($"General compatibility: {generalCompatible}");
        Debug.Log($"Specific compatibility: {specificCompatible}");
        Debug.Log($"Requirements: {GetSlotRequirements()}");
        Debug.Log("============================");
    }
}

[System.Serializable]
public enum EquipmentSlotCategory
{
    Any,        // Любой экипируемый предмет
    Helmet,     // Шлемы, шапки
    Chest,      // Нагрудники, рубашки
    Legs,       // Штаны, ботинки
    Weapon,     // Оружие
    Shield,     // Щиты
    Accessory   // Кольца, амулеты
}