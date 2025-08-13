using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections.Generic;

public class InventoryUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject _inventoryPanel;           // Основная панель инвентаря
    [SerializeField] private Transform _slotsContainer;            // Контейнер для слотов
    [SerializeField] private GameObject _slotPrefab;               // Префаб слота
    [SerializeField] private Button _closeButton;                 // Кнопка закрытия
    
    [Header("Animation Settings")]
    [SerializeField] private float _openAnimationDuration = 0.3f;
    [SerializeField] private float _closeAnimationDuration = 0.2f;
    
    // Приватные поля
    private Inventory _inventory;                    // Ссылка на данные инвентаря
    private List<InventorySlotUI> _slotUIElements;   // Список UI слотов
    private bool _isOpen = false;
    private int _selectedSlotIndex = -1;             // Выбранный слот (-1 = ничего не выбрано)
    
    // Кэшируем начальные параметры для анимации
    private Vector3 _originalScale;
    private CanvasGroup _canvasGroup;
    
    private void Awake()
    {
        // Инициализируем компоненты
        _slotUIElements = new List<InventorySlotUI>();
        _originalScale = _inventoryPanel.transform.localScale;
        
        // Добавляем CanvasGroup для анимации прозрачности
        _canvasGroup = _inventoryPanel.GetComponent<CanvasGroup>();
        if (_canvasGroup == null)
        {
            _canvasGroup = _inventoryPanel.AddComponent<CanvasGroup>();
        }
        
        // Подписываемся на кнопку закрытия
        if (_closeButton != null)
        {
            _closeButton.onClick.AddListener(CloseInventory);
        }
        
        // Инвентарь изначально закрыт
        _inventoryPanel.SetActive(false);
    }
    
    private void Start()
    {
        // Находим компонент инвентаря
        _inventory = FindObjectOfType<Inventory>();
        
        if (_inventory == null)
        {
            Debug.LogError("Inventory component not found!");
            return;
        }
        
        // Подписываемся на события инвентаря
        _inventory.OnSlotChanged += OnSlotChanged;
        _inventory.OnInventoryChanged += OnInventoryChanged;
        
        // Создаём UI слоты
        CreateSlots();
    }
    
    private void OnDestroy()
    {
        // Отписываемся от событий при уничтожении объекта
        if (_inventory != null)
        {
            _inventory.OnSlotChanged -= OnSlotChanged;
            _inventory.OnInventoryChanged -= OnInventoryChanged;
        }
    }
    
    // Создание UI слотов
    private void CreateSlots()
    {
        // Очищаем существующие слоты (если есть)
        foreach (Transform child in _slotsContainer)
        {
            Destroy(child.gameObject);
        }
        _slotUIElements.Clear();
        
        // Создаём слоты по количеству в инвентаре
        for (int i = 0; i < _inventory.Size; i++)
        {
            GameObject slotObject = Instantiate(_slotPrefab, _slotsContainer);
            InventorySlotUI slotUI = slotObject.GetComponent<InventorySlotUI>();
            
            if (slotUI == null)
            {
                Debug.LogError($"Slot prefab must have InventorySlotUI component!");
                continue;
            }
            
            // Инициализируем слот
            slotUI.Initialize(i, _inventory.GetSlot(i));
            
            // Подписываемся на события слота
            slotUI.OnSlotClicked += OnSlotClicked;
            slotUI.OnSlotRightClicked += OnSlotRightClicked;
            slotUI.OnSlotHovered += OnSlotHovered;
            
            _slotUIElements.Add(slotUI);
        }
        
        Debug.Log($"Created {_slotUIElements.Count} inventory slots");
    }
    
    // Обработка события изменения конкретного слота
    private void OnSlotChanged(int slotIndex)
    {
        if (slotIndex >= 0 && slotIndex < _slotUIElements.Count)
        {
            _slotUIElements[slotIndex].UpdateSlotVisuals();
        }
    }
    
    // Обработка общего события изменения инвентаря
    private void OnInventoryChanged()
    {
        // Можно добавить общие эффекты или звуки
        // Пока просто обновляем все слоты (на случай кардинальных изменений)
        RefreshAllSlots();
    }
    
    // Обновление всех слотов
    private void RefreshAllSlots()
    {
        for (int i = 0; i < _slotUIElements.Count; i++)
        {
            _slotUIElements[i].UpdateSlotVisuals();
        }
    }
    
    // Обработка клика по слоту
    private void OnSlotClicked(int slotIndex)
    {
        Debug.Log($"Slot {slotIndex} clicked");
        
        // Пока просто выделяем слот
        SelectSlot(slotIndex);
        
        // TODO: Здесь будет логика drag&drop
    }
    
    // Обработка правого клика по слоту
    private void OnSlotRightClicked(int slotIndex)
    {
        Debug.Log($"Slot {slotIndex} right clicked");
        
        // TODO: Контекстное меню или быстрое использование предмета
    }
    
    // Обработка наведения на слот
    private void OnSlotHovered(int slotIndex)
    {
        // TODO: Показ tooltip с информацией о предмете
    }
    
    // Выделение слота
    private void SelectSlot(int slotIndex)
    {
        // Снимаем выделение с предыдущего слота
        if (_selectedSlotIndex >= 0 && _selectedSlotIndex < _slotUIElements.Count)
        {
            // TODO: Убрать визуальное выделение
        }
        
        _selectedSlotIndex = slotIndex;
        
        // Выделяем новый слот
        if (_selectedSlotIndex >= 0 && _selectedSlotIndex < _slotUIElements.Count)
        {
            // TODO: Добавить визуальное выделение
        }
    }
    
    // Публичные методы для управления инвентарём
    public void OpenInventory()
    {
        if (_isOpen) return;
        
        _isOpen = true;
        _inventoryPanel.SetActive(true);
        
        // Анимация появления
        _inventoryPanel.transform.localScale = Vector3.zero;
        _canvasGroup.alpha = 0f;
        
        Sequence openSequence = DOTween.Sequence();
        openSequence.Append(_inventoryPanel.transform.DOScale(_originalScale, _openAnimationDuration).SetEase(Ease.OutBack));
        openSequence.Join(_canvasGroup.DOFade(1f, _openAnimationDuration));
        
        Debug.Log("Inventory opened");
    }
    
    public void CloseInventory()
    {
        if (!_isOpen) return;
        
        _isOpen = false;
        
        // Анимация исчезновения
        Sequence closeSequence = DOTween.Sequence();
        closeSequence.Append(_inventoryPanel.transform.DOScale(0f, _closeAnimationDuration).SetEase(Ease.InBack));
        closeSequence.Join(_canvasGroup.DOFade(0f, _closeAnimationDuration));
        closeSequence.OnComplete(() => _inventoryPanel.SetActive(false));
        
        Debug.Log("Inventory closed");
    }
    
    public void ToggleInventory()
    {
        if (_isOpen)
        {
            CloseInventory();
        }
        else
        {
            OpenInventory();
        }
    }
    
    // Проверка состояния
    public bool IsOpen => _isOpen;
}