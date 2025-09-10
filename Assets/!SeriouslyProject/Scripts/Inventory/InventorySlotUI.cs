using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using DG.Tweening;

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
    
    // Приватные поля
    private InventorySlot _assignedSlot;
    private int _slotIndex;
    private bool _isSelected = false;
    private bool _isEquipmentSlot = false;

    // События для взаимодействия с слотом
    public System.Action<int> OnSlotClicked;
    public System.Action<int> OnSlotRightClicked;
    public System.Action<int> OnSlotHovered;
    
    // Drag & Drop поля
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
        // Получаем компоненты
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
        
        // Изначально слот пустой
        SetSlotEmpty();
    }
    
    // Инициализация слота с привязкой к данным
    public void Initialize(int slotIndex, InventorySlot slot, bool isEquipmentSlot = false)
    {
        _slotIndex = slotIndex;
        _assignedSlot = slot;
        _isEquipmentSlot = isEquipmentSlot;
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
        
        // Устанавливаем количество
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
        
        // Анимация появления предмета
        if (!_isDragging)
            AnimateItemAppear();
    }
    
    // Очистка слота (визуально)
    private void SetSlotEmpty()
    {
        _itemIcon.enabled = false;
        _itemIcon.sprite = null;
        _quantityText.enabled = false;
        _quantityText.text = "";
        
        if (!_isDragging)
            _backgroundImage.color = _normalColor;
    }
    
    // === DRAG & DROP IMPLEMENTATION ===
    
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
    }
    
    public void OnDrag(PointerEventData eventData)
    {
        if (!_isDragging || _dragPreview == null)
            return;
            
        // Перемещаем preview за курсором
        Vector3 screenPosition = eventData.position;
        Vector3 worldPosition = _canvas.worldCamera != null ? 
            _canvas.worldCamera.ScreenToWorldPoint(screenPosition) : 
            screenPosition;
            
        _dragPreview.transform.position = worldPosition;
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
    
    // Создание визуального preview для перетаскивания
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
    
    // Обработка логики drop
    private void HandleItemDrop(InventorySlotUI draggedSlot)
    {
        if (draggedSlot == null || draggedSlot._assignedSlot == null)
            return;
            
        InventoryUI inventoryUI = FindObjectOfType<InventoryUI>();
        if (inventoryUI == null)
            return;
            
        Item draggedItem = draggedSlot._assignedSlot.Item;
        int draggedQuantity = draggedSlot._assignedSlot.Quantity;
        
        // Случай 1: Перетаскиваем из инвентаря в экипировку
        if (!draggedSlot._isEquipmentSlot && _isEquipmentSlot)
        {
            if (CanEquipItem(draggedItem))
            {
                // Экипируем предмет
                EquipItem(draggedSlot, draggedItem);
            }
            else
            {
                Debug.Log($"Cannot equip {draggedItem.ItemName} in this equipment slot");
                ShowDropFeedback(false);
            }
        }
        // Случай 2: Перетаскиваем из экипировки в инвентарь
        else if (draggedSlot._isEquipmentSlot && !_isEquipmentSlot)
        {
            UnequipItem(draggedSlot, draggedItem, draggedQuantity);
        }
        // Случай 3: Обмен между обычными слотами
        else if (!draggedSlot._isEquipmentSlot && !_isEquipmentSlot)
        {
            SwapInventoryItems(draggedSlot);
        }
        // Случай 4: Обмен между слотами экипировки
        else if (draggedSlot._isEquipmentSlot && _isEquipmentSlot)
        {
            SwapEquipmentItems(draggedSlot);
        }
    }
    
    // Проверка, можно ли экипировать предмет
    private bool CanEquipItem(Item item)
    {
        // Здесь можно добавить логику проверки типа предмета и типа слота экипировки
        // Например, шлемы только в слот головы, оружие только в слот оружия и т.д.
        
        // Пока что разрешаем экипировать любые предметы типа Equipment
        return item.ItemType == ItemType.Equipment || 
               item.ItemType == ItemType.Weapon || 
               item.ItemType == ItemType.Armor;
    }
    
    // Экипировка предмета
    private void EquipItem(InventorySlotUI sourceSlot, Item item)
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        
        // Если в слоте экипировки уже есть предмет, возвращаем его в инвентарь
        if (!_assignedSlot.IsEmpty())
        {
            Item currentEquippedItem = _assignedSlot.Item;
            inventory.UnequipItem(_slotIndex);
        }
        
        // Убираем предмет из инвентаря
        inventory.RemoveItem(sourceSlot._slotIndex, 1);
        
        // Экипируем предмет
        inventory.EquipItem(_slotIndex, item);
        
        ShowDropFeedback(true);
        Debug.Log($"Equipped {item.ItemName} to equipment slot {_slotIndex}");
    }
    
    // Снятие экипировки
    private void UnequipItem(InventorySlotUI sourceSlot, Item item, int quantity)
    {
        Inventory inventory = FindObjectOfType<Inventory>();
        
        // Проверяем, есть ли место в инвентаре
        if (_assignedSlot.IsEmpty())
        {
            // Если целевой слот пустой, просто перемещаем
            _assignedSlot.AddItem(item, quantity);
            sourceSlot._assignedSlot.Clear();
        }
        else if (_assignedSlot.CanAddItem(item))
        {
            // Если можем добавить к существующему стаку
            _assignedSlot.AddItem(item, quantity);
            sourceSlot._assignedSlot.Clear();
        }
        else
        {
            // Обмениваем предметы местами
            SwapItems(sourceSlot);
        }
        
        ShowDropFeedback(true);
        Debug.Log($"Unequipped {item.ItemName} from equipment slot {sourceSlot._slotIndex}");
    }
    
    // Обмен предметами между обычными слотами
    private void SwapInventoryItems(InventorySlotUI draggedSlot)
    {
        SwapItems(draggedSlot);
    }
    
    // Обмен предметами между слотами экипировки
    private void SwapEquipmentItems(InventorySlotUI draggedSlot)
    {
        SwapItems(draggedSlot);
    }
    
    // Универсальный метод обмена предметами
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
    
    // Визуальная обратная связь при drop
    private void ShowDropFeedback(bool success)
    {
        Color feedbackColor = success ? Color.green : Color.red;
        
        _backgroundImage.DOColor(feedbackColor, 0.1f)
            .OnComplete(() => _backgroundImage.DOColor(_normalColor, 0.2f));
    }
    
    // Сброс подсветки drop зоны
    private void ResetDropHighlight()
    {
        _backgroundImage.DOColor(_normalColor, 0.1f);
    }
    
    // === HOVER EFFECTS ===
    
    private void OnMouseEnter()
    {
        if (!_isDragging && _draggedSlot != null && _draggedSlot != this)
        {
            // Подсвечиваем слот при наведении во время перетаскивания
            bool canDrop = CanDropItem(_draggedSlot);
            Color highlightColor = canDrop ? _dropValidColor : _dropInvalidColor;
            _backgroundImage.color = highlightColor;
        }
    }
    
    private void OnMouseExit()
    {
        if (!_isDragging)
        {
            ResetDropHighlight();
        }
    }
    
    // Проверка, можно ли сбросить предмет в этот слот
    private bool CanDropItem(InventorySlotUI draggedSlot)
    {
        if (draggedSlot == null || draggedSlot._assignedSlot == null)
            return false;
            
        Item draggedItem = draggedSlot._assignedSlot.Item;
        
        // Если перетаскиваем в экипировку
        if (!draggedSlot._isEquipmentSlot && _isEquipmentSlot)
        {
            return CanEquipItem(draggedItem);
        }
        
        // В остальных случаях разрешаем
        return true;
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
    
    // Принудительное обновление с отладкой
    public void ForceUpdate()
    {
        Debug.Log($"Force updating slot {_slotIndex}");
        DebugSlotState();
        UpdateSlotVisuals();
    }
    
    // Геттеры для внешнего доступа
    public bool IsEquipmentSlot => _isEquipmentSlot;
    public InventorySlot AssignedSlot => _assignedSlot;
    public int SlotIndex => _slotIndex;
}