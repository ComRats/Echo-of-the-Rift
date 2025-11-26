using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Определяет тип и ограничения для слота экипировки.
/// </summary>
public class EquipmentSlotType : MonoBehaviour
{
    [Header("Настройки слота")]
    [SerializeField] private EquipmentSlotCategory _slotType = EquipmentSlotCategory.Any;
    [SerializeField] private string _slotName = "Слот экипировки";
    [SerializeField] private Sprite _slotIcon; // Иконка-подсказка для пустого слота

    [Header("Визуальные настройки")]
    [SerializeField] private Image _backgroundHint; // Иконка фона для пустого слота
    [SerializeField] private Color _slotColor = Color.white;
    [SerializeField] private Color _validDropColor = new Color(0.8f, 1f, 0.8f, 0.5f);
    [SerializeField] private Color _invalidDropColor = new Color(1f, 0.8f, 0.8f, 0.3f);

    private InventorySlotUI _slotUI;
    private Color _originalHintColor;

    public EquipmentSlotCategory SlotType => _slotType;
    public string SlotName => _slotName;

    private void Awake()
    {
        _slotUI = GetComponent<InventorySlotUI>();
        if (_slotUI == null) Debug.LogWarning($"[EquipmentSlotType] InventorySlotUI not found on {gameObject.name}");
        
        SetupSlotAppearance();
    }

    private void SetupSlotAppearance()
    {
        if (_backgroundHint != null && _slotIcon != null)
        {
            _backgroundHint.sprite = _slotIcon;
            _originalHintColor = new Color(_slotColor.r, _slotColor.g, _slotColor.b, 0.3f);
            _backgroundHint.color = _originalHintColor;
        }
    }

    /// <summary>
    /// Проверяет, можно ли экипировать предмет в этот слот.
    /// </summary>
    public bool CanEquipItem(Item item)
    {
        if (item == null)
        {
            return false;
        }

        // Логика совместимости делегируется самому предмету.
        bool isCompatible = item.IsCompatibleWithSlotCategory(_slotType);
        
        if (!isCompatible)
        {
            Debug.Log($"Предмет '{item.ItemName}' несовместим со слотом '{_slotName}'.");
        }

        return isCompatible;
    }

    /// <summary>
    /// Показывает или скрывает подсказку на фоне.
    /// </summary>
    public void ShowSlotHint(bool show)
    {
        if (_backgroundHint != null)
        {
            _backgroundHint.gameObject.SetActive(show);
        }
    }

    /// <summary>
    /// Подсвечивает слот во время перетаскивания предмета.
    /// </summary>
    public void HighlightForItem(Item item, bool highlight)
    {
        if (_backgroundHint == null) return;

        if (!highlight)
        {
            _backgroundHint.color = _originalHintColor;
            return;
        }
        
        if (item != null && CanEquipItem(item))
        {
            _backgroundHint.color = _validDropColor;
        }
        else
        {
            _backgroundHint.color = _invalidDropColor;
        }
    }

    /// <summary>
    /// Возвращает описание требований слота.
    /// </summary>
    public string GetSlotRequirements()
    {
        switch (_slotType)
        {
            case EquipmentSlotCategory.Any:
                return "Любой экипируемый предмет";
            case EquipmentSlotCategory.Helmet:
                return "Шлемы, шляпы и другие головные уборы";
            case EquipmentSlotCategory.Chest:
                return "Нагрудная броня, рубашки и другая экипировка для торса";
            case EquipmentSlotCategory.Legs:
                return "Ножная броня, штаны, ботинки и другая обувь";
            case EquipmentSlotCategory.Weapon:
                return "Только оружие";
            case EquipmentSlotCategory.Shield:
                return "Щиты и баклеры";
            case EquipmentSlotCategory.Accessory:
                return "Кольца, амулеты и аксессуары";
            default:
                return "Слот для экипировки";
        }
    }

    /// <summary>
    /// Отлаживает совместимость предмета со слотом.
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugCompatibility(Item item)
    {
        if (item == null)
        {
            Debug.Log($"[EquipmentSlot] Slot '{_slotName}' ({_slotType}): No item to check.");
            return;
        }

        bool isCompatible = CanEquipItem(item);

        Debug.Log($"[EquipmentSlot] Compatibility for '{item.ItemName}' in slot '{_slotName}': {isCompatible}. Requirements: {GetSlotRequirements()}");
    }
}

/// <summary>
/// Категории слотов для экипировки.
/// </summary>
[System.Serializable]
public enum EquipmentSlotCategory
{
    Any,        // Любая экипировка
    Helmet,     // Шлем
    Chest,      // Нагрудник
    Legs,       // Ноги
    Weapon,     // Оружие
    Shield,     // Щит
    Accessory   // Аксессуар
}
