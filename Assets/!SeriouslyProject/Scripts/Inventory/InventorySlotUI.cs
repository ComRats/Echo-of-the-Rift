using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

/// <summary>
/// UI-компонент слота инвентаря с поддержкой перетаскивания.
/// </summary>
public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
{
    [Header("UI Элементы")]
    [SerializeField] private Image _backgroundImage;
    [SerializeField] private Image _itemIcon;
    [SerializeField] private TextMeshProUGUI _quantityText;
    [SerializeField] private Button _slotButton;
    
    [Header("Визуальные настройки")]
    [SerializeField] private Color _normalColor = Color.white;
    [SerializeField] private Color _highlightColor = new Color(1f, 1f, 0.8f, 1f);
    [SerializeField] private Color _selectedColor = new Color(0.8f, 1f, 0.8f, 1f);
    [SerializeField] private Color _dropValidColor = new Color(0.8f, 1f, 0.8f, 0.5f);
    [SerializeField] private Color _dropInvalidColor = new Color(1f, 0.8f, 0.8f, 0.5f);
    
    [Header("Настройки перетаскивания")]
    [SerializeField] private float _dragAlpha = 0.6f;
    [SerializeField] private Vector3 _dragScale = new Vector3(1.2f, 1.2f, 1.2f);
    
    private InventorySlot _assignedSlot;
    private int _slotIndex;
    //Never used
    //private bool _isSelected = false;
    private bool _isEquipmentSlot = false;

    public System.Action<int> OnSlotClicked;
    public System.Action<int> OnSlotRightClicked;
    public System.Action<int> OnSlotHovered;
    
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Vector3 _originalPosition;
    private Transform _originalParent;
    private GameObject _dragIcon;
    private bool _isDragging = false;
    
    private static InventorySlotUI _draggedSlot;
    private static GameObject _dragPreview;
    
    private void Awake()
    {
        InitializeComponents();
        SetSlotEmpty();
    }

    private void InitializeComponents()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        _canvas = GetComponentInParent<Canvas>();
        
        if (_slotButton != null)
        {
            _slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(_slotIndex));
        }
    }
    
    /// <summary>
    /// Инициализирует слот.
    /// </summary>
    public void Initialize(int slotIndex, InventorySlot slot, bool isEquipmentSlot = false)
    {
        _slotIndex = slotIndex;
        _assignedSlot = slot;
        _isEquipmentSlot = isEquipmentSlot;
        Debug.Log($"[InventorySlotUI] Initializing slot UI {_slotIndex}, IsEquipment: {isEquipmentSlot}");
        UpdateSlotVisuals();
    }
    
    /// <summary>
    /// Обновляет визуальное отображение слота.
    /// </summary>
    public void UpdateSlotVisuals()
    {
        if (_assignedSlot == null || _assignedSlot.IsEmpty())
        {
            SetSlotEmpty();
            return;
        }
        
        if (_assignedSlot.Item.Icon != null)
        {
            _itemIcon.sprite = _assignedSlot.Item.Icon;
            _itemIcon.enabled = true;
            _itemIcon.color = Color.white;
        }
        else
        {
            _itemIcon.enabled = false;
        }
        
        if (_assignedSlot.Quantity > 1)
        {
            _quantityText.text = _assignedSlot.Quantity.ToString();
            _quantityText.enabled = true;
            _quantityText.color = Color.white;
        }
        else
        {
            _quantityText.enabled = false;
        }
        
        if (!_isDragging)
            AnimateItemAppear();
    }
    
    /// <summary>
    /// Очищает визуальное отображение слота.
    /// </summary>
    private void SetSlotEmpty()
    {
        _itemIcon.enabled = false;
        _itemIcon.sprite = null;
        _quantityText.enabled = false;
        _quantityText.text = "";
        
        if (!_isDragging)
            _backgroundImage.color = _normalColor;
    }

    #region Drag & Drop Implementation

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_assignedSlot == null || _assignedSlot.IsEmpty())
        {
            Debug.Log($"[InventorySlotUI] OnBeginDrag: Attempted to drag an empty slot {_slotIndex}.");
            return;
        }

        _isDragging = true;
        _draggedSlot = this;

        _originalPosition = _rectTransform.anchoredPosition;
        _originalParent = transform.parent;

        CreateDragPreview();

        _canvasGroup.alpha = _dragAlpha;
        _canvasGroup.blocksRaycasts = false;

        Debug.Log($"[InventorySlotUI] Drag started for item '{_assignedSlot.Item.ItemName}' from slot {_slotIndex}.");
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging || _dragPreview == null)
            return;

        Vector3 screenPosition = eventData.position;

        RectTransform canvasRect = _canvas.GetComponent<RectTransform>();
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            screenPosition,
            _canvas.worldCamera,
            out localPoint
        );

        _dragPreview.transform.localPosition = localPoint;
        UpdateSlotVisuals();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (!_isDragging)
            return;

        _isDragging = false;

        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        if (_dragPreview != null)
        {
            Destroy(_dragPreview);
            _dragPreview = null;
        }

        _draggedSlot = null;

        Debug.Log($"[InventorySlotUI] Drag ended for slot {_slotIndex}.");
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        if (_draggedSlot == null || _draggedSlot == this)
            return;

        Debug.Log($"[InventorySlotUI] OnDrop: предмет перетаскивается на слот {_slotIndex} из слота {_draggedSlot._slotIndex}.");
            
        Debug.Log($"Попытка перетащить {_draggedSlot._assignedSlot.Item.ItemName} на слот {_slotIndex}");
        
        HandleItemDrop(_draggedSlot);
        
        ResetDropHighlight();
    }

    #endregion

    /// <summary>
    /// Создает визуальный preview для перетаскивания
    /// </summary>
    private void CreateDragPreview()
    {
        if (_dragPreview != null)
            Destroy(_dragPreview);
            
        // Создаем копию иконки предмета
        _dragPreview = new GameObject("DragPreview");
        _dragPreview.transform.SetParent(_canvas.transform, false);
        _dragPreview.transform.SetAsLastSibling(); // Поверх всего
        
        // Добавляем Image компонент
        Image previewImage = _dragPreview.AddComponent<Image>();
        previewImage.sprite = _assignedSlot.Item.Icon;
        previewImage.color = new Color(1f, 1f, 1f, 0.8f);
        previewImage.raycastTarget = false;
        
        // Настраиваем RectTransform
        RectTransform previewRect = _dragPreview.GetComponent<RectTransform>();
        previewRect.sizeDelta = _itemIcon.rectTransform.sizeDelta * 1.2f;
        
        // Добавляем текст количества, если нужно
        if (_assignedSlot.Quantity > 1)
        {
            GameObject quantityObj = new GameObject("Quantity");
            quantityObj.transform.SetParent(_dragPreview.transform, false);
            
            TextMeshProUGUI quantityText = quantityObj.AddComponent<TextMeshProUGUI>();
            quantityText.text = _assignedSlot.Quantity.ToString();
            quantityText.fontSize = 14;
            quantityText.color = Color.white;
            quantityText.alignment = TextAlignmentOptions.BottomRight;
            quantityText.raycastTarget = false;
            
            RectTransform quantityRect = quantityObj.GetComponent<RectTransform>();
            quantityRect.anchorMin = Vector2.zero;
            quantityRect.anchorMax = Vector2.one;
            quantityRect.offsetMin = Vector2.zero;
            quantityRect.offsetMax = Vector2.zero;
        }
    }
    
    /// <summary>
    /// Обрабатывает логику размещения предмета в зависимости от типов слотов
    /// </summary>
    private void HandleItemDrop(InventorySlotUI draggedSlot)
    {
        if (draggedSlot == null || draggedSlot.AssignedSlot == null) return;

        var inventory = FindObjectOfType<Inventory>(); // TODO: Replace with dependency injection
        if (inventory == null) return;

        // Determine the type of drag operation
        bool fromInventoryToEquipment = !draggedSlot.IsEquipmentSlot && this.IsEquipmentSlot;
        bool fromEquipmentToInventory = draggedSlot.IsEquipmentSlot && !this.IsEquipmentSlot;
        bool isInventorySwap = !draggedSlot.IsEquipmentSlot && !this.IsEquipmentSlot;
        bool isEquipmentSwap = draggedSlot.IsEquipmentSlot && this.IsEquipmentSlot;

        if (fromInventoryToEquipment)
        {
            HandleEquip(inventory, draggedSlot);
        }
        else if (fromEquipmentToInventory)
        {
            HandleUnequip(inventory, draggedSlot);
        }
        else if (isInventorySwap || isEquipmentSwap)
        {
            HandleSwap(inventory, draggedSlot);
        }
    }

    /// <summary>
    /// Проверяет, можно ли экипировать предмет в этот слот экипировки
    /// </summary>
    private bool CanEquipItemInThisSlot(Item item)
    {
        if (item == null) return false;
        
        // Базовая проверка типа предмета
        if (!CanEquipItem(item)) return false;
        
        // Проверяем через компонент EquipmentSlotType, если он есть
        EquipmentSlotType slotType = GetComponent<EquipmentSlotType>();
        if (slotType != null)
        {
            return slotType.CanEquipItem(item);
        }
        
        // Если компонента нет, разрешаем любую экипировку (обратная совместимость)
        return true;
    }

    /// <summary>
    /// Базовая проверка, является ли предмет экипировкой
    /// </summary>
    private bool CanEquipItem(Item item)
    {
        return item.ItemType == ItemType.Equipment || 
               item.ItemType == ItemType.Weapon || 
               item.ItemType == ItemType.Armor;
    }
    
    /// <summary>
    /// Обрабатывает экипировку предмета
    /// </summary>
    private void HandleEquip(Inventory inventory, InventorySlotUI sourceSlot)
    {
        Item itemToEquip = sourceSlot.AssignedSlot.Item;

        if (!CanEquipItemInThisSlot(itemToEquip))
        {
            Debug.LogWarning($"[InventorySlotUI] Невозможно экипировать '{itemToEquip.ItemName}'. Несовместимый тип слота.");
            return;
        }
        
        // If there's an item in the equipment slot, swap it back to the source inventory slot.
        if (!AssignedSlot.IsEmpty())
        {
            Item currentlyEquipped = AssignedSlot.Item;
            inventory.UnequipItem(SlotIndex);
            sourceSlot.AssignedSlot.Clear();
            sourceSlot.AssignedSlot.AddItem(currentlyEquipped, 1);
        }
        else
        {
            sourceSlot.AssignedSlot.Clear();
        }

        inventory.EquipItem(SlotIndex, itemToEquip);
    }
    
    /// <summary>
    /// Обрабатывает снятие экипировки
    /// </summary>
    private void HandleUnequip(Inventory inventory, InventorySlotUI sourceSlot)
    {
        Item itemToUnequip = sourceSlot.AssignedSlot.Item;

        // If the target inventory slot is empty, just move the item.
        if (AssignedSlot.IsEmpty())
        {
            inventory.UnequipItem(sourceSlot.SlotIndex);
            AssignedSlot.AddItem(itemToUnequip, 1);
        }
        // If the target slot contains an equippable item, swap them.
        else if (CanEquipItem(AssignedSlot.Item))
        {
            HandleSwap(inventory, sourceSlot);
        }
        else
        {
            Debug.LogWarning($"[InventorySlotUI] Невозможно снять экипировку в слот {SlotIndex}. Он занят неэкипируемым предметом.");
        }
    }

    /// <summary>
    /// Обрабатывает обмен между слотами экипировки
    /// </summary>
    private void HandleSwap(Inventory inventory, InventorySlotUI sourceSlot)
    {
        Item sourceItem = sourceSlot.AssignedSlot.Item;
        Item targetItem = this.AssignedSlot.Item;

        // Check if the swap is valid for equipment slots
        if (this.IsEquipmentSlot && !this.CanEquipItemInThisSlot(sourceItem))
        {
             Debug.LogWarning($"[InventorySlotUI] Невозможно обменять: предмет '{sourceItem.ItemName}' несовместим с этим слотом экипировки.");
             return;
        }
        if (sourceSlot.IsEquipmentSlot && !sourceSlot.CanEquipItemInThisSlot(targetItem))
        {
             Debug.LogWarning($"[InventorySlotUI] Невозможно обменять: предмет '{targetItem.ItemName}' несовместим с исходным слотом экипировки.");
             return;
        }

        // Perform the swap
        var tempSlot = sourceSlot.AssignedSlot.Clone();
        sourceSlot.AssignedSlot.Clear();
        if(targetItem != null)
            sourceSlot.AssignedSlot.AddItem(targetItem, this.AssignedSlot.Quantity);
        
        this.AssignedSlot.Clear();
        if(tempSlot.Item != null)
            this.AssignedSlot.AddItem(tempSlot.Item, tempSlot.Quantity);

        // Notify UI to update
        sourceSlot.UpdateSlotVisuals();
        this.UpdateSlotVisuals();
    }
    
    /// <summary>
    /// Универсальный метод обмена предметами между слотами
    /// </summary>
    private void SwapItems(InventorySlotUI otherSlot)
    {
        if (otherSlot == null) return;
        
        // Сохраняем данные слотов
        Item thisItem = _assignedSlot.IsEmpty() ? null : _assignedSlot.Item;
        int thisQuantity = _assignedSlot.IsEmpty() ? 0 : _assignedSlot.Quantity;
        
        Item otherItem = otherSlot._assignedSlot.IsEmpty() ? null : otherSlot._assignedSlot.Item;
        int otherQuantity = otherSlot._assignedSlot.IsEmpty() ? 0 : otherSlot._assignedSlot.Quantity;

        // Очищаем оба слота
        _assignedSlot.Clear();
        otherSlot._assignedSlot.Clear();

        // Устанавливаем новые значения
        if (otherItem != null)
            _assignedSlot.AddItem(otherItem, otherQuantity);

        if (thisItem != null)
            otherSlot._assignedSlot.AddItem(thisItem, thisQuantity);

        // Обновляем визуальное отображение
        UpdateSlotVisuals();
        otherSlot.UpdateSlotVisuals();

        ShowDropFeedback(true);
        Debug.Log($"Обмен предметами между слотами {_slotIndex} и {otherSlot._slotIndex}");
    }

    #region Visual Effects

    /// <summary>
    /// Показывает визуальную обратную связь при drop операции
    /// </summary>
    private void ShowDropFeedback(bool success)
    {
        Color feedbackColor = success ? Color.green : Color.red;

        _backgroundImage.DOColor(feedbackColor, 0.1f)
            .OnComplete(() => _backgroundImage.DOColor(_normalColor, 0.2f));
            
        UpdateSlotVisuals();
    }
    
    /// <summary>
    /// Сбрасывает подсветку drop зоны
    /// </summary>
    private void ResetDropHighlight()
    {
        _backgroundImage.DOColor(_normalColor, 0.1f);
    }
    
    private void OnMouseEnter()
    {
        if (!_isDragging && _draggedSlot != null && _draggedSlot != this)
        {
            // Подсвечиваем слот при наведении во время перетаскивания
            bool canDrop = CanDropItem(_draggedSlot);
            Color highlightColor = canDrop ? _dropValidColor : _dropInvalidColor;
            _backgroundImage.color = highlightColor;
            
            // Подсвечиваем типизированный слот если есть
            EquipmentSlotType slotType = GetComponent<EquipmentSlotType>();
            if (slotType != null && _draggedSlot._assignedSlot != null)
            {
                slotType.HighlightForItem(_draggedSlot._assignedSlot.Item, true);
            }
        }
    }
    
    private void OnMouseExit()
    {
        if (!_isDragging)
        {
            ResetDropHighlight();
            
            // Убираем подсветку типизированного слота
            EquipmentSlotType slotType = GetComponent<EquipmentSlotType>();
            if (slotType != null)
            {
                slotType.HighlightForItem(null, false);
            }
        }
    }
    
    /// <summary>
    /// Проверяет, можно ли сбросить предмет в этот слот
    /// </summary>
   private bool CanDropItem(InventorySlotUI draggedSlot)
    {
        if (draggedSlot == null || draggedSlot._assignedSlot == null)
            return false;
            
        Item draggedItem = draggedSlot._assignedSlot.Item;
        
        // Если перетаскиваем в экипировку
        if (!draggedSlot._isEquipmentSlot && _isEquipmentSlot)
        {
            return CanEquipItemInThisSlot(draggedItem);
        }
        
        // Если обмен между слотами экипировки
        if (draggedSlot._isEquipmentSlot && _isEquipmentSlot)
        {
            // Проверяем совместимость обеих сторон
            bool canDropHere = CanEquipItemInThisSlot(draggedItem);
            
            if (!_assignedSlot.IsEmpty())
            {
                EquipmentSlotType draggedSlotType = draggedSlot.GetComponent<EquipmentSlotType>();
                if (draggedSlotType != null)
                {
                    canDropHere = canDropHere && draggedSlotType.CanEquipItem(_assignedSlot.Item);
                }
            }
            
            return canDropHere;
        }
        
        // В остальных случаях разрешаем
        return true;
    }
    
    /// <summary>
    /// Анимация появления предмета (с DOTween)
    /// </summary>
    private void AnimateItemAppear()
    {
        if (_itemIcon.enabled)
        {
            _itemIcon.transform.localScale = Vector3.zero;
            _itemIcon.transform.DOScale(1f, 0.2f).SetEase(Ease.OutBack);
        }
    }
    
    /// <summary>
    /// Анимация исчезновения предмета
    /// </summary>
    public void AnimateItemDisappear(System.Action onComplete = null)
    {
        _itemIcon.transform.DOScale(0f, 0.15f)
            .SetEase(Ease.InBack)
            .OnComplete(() => {
                SetSlotEmpty();
                onComplete?.Invoke();
            });
    }

    #endregion

    #region Debug Methods

    /// <summary>
    /// Метод для отладки - проверка состояния слота
    /// </summary>
    [System.Diagnostics.Conditional("UNITY_EDITOR")]
    public void DebugSlotState()
    {
        Debug.Log($"=== SLOT {_slotIndex} DEBUG ===");
        Debug.Log($"Is Equipment Slot: {_isEquipmentSlot}");
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
    
    /// <summary>
    /// Принудительное обновление с отладкой
    /// </summary>
    public void ForceUpdate()
    {
        Debug.Log($"Force updating slot {_slotIndex}");
        DebugSlotState();
        UpdateSlotVisuals();
    }

    #endregion
    
    // Публичные свойства для внешнего доступа
    public bool IsEquipmentSlot => _isEquipmentSlot;
    public InventorySlot AssignedSlot => _assignedSlot;
    public int SlotIndex => _slotIndex;
}