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
        
        // Устанавливаем иконку предмета
        _itemIcon.sprite = _assignedSlot.Item.Icon;
        _itemIcon.enabled = true;
        
        // Устанавливаем количество
        if (_assignedSlot.Quantity > 1)
        {
            _quantityText.text = _assignedSlot.Quantity.ToString();
            _quantityText.enabled = true;
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
        _quantityText.enabled = false;
        _backgroundImage.color = _normalColor;
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
}