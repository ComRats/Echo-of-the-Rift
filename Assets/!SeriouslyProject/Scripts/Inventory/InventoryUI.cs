using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;
using Zenject;

/// <summary>
/// Управляет UI инвентаря, включая отображение слотов и анимации окна.
/// </summary>
public class InventoryUI : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private Button _closeButton;
    [SerializeField] private GameObject _inventoryPanel;

    [Header("Контейнеры слотов")]
    [SerializeField] private Transform _inventorySlotsParent;
    [SerializeField] private Transform _equipmentSlotsParent;
    
    [Header("Настройки анимации")]
    [SerializeField] private float _openAnimationDuration = 0.3f;
    [SerializeField] private float _closeAnimationDuration = 0.2f;

    private Inventory _inventory;
    private List<InventorySlotUI> _slotUIElements = new List<InventorySlotUI>();
    private List<InventorySlotUI> _equipmentSlotUIElements = new List<InventorySlotUI>();
    
    private bool _isOpen = false;
    private Vector3 _originalScale;
    private CanvasGroup _canvasGroup;

    /// <summary>
    /// Открыто ли окно инвентаря.
    /// </summary>
    public bool IsOpen => _isOpen;

    [Inject]
    private void Construct(Inventory inventory)
    {
        _inventory = inventory;
    }

    private void Awake()
    {
        _originalScale = _inventoryPanel.transform.localScale;
        _canvasGroup = _inventoryPanel.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = _inventoryPanel.AddComponent<CanvasGroup>();
        }

        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(CloseInventory);
        }

        _inventoryPanel.SetActive(false);
    }

    private void OnEnable()
    {
        _inventory.OnInventoryChanged += RefreshAllSlots;
    }

    private void OnDisable()
    {
        _inventory.OnInventoryChanged -= RefreshAllSlots;
    }

    private void Start()
    {
        InitializeUI();
    }

    /// <summary>
    /// Инициализирует UI инвентаря.
    /// </summary>
    private void InitializeUI()
    {
        InitializeInventorySlots();
        InitializeEquipmentSlots();
        RefreshAllSlots();
    }
    
    private void InitializeInventorySlots()
    {
        if (_inventorySlotsParent == null) return;
        
        var preMadeSlots = _inventorySlotsParent.GetComponentsInChildren<InventorySlotUI>();
        for (int i = 0; i < preMadeSlots.Length && i < _inventory.Size; i++)
        {
            preMadeSlots[i].Initialize(i, _inventory.GetSlot(i), false);
            _slotUIElements.Add(preMadeSlots[i]);
        }
    }

    private void InitializeEquipmentSlots()
    {
        if (_equipmentSlotsParent == null) return;

        var preMadeSlots = _equipmentSlotsParent.GetComponentsInChildren<InventorySlotUI>();
        for (int i = 0; i < preMadeSlots.Length && i < _inventory.EquipmentSize; i++)
        {
            preMadeSlots[i].Initialize(i, _inventory.GetEquipmentSlot(i), true);
            _equipmentSlotUIElements.Add(preMadeSlots[i]);
        }
    }

    /// <summary>
    /// Обновляет все слоты для соответствия текущему состоянию инвентаря.
    /// </summary>
    public void RefreshAllSlots()
    {
        foreach (var slotUI in _slotUIElements)
        {
            slotUI.UpdateSlotVisuals();
        }
        foreach (var slotUI in _equipmentSlotUIElements)
        {
            slotUI.UpdateSlotVisuals();
        }
    }

    /// <summary>
    /// Открывает окно инвентаря с анимацией.
    /// </summary>
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

    /// <summary>
    /// Закрывает окно инвентаря с анимацией.
    /// </summary>
    public void CloseInventory()
    {
        if (!_isOpen) return;

        _isOpen = false;

        DOTween.Sequence()
            .Append(_inventoryPanel.transform.DOScale(0f, _closeAnimationDuration).SetEase(Ease.InBack))
            .Join(_canvasGroup.DOFade(0f, _closeAnimationDuration))
            .OnComplete(() => _inventoryPanel.SetActive(false));
    }
    
    /// <summary>
    /// Переключает видимость окна инвентаря.
    /// </summary>
    public void ToggleInventory()
    {
        if (_isOpen) 
            CloseInventory();
        else 
            OpenInventory();
    }
}