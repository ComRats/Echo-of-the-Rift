using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Button _closeButton;                 
    [SerializeField] private GameObject _inventoryPanel;          

    [Header("Inventory References")]
    [SerializeField] private Transform _slotsContainer;           
    [SerializeField] private GameObject _slotPrefab;              

    [Header("Equipment References")]
    [SerializeField] private Transform _equipmentSlotsContainer;  
    [SerializeField] private GameObject _equipmentSlotPrefab;     

    [Header("Animation Settings")]
    [SerializeField] private float _openAnimationDuration = 0.3f;
    [SerializeField] private float _closeAnimationDuration = 0.2f;
    
    // Приватные поля
    private Inventory _inventory;                    
    private List<InventorySlotUI> _slotUIElements;   
    private List<InventorySlotUI> _equipmentSlotUIElements; 
    private bool _isOpen = false;
    private int _selectedSlotIndex = -1;             
    
    // Кэш для анимации
    private Vector3 _originalScale;
    private CanvasGroup _canvasGroup;
    
    private void Awake()
    {
        _slotUIElements = new List<InventorySlotUI>();
        _equipmentSlotUIElements = new List<InventorySlotUI>();

        _originalScale = _inventoryPanel.transform.localScale;
        
        _canvasGroup = _inventoryPanel.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
            _canvasGroup = _inventoryPanel.AddComponent<CanvasGroup>();
        
        if (_closeButton != null)
            _closeButton.onClick.AddListener(CloseInventory);
        
        _inventoryPanel.SetActive(false);
    }
    
    private void Start()
    {
        _inventory = FindObjectOfType<Inventory>();
        if (_inventory == null)
        {
            Debug.LogError("Inventory component not found!");
            return;
        }
        
        _inventory.OnSlotChanged += OnSlotChanged;
        _inventory.OnInventoryChanged += OnInventoryChanged;
        
        // Создаём UI для обычных и экипируемых слотов
        CreateInventorySlots();
        CreateEquipmentSlots();
    }
    
    private void OnDestroy()
    {
        if (_inventory != null)
        {
            _inventory.OnSlotChanged -= OnSlotChanged;
            _inventory.OnInventoryChanged -= OnInventoryChanged;
        }
    }
    
    // === Создание обычных слотов ===
    private void CreateInventorySlots()
    {
        foreach (Transform child in _slotsContainer)
            Destroy(child.gameObject);

        _slotUIElements.Clear();
        
        for (int i = 0; i < _inventory.Size; i++)
        {
            GameObject slotObject = Instantiate(_slotPrefab, _slotsContainer);
            InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();
            if (slotUI == null)
            {
                Debug.LogError("Slot prefab must have InventorySlotUI component!");
                continue;
            }

            // Инициализируем как обычный слот (isEquipmentSlot = false)
            slotUI.Initialize(i, _inventory.GetSlot(i), false);
            slotUI.OnSlotClicked += OnSlotClicked;
            slotUI.OnSlotRightClicked += OnSlotRightClicked;
            slotUI.OnSlotHovered += OnSlotHovered;
            
            _slotUIElements.Add(slotUI);
        }
        
        Debug.Log($"Created {_slotUIElements.Count} inventory slots with drag & drop support");
    }

    // === Создание слотов экипировки ===
    private void CreateEquipmentSlots()
    {
        foreach (Transform child in _equipmentSlotsContainer)
            Destroy(child.gameObject);

        _equipmentSlotUIElements.Clear();

        int equipmentSlotCount = _inventory.EquipmentSize; 

        for (int i = 0; i < equipmentSlotCount; i++)
        {
            GameObject slotObject = Instantiate(_equipmentSlotPrefab, _equipmentSlotsContainer);
            InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();
            if (slotUI == null)
            {
                Debug.LogError("Equipment slot prefab must have InventorySlotUI component!");
                continue;
            }

            // Инициализируем как слот экипировки (isEquipmentSlot = true)
            slotUI.Initialize(i, _inventory.GetEquipmentSlot(i), true);
            slotUI.OnSlotClicked += OnEquipmentSlotClicked;
            slotUI.OnSlotRightClicked += OnEquipmentSlotRightClicked;
            slotUI.OnSlotHovered += OnEquipmentSlotHovered;

            _equipmentSlotUIElements.Add(slotUI);
        }

        Debug.Log($"Created {_equipmentSlotUIElements.Count} equipment slots with drag & drop support");
    }

    // === Обычные слоты ===
    private void OnSlotChanged(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < _slotUIElements.Count)
            _slotUIElements[slotIndex].UpdateSlotVisuals();
    }
    
    private void OnInventoryChanged()
    {
        RefreshAllSlots();
    }

    private void RefreshAllSlots()
    {
        foreach (var slot in _slotUIElements)
            slot.UpdateSlotVisuals();

        foreach (var eqSlot in _equipmentSlotUIElements)
            eqSlot.UpdateSlotVisuals();
    }

    // === Экипировка ===
    private void OnEquipmentSlotClicked(int slotIndex)
    {
        Debug.Log($"Equipment Slot {slotIndex} clicked");
        SelectSlot(slotIndex);
        
        // Можно добавить специальную логику для клика по экипировке
        var equipmentSlot = _inventory.GetEquipmentSlot(slotIndex);
        if (!equipmentSlot.IsEmpty())
        {
            Debug.Log($"Equipment slot contains: {equipmentSlot.Item.ItemName}");
        }
    }

    private void OnEquipmentSlotRightClicked(int slotIndex)
    {
        Debug.Log($"Equipment Slot {slotIndex} right clicked");
        
        // Быстрое снятие экипировки правой кнопкой мыши
        var equipmentSlot = _inventory.GetEquipmentSlot(slotIndex);
        if (!equipmentSlot.IsEmpty())
        {
            bool success = _inventory.UnequipItem(slotIndex);
            if (success)
            {
                Debug.Log($"Quickly unequipped item from slot {slotIndex}");
            }
            else
            {
                Debug.Log($"Failed to unequip item from slot {slotIndex} - inventory might be full");
            }
        }
    }

    private void OnEquipmentSlotHovered(int slotIndex)
    {
        // TODO: Подсказка по предмету экипировки
        var equipmentSlot = _inventory.GetEquipmentSlot(slotIndex);
        if (!equipmentSlot.IsEmpty())
        {
            // Здесь можно показать tooltip с информацией о предмете
            Debug.Log($"Hovering over equipped item: {equipmentSlot.Item.ItemName}");
        }
    }

    // === Обычные клики ===
    private void OnSlotClicked(int slotIndex)
    {
        Debug.Log($"Inventory Slot {slotIndex} clicked");
        SelectSlot(slotIndex);
        
        // Можно добавить логику использования предмета при клике
        var slot = _inventory.GetSlot(slotIndex);
        if (!slot.IsEmpty())
        {
            Debug.Log($"Inventory slot contains: {slot.Item.ItemName} x{slot.Quantity}");
        }
    }
    
    private void OnSlotRightClicked(int slotIndex)
    {
        Debug.Log($"Inventory Slot {slotIndex} right clicked");
        
        // Можно добавить логику быстрого использования или разделения стака
        var slot = _inventory.GetSlot(slotIndex);
        if (!slot.IsEmpty())
        {
            // Пример: быстрое использование предмета
            if (slot.Item.ItemType == ItemType.Consumable)
            {
                slot.Item.Use();
                _inventory.RemoveItem(slotIndex, 1);
                Debug.Log($"Used {slot.Item.ItemName}");
            }
            else
            {
                Debug.Log($"Cannot use {slot.Item.ItemName} - not a consumable item");
            }
        }
    }
    
    private void OnSlotHovered(int slotIndex)
    {
        // TODO: Показать tooltip с информацией о предмете
        var slot = _inventory.GetSlot(slotIndex);
        if (!slot.IsEmpty())
        {
            Debug.Log($"Hovering over: {slot.Item.ItemName}");
            // Здесь можно показать tooltip с описанием предмета
        }
    }
    
    private void SelectSlot(int slotIndex)
    {
        _selectedSlotIndex = slotIndex;
        Debug.Log($"Selected slot: {slotIndex}");
    }

    // === Методы для внешнего управления ===
    
    public void OpenInventory()
    {
        if (_isOpen) return;
        
        _isOpen = true;
        _inventoryPanel.SetActive(true);
        
        _inventoryPanel.transform.localScale = Vector3.zero;
        _canvasGroup.alpha = 0f;
        
        DOTween.Sequence()
            .Append(_inventoryPanel.transform.DOScale(_originalScale, _openAnimationDuration).SetEase(Ease.OutBack))
            .Join(_canvasGroup.DOFade(1f, _openAnimationDuration));
            
        Debug.Log("Inventory opened");
    }
    
    public void CloseInventory()
    {
        if (!_isOpen) return;
        
        _isOpen = false;
        
        DOTween.Sequence()
            .Append(_inventoryPanel.transform.DOScale(0f, _closeAnimationDuration).SetEase(Ease.InBack))
            .Join(_canvasGroup.DOFade(0f, _closeAnimationDuration))
            .OnComplete(() => {
                _inventoryPanel.SetActive(false);
                Debug.Log("Inventory closed");
            });
    }
    
    public void ToggleInventory()
    {
        if (_isOpen) 
            CloseInventory();
        else 
            OpenInventory();
    }
    
    // === Методы для отладки и тестирования ===
    
    public void ForceRefreshAllSlots()
    {
        Debug.Log("Force refreshing all slots");
        RefreshAllSlots();
    }
    
    public void DebugInventoryState()
    {
        Debug.Log("=== INVENTORY UI DEBUG ===");
        Debug.Log($"Is Open: {_isOpen}");
        Debug.Log($"Selected Slot: {_selectedSlotIndex}");
        Debug.Log($"Regular Slots: {_slotUIElements.Count}");
        Debug.Log($"Equipment Slots: {_equipmentSlotUIElements.Count}");
        
        // Отладка содержимого слотов
        for (int i = 0; i < _slotUIElements.Count; i++)
        {
            var slot = _inventory.GetSlot(i);
            if (!slot.IsEmpty())
            {
                Debug.Log($"Inventory Slot {i}: {slot.Item.ItemName} x{slot.Quantity}");
            }
        }
        
        for (int i = 0; i < _equipmentSlotUIElements.Count; i++)
        {
            var slot = _inventory.GetEquipmentSlot(i);
            if (!slot.IsEmpty())
            {
                Debug.Log($"Equipment Slot {i}: {slot.Item.ItemName}");
            }
        }
        
        Debug.Log("========================");
    }
    
    // === Публичные свойства ===
    
    public bool IsOpen => _isOpen;
    public int SelectedSlotIndex => _selectedSlotIndex;
    public List<InventorySlotUI> SlotUIElements => _slotUIElements;
    public List<InventorySlotUI> EquipmentSlotUIElements => _equipmentSlotUIElements;
}