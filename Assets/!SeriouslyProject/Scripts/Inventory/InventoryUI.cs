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

            slotUI.Initialize(i, _inventory.GetSlot(i));
            slotUI.OnSlotClicked += OnSlotClicked;
            slotUI.OnSlotRightClicked += OnSlotRightClicked;
            slotUI.OnSlotHovered += OnSlotHovered;
            
            _slotUIElements.Add(slotUI);
        }
        
        Debug.Log($"Created {_slotUIElements.Count} inventory slots");
    }

    // === Создание слотов экипировки ===
    private void CreateEquipmentSlots()
    {
        foreach (Transform child in _equipmentSlotsContainer)
            Destroy(child.gameObject);

        _equipmentSlotUIElements.Clear();

        // допустим, у тебя фиксированное число экипируемых слотов
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

            slotUI.Initialize(i, _inventory.GetEquipmentSlot(i));
            slotUI.OnSlotClicked += OnEquipmentSlotClicked;
            slotUI.OnSlotRightClicked += OnEquipmentSlotRightClicked;
            slotUI.OnSlotHovered += OnEquipmentSlotHovered;

            _equipmentSlotUIElements.Add(slotUI);
        }

        Debug.Log($"Created {_equipmentSlotUIElements.Count} equipment slots");
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
        // TODO: Логика экипировки
    }

    private void OnEquipmentSlotRightClicked(int slotIndex)
    {
        Debug.Log($"Equipment Slot {slotIndex} right clicked");
        // TODO: Снять/быстро снять предмет
    }

    private void OnEquipmentSlotHovered(int slotIndex)
    {
        // TODO: Подсказка по предмету
    }

    // === Обычные клики ===
    private void OnSlotClicked(int slotIndex)
    {
        Debug.Log($"Slot {slotIndex} clicked");
        SelectSlot(slotIndex);
    }
    
    private void OnSlotRightClicked(int slotIndex)
    {
        Debug.Log($"Slot {slotIndex} right clicked");
    }
    
    private void OnSlotHovered(int slotIndex)
    {
        // TODO: Tooltip
    }
    
    private void SelectSlot(int slotIndex)
    {
        _selectedSlotIndex = slotIndex;
    }

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
    }
    
    public void CloseInventory()
    {
        if (!_isOpen) return;
        
        _isOpen = false;
        
        DOTween.Sequence()
            .Append(_inventoryPanel.transform.DOScale(0f, _closeAnimationDuration).SetEase(Ease.InBack))
            .Join(_canvasGroup.DOFade(0f, _closeAnimationDuration))
            .OnComplete(() => _inventoryPanel.SetActive(false));
    }
    
    public void ToggleInventory()
    {
        if (_isOpen) CloseInventory();
        else OpenInventory();
    }
    
    public bool IsOpen => _isOpen;
}