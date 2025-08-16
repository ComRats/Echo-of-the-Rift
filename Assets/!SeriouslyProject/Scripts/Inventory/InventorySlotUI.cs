using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class InventorySlotUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Image _backgroundImage;      // Фон слота
    [SerializeField] private Image _itemIcon;             // Иконка предмета
    [SerializeField] private TextMeshProUGUI _quantityText; // Текст количества
    [SerializeField] private Button _slotButton;          // Кнопка для взаимодействия
    
    [Header("Visual Settings")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _highlightColor = new Color(1f, 1f, 0.8f, 1f);
    [SerializeField] private Color _selectedColor = new Color(0.8f, 1f, 0.8f, 1f);
    
    // Приватные поля
    private InventorySlot _assignedSlot;
    private int _slotIndex;
    private bool _isSelected = false;

    // События для взаимодействия с слотом
    public System.Action<int> OnSlotClicked;
    public System.Action<int> OnSlotRightClicked;
    public System.Action<int> OnSlotHovered;
    
    private void Awake()
    {
        // Подписываемся на события кнопки
        if (_slotButton != null)
        {
            _slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(_slotIndex));
        }
        
        // Изначально слот пустой
        SetSlotEmpty();
    }
    
    // Инициализация слота с привязкой к данным
    public void Initialize(int slotIndex, InventorySlot slot)
    {
        _slotIndex = slotIndex;
        _assignedSlot = slot;
        UpdateSlotVisuals();
    }
    
    // Обновление визуального отображения слота
    public void UpdateSlotVisuals()
    {
        if (_assignedSlot == null || _assignedSlot.IsEmpty())
        {
            SetSlotEmpty();
            return;
        }
        
        // Отладочный вывод
        Debug.Log($"Updating slot visuals: {_assignedSlot.Item.ItemName}, Quantity: {_assignedSlot.Quantity}");
        
        // Устанавливаем иконку предмета
        if (_assignedSlot.Item.Icon != null)
        {
            _itemIcon.sprite = _assignedSlot.Item.Icon;
            _itemIcon.enabled = true;
            _itemIcon.color = Color.white; // Убеждаемся, что цвет не прозрачный
            
            Debug.Log($"Icon set for {_assignedSlot.Item.ItemName}");
        }
        else
        {
            Debug.LogWarning($"No icon assigned for item: {_assignedSlot.Item.ItemName}");
            _itemIcon.enabled = false;
        }
        
        // Устанавливаем количество
        if (_assignedSlot.Quantity > 1)
        {
            _quantityText.text = _assignedSlot.Quantity.ToString();
            _quantityText.enabled = true;
            _quantityText.color = Color.white; // Убеждаемся, что текст видимый
            
            Debug.Log($"Quantity text set: {_assignedSlot.Quantity}");
        }
        else
        {
            _quantityText.enabled = false;
        }
        
        // Анимация появления предмета
        AnimateItemAppear();
    }
    
    // Очистка слота (визуально)
    private void SetSlotEmpty()
    {
        _itemIcon.enabled = false;
        _itemIcon.sprite = null;
        _quantityText.enabled = false;
        _quantityText.text = "";
        _backgroundImage.color = _normalColor;
        
        Debug.Log($"Slot {_slotIndex} cleared visually");
    }
    
    // Анимация появления предмета (с DOTween)
    private void AnimateItemAppear()
    {
        if (_itemIcon.enabled)
        {
            _itemIcon.transform.localScale = Vector3.zero;
            _itemIcon.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        }
    }
    
    // Анимация исчезновения предмета
    public void AnimateItemDisappear(System.Action onComplete = null)
    {
        _itemIcon.transform.DOScale(0f, 0.15f)
            .SetEase(Ease.InBack)
            .OnComplete(() => {
                SetSlotEmpty();
                onComplete?.Invoke();
            });
    }
    
    // Метод для отладки - проверка состояния слота
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugSlotState()
    {
        Debug.Log($"=== SLOT {_slotIndex} DEBUG ===");
        Debug.Log($"Assigned slot null: {_assignedSlot == null}");
        
        if (_assignedSlot != null)
        {
            Debug.Log($"Slot empty: {_assignedSlot.IsEmpty()}");
            if (!_assignedSlot.IsEmpty())
            {
                Debug.Log($"Item: {_assignedSlot.Item.ItemName}");
                Debug.Log($"Quantity: {_assignedSlot.Quantity}");
                Debug.Log($"Icon null: {_assignedSlot.Item.Icon == null}");
            }
        }
        
        Debug.Log($"ItemIcon enabled: {_itemIcon.enabled}");
        Debug.Log($"ItemIcon sprite null: {_itemIcon.sprite == null}");
        Debug.Log($"QuantityText enabled: {_quantityText.enabled}");
        Debug.Log($"QuantityText text: '{_quantityText.text}'");
        Debug.Log("========================");
    }
    
    // Принудительное обновление с отладкой
    public void ForceUpdate()
    {
        Debug.Log($"Force updating slot {_slotIndex}");
        DebugSlotState();
        UpdateSlotVisuals();
    }
}