using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

/// <summary>
/// UI компонент слота инвентаря с поддержкой drag & drop
/// Обрабатывает визуализацию предметов и их перетаскивание между слотами
/// </summary>
public class InventorySlotUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IDropHandler
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
    [SerializeField] private Color _dropValidColor = new Color(0.8f, 1f, 0.8f, 0.5f);
    [SerializeField] private Color _dropInvalidColor = new Color(1f, 0.8f, 0.8f, 0.5f);
    
    [Header("Drag Settings")]
    [SerializeField] private float _dragAlpha = 0.6f;
    [SerializeField] private Vector3 _dragScale = new Vector3(1.2f, 1.2f, 1.2f);
    
    // Основные поля слота
    private InventorySlot _assignedSlot;
    private int _slotIndex;
    //private bool _isSelected = false;
    private bool _isEquipmentSlot = false;

    // События для взаимодействия с системой инвентаря
    public System.Action<int> OnSlotClicked;
    public System.Action<int> OnSlotRightClicked;
    public System.Action<int> OnSlotHovered;
    
    // Компоненты для drag & drop
    private Canvas _canvas;
    private CanvasGroup _canvasGroup;
    private RectTransform _rectTransform;
    private Vector3 _originalPosition;
    private Transform _originalParent;
    private GameObject _dragIcon;
    private bool _isDragging = false;
    
    // Статические поля для отслеживания перетаскиваемого объекта
    private static InventorySlotUI _draggedSlot;
    private static GameObject _dragPreview;
    
    private void Awake()
    {
        InitializeComponents();
        SetSlotEmpty();
    }

    /// <summary>
    /// Инициализирует компоненты и подписывается на события
    /// </summary>
    private void InitializeComponents()
    {
        _rectTransform = GetComponent<RectTransform>();
        _canvasGroup = GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = gameObject.AddComponent<CanvasGroup>();
        
        _canvas = GetComponentInParent<Canvas>();
        
        // Подписываемся на события кнопки
        if (_slotButton != null)
        {
            _slotButton.onClick.AddListener(() => OnSlotClicked?.Invoke(_slotIndex));
        }
    }
    
    /// <summary>
    /// Инициализация слота с привязкой к данным инвентаря
    /// </summary>
    /// <param name="slotIndex">Индекс слота в массиве</param>
    /// <param name="slot">Данные слота из системы инвентаря</param>
    /// <param name="isEquipmentSlot">Является ли слот экипировкой</param>
    public void Initialize(int slotIndex, InventorySlot slot, bool isEquipmentSlot = false)
    {
        _slotIndex = slotIndex;
        _assignedSlot = slot;
        _isEquipmentSlot = isEquipmentSlot;
        UpdateSlotVisuals();
    }
    
    /// <summary>
    /// Обновляет визуальное отображение слота на основе данных
    /// </summary>
    public void UpdateSlotVisuals()
    {
        if (_assignedSlot == null || _assignedSlot.IsEmpty())
        {
            SetSlotEmpty();
            return;
        }
        
        // Устанавливаем иконку предмета
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
        
        // Показываем количество только если больше 1
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
        
        // Анимация появления предмета (если не перетаскиваем)
        if (!_isDragging)
            AnimateItemAppear();
    }
    
    /// <summary>
    /// Очищает визуальное отображение пустого слота
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
        // Проверяем, есть ли предмет для перетаскивания
        if (_assignedSlot == null || _assignedSlot.IsEmpty())
            return;

        _isDragging = true;
        _draggedSlot = this;

        // Сохраняем исходное состояние
        _originalPosition = _rectTransform.anchoredPosition;
        _originalParent = transform.parent;

        // Создаем визуальный preview для перетаскивания
        CreateDragPreview();

        // Делаем оригинальный слот полупрозрачным
        _canvasGroup.alpha = _dragAlpha;
        _canvasGroup.blocksRaycasts = false;

        Debug.Log($"Started dragging {_assignedSlot.Item.ItemName} from slot {_slotIndex}");
        UpdateSlotVisuals();
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging || _dragPreview == null)
            return;

        // Перемещаем preview за курсором
        Vector3 screenPosition = eventData.position;

        // Правильное преобразование экранных координат
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

        // Восстанавливаем исходное состояние
        _canvasGroup.alpha = 1f;
        _canvasGroup.blocksRaycasts = true;

        // Уничтожаем preview
        if (_dragPreview != null)
        {
            Destroy(_dragPreview);
            _dragPreview = null;
        }

        _draggedSlot = null;

        Debug.Log($"Ended dragging from slot {_slotIndex}");
        UpdateSlotVisuals();
    }
    
    public void OnDrop(PointerEventData eventData)
    {
        // Проверяем, что на нас что-то перетаскивают
        if (_draggedSlot == null || _draggedSlot == this)
            return;
            
        Debug.Log($"Attempting to drop {_draggedSlot._assignedSlot.Item.ItemName} on slot {_slotIndex}");
        
        // Выполняем логику обмена/перемещения
        HandleItemDrop(_draggedSlot);
        
        // Сбрасываем подсветку
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
        if (draggedSlot == null || draggedSlot._assignedSlot == null)
            return;
            
        InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI == null)
            return;
            
        Item draggedItem = draggedSlot._assignedSlot.Item;
        int draggedQuantity = draggedSlot._assignedSlot.Quantity;
        
        // Определяем тип операции на основе типов слотов
        if (!draggedSlot._isEquipmentSlot && _isEquipmentSlot)
        {
            // Перетаскиваем из инвентаря в экипировку
            HandleEquipItem(draggedSlot, draggedItem);
        }
        else if (draggedSlot._isEquipmentSlot && !_isEquipmentSlot)
        {
            // Перетаскиваем из экипировки в инвентарь
            HandleUnequipItem(draggedSlot, draggedItem, draggedQuantity);
        }
        else if (!draggedSlot._isEquipmentSlot && !_isEquipmentSlot)
        {
            // Обмен между обычными слотами
            SwapInventoryItems(draggedSlot);
        }
        else if (draggedSlot._isEquipmentSlot && _isEquipmentSlot)
        {
            // Обмен между слотами экипировки
            HandleEquipmentSwap(draggedSlot);
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
    private void HandleEquipItem(InventorySlotUI sourceSlot, Item item)
    {
        if (!CanEquipItemInThisSlot(item))
        {
            Debug.Log($"Cannot equip {item.ItemName} in this equipment slot - wrong item type");
            ShowDropFeedback(false);
            return;
        }

        Inventory inventory = FindObjectOfType<Inventory>();
        
        // Если в слоте экипировки уже есть предмет, возвращаем его в инвентарь
        if (!_assignedSlot.IsEmpty())
        {
            Item currentEquippedItem = _assignedSlot.Item;
            inventory.UnequipItem(_slotIndex);
        }
        
        // Убираем предмет из инвентаря и экипируем
        inventory.RemoveItem(sourceSlot._slotIndex, 1);
        inventory.EquipItem(_slotIndex, item);
        
        ShowDropFeedback(true);
        Debug.Log($"Equipped {item.ItemName} to equipment slot {_slotIndex}");
    }
    
    /// <summary>
    /// Обрабатывает снятие экипировки
    /// </summary>
    private void HandleUnequipItem(InventorySlotUI sourceSlot, Item item, int quantity)
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        
        if (_assignedSlot.IsEmpty())
        {
            // Простое перемещение в пустой слот
            _assignedSlot.AddItem(item, quantity);
            sourceSlot._assignedSlot.Clear();
        }
        else if (_assignedSlot.CanAddItem(item))
        {
            // Добавляем к существующему стаку
            _assignedSlot.AddItem(item, quantity);
            sourceSlot._assignedSlot.Clear();
        }
        else
        {
            // Нужен обмен - проверяем совместимость
            Item inventoryItem = _assignedSlot.Item;
            
            EquipmentSlotType sourceSlotType = sourceSlot.GetComponent<EquipmentSlotType>();
            bool canEquipInSourceSlot = false;
            
            if (sourceSlotType != null)
            {
                canEquipInSourceSlot = sourceSlotType.CanEquipItem(inventoryItem);
            }
            else
            {
                // Базовая проверка если нет компонента типа слота
                canEquipInSourceSlot = CanEquipItem(inventoryItem);
            }
            
            if (canEquipInSourceSlot)
            {
                SwapItems(sourceSlot);
            }
            else
            {
                string sourceSlotName = sourceSlotType != null ? sourceSlotType.SlotName : "equipment slot";
                Debug.Log($"Cannot swap - {inventoryItem.ItemName} cannot be equipped in {sourceSlotName}");
                ShowDropFeedback(false);
                return;
            }
        }
        
        ShowDropFeedback(true);
        Debug.Log($"Unequipped {item.ItemName} from equipment slot {sourceSlot._slotIndex}");
    }

    /// <summary>
    /// Обрабатывает обмен между слотами экипировки
    /// </summary>
    private void HandleEquipmentSwap(InventorySlotUI draggedSlot)
    {
        Item draggedItem = draggedSlot._assignedSlot.Item;
        
        // Проверяем совместимость обеих сторон
        bool canSwap = CanEquipItemInThisSlot(draggedItem);
        
        if (canSwap && !_assignedSlot.IsEmpty())
        {
            EquipmentSlotType draggedSlotType = draggedSlot.GetComponent<EquipmentSlotType>();
            if (draggedSlotType != null && !draggedSlotType.CanEquipItem(_assignedSlot.Item))
            {
                canSwap = false;
                Debug.Log($"Cannot equip {_assignedSlot.Item.ItemName} in the other equipment slot type");
            }
        }
        
        if (canSwap)
        {
            SwapEquipmentItems(draggedSlot);
        }
        else
        {
            ShowDropFeedback(false);
        }
    }
    
    /// <summary>
    /// Обмен предметами между обычными слотами
    /// </summary>
    private void SwapInventoryItems(InventorySlotUI draggedSlot)
    {
        SwapItems(draggedSlot);
    }
    
    /// <summary>
    /// Обмен предметами между слотами экипировки
    /// </summary>
    private void SwapEquipmentItems(InventorySlotUI draggedSlot)
    {
        SwapItems(draggedSlot);
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
        Debug.Log($"Swapped items between slots {_slotIndex} and {otherSlot._slotIndex}");
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