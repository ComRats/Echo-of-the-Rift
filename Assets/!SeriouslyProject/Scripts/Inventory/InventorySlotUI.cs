using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using UnityEngine.EventSystems;

public class InventorySlotUI : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI References")]
    [SerializeField] private Image _backgroundImage;      // Фон слота
    [SerializeField] private Image _itemIcon;             // Иконка предмета
    [SerializeField] private TextMeshProUGUI _quantityText; // Текст количества
    [SerializeField] private CanvasGroup _canvasGroup;    // Для изменения альфы при драге
    
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
    
    // Для drag and drop
    private Canvas _parentCanvas;
    private GameObject _draggedItem;
    private Image _draggedIcon;
    private TextMeshProUGUI _draggedQuantity;
    private int _draggedQuantityAmount;
    private Item _draggedItemData;
    
    private void Awake()
    {
        // Находим родительский Canvas
        _parentCanvas = GetComponentInParent<Canvas>();
        
        if (_canvasGroup == null)
        {
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
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
    
    // IPointerDownHandler - Начало клика
    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OnSlotRightClicked?.Invoke(_slotIndex);
            // TODO: Можно добавить логику разделения стака (split stack) здесь
        }
        else if (eventData.button == PointerEventData.InputButton.Left && !_assignedSlot.IsEmpty())
        {
            // Подготовка к драгу
        }
    }
    
    // IBeginDragHandler - Начало драга
    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_assignedSlot.IsEmpty()) return;
        
        // Создаем dragged item
        _draggedItem = new GameObject("DraggedItem");
        _draggedItem.transform.SetParent(_parentCanvas.transform, false);
        _draggedItem.transform.SetAsLastSibling();
        
        RectTransform rectTransform = _draggedItem.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(50, 50); // Размер иконки
        
        _draggedIcon = _draggedItem.AddComponent<Image>();
        _draggedIcon.sprite = _itemIcon.sprite;
        _draggedIcon.raycastTarget = false;
        
        _draggedQuantity = _draggedItem.AddComponent<TextMeshProUGUI>();
        _draggedQuantity.raycastTarget = false;
        _draggedQuantity.fontSize = 14;
        _draggedQuantity.alignment = TextAlignmentOptions.BottomRight;
        
        CanvasGroup draggedGroup = _draggedItem.AddComponent<CanvasGroup>();
        draggedGroup.blocksRaycasts = false;
        
        // Копируем данные
        _draggedItemData = _assignedSlot.Item;
        _draggedQuantityAmount = _assignedSlot.Quantity;
        if (_draggedQuantityAmount > 1)
        {
            _draggedQuantity.text = _draggedQuantityAmount.ToString();
        }
        
        // Убираем из исходного слота
        _assignedSlot.Clear();
        UpdateSlotVisuals();
        
        // Делаем исходный слот полупрозрачным
        _canvasGroup.alpha = 0.6f;
        _canvasGroup.blocksRaycasts = false;
    }
    
    // IDragHandler - Процесс драга
    public void OnDrag(PointerEventData eventData)
    {
        if (_draggedItem != null)
        {
            _draggedItem.transform.position = eventData.position;
        }
    }
    
    // IEndDragHandler - Конец драга
    public void OnEndDrag(PointerEventData eventData)
    {
        if (_draggedItem == null) return;
        
        // Проверяем, над каким слотом дропнули
        InventorySlotUI dropSlot = eventData.pointerEnter?.GetComponent<InventorySlotUI>();
        
        bool success = false;
        if (dropSlot != null && dropSlot != this)
        {
            // Пытаемся переместить в новый слот
            Inventory inventory = FindObjectOfType<Inventory>();
            success = inventory.MoveItem(_slotIndex, dropSlot._slotIndex, _draggedQuantityAmount);
        }
        
        if (!success)
        {
            // Возвращаем обратно
            _assignedSlot.AddItem(_draggedItemData, _draggedQuantityAmount);
            UpdateSlotVisuals();
        }
        
        // Уничтожаем dragged item
        Destroy(_draggedItem);
        _draggedItem = null;
        
        // Восстанавливаем исходный слот
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;
    }
    
    // IPointerEnterHandler - Наведение
    public void OnPointerEnter(PointerEventData eventData)
    {
        _backgroundImage.color = _highlightColor;
        OnSlotHovered?.Invoke(_slotIndex);
    }
    
    // IPointerExitHandler - Уход курсора
    public void OnPointerExit(PointerEventData eventData)
    {
        _backgroundImage.color = _normalColor;
    }
}