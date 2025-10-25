using UnityEngine;

public class InventoryTester : MonoBehaviour
{
    [Header("Test Items")]
    [SerializeField] private Item[] _testItems; // Массив тестовых предметов

    [Header("References")]
    [SerializeField] private Inventory _inventory;
    [SerializeField] private InventoryUI _inventoryUI;

    [Header("Test Settings")]
    [SerializeField] private KeyCode _toggleInventoryKey = KeyCode.Tab;
    [SerializeField] private KeyCode _addRandomItemKey = KeyCode.Q;
    [SerializeField] private KeyCode _clearInventoryKey = KeyCode.C;
    [SerializeField] private KeyCode _debugKey = KeyCode.F1;
    [SerializeField] private KeyCode _pigKey = KeyCode.J;


    private void Start()
    {
        // Автоматически находим компоненты, если не назначены
        if (_inventory == null)
            _inventory = FindObjectOfType<Inventory>();

        if (_inventoryUI == null)
            _inventoryUI = FindObjectOfType<InventoryUI>();

        // Проверяем, что всё найдено
        if (_inventory == null)
        {
            Debug.LogError("Inventory not found! Make sure there's an Inventory component in the scene.");
            return;
        }

        if (_inventoryUI == null)
        {
            Debug.LogError("InventoryUI not found! Make sure there's an InventoryUI component in the scene.");
            return;
        }

        Debug.Log("InventoryTester initialized successfully!");
        Debug.Log($"Controls: {_toggleInventoryKey} - Toggle Inventory, {_addRandomItemKey} - Add Random Item, {_clearInventoryKey} - Clear Inventory, {_debugKey} - Debug UI");

        // Добавляем несколько тестовых предметов при старте
        AddTestItems();
    }

    private void Update()
    {
        // Обработка клавиш
        if (Input.GetKeyDown(_toggleInventoryKey))
        {
            _inventoryUI.ToggleInventory();
        }

        if (Input.GetKeyDown(_addRandomItemKey))
        {
            AddRandomItem();
        }

        if (Input.GetKeyDown(_clearInventoryKey))
        {
            ClearInventory();
        }

        if (Input.GetKeyDown(_debugKey))
        {
            DebugUI();
        }

        if (Input.GetKeyDown(_pigKey))
        {
            DebugUI();
        }
    }

    private void AddPig()
    {
        // Не трожь
    }
    
    // Добавить случайный предмет
    private void AddRandomItem()
    {
        if (_testItems == null || _testItems.Length == 0)
        {
            Debug.LogWarning("No test items assigned!");
            return;
        }

        // Выбираем случайный предмет
        Item randomItem = _testItems[Random.Range(0, _testItems.Length)];

        // Определяем количество в зависимости от типа предмета
        int randomQuantity;

        if (randomItem.IsStackable)
        {
            // Для стакаемых предметов - случайное количество от 1 до 10
            randomQuantity = Random.Range(1, 11);
        }
        else
        {
            // Для нестакаемых предметов - всегда 1
            randomQuantity = 1;
        }

        bool success = _inventory.AddItem(randomItem, randomQuantity);

        if (success)
        {
            string stackInfo = randomItem.IsStackable ? $" (stackable, max: {randomItem.MaxStackSize})" : " (non-stackable)";
            Debug.Log($"Added {randomQuantity}x {randomItem.ItemName}{stackInfo} to inventory");
        }
        else
        {
            Debug.Log($"Failed to add {randomItem.ItemName} - inventory might be full");
        }
    }
    
    // Очистить инвентарь
    private void ClearInventory()
    {
        for (int i = 0; i < _inventory.Size; i++)
        {
            InventorySlot slot = _inventory.GetSlot(i);
            if (!slot.IsEmpty())
            {
                slot.Clear();
            }
        }
        
        Debug.Log("Inventory cleared!");
    }
    
    // Добавить несколько тестовых предметов при старте
    private void AddTestItems()
    {
        if (_testItems == null || _testItems.Length == 0)
        {
            Debug.LogWarning("No test items to add at start!");
            return;
        }
        
        // Добавляем по одному предмету каждого типа
        foreach (Item item in _testItems)
        {
            if (item != null)
            {
                int quantityToAdd;
                
                if (item.IsStackable)
                {
                    // Для стакаемых предметов - небольшое случайное количество
                    quantityToAdd = Random.Range(1, 6);
                }
                else
                {
                    // Для нестакаемых предметов - только 1
                    quantityToAdd = 1;
                }
                
                _inventory.AddItem(item, quantityToAdd);
                
                string stackInfo = item.IsStackable ? $" (x{quantityToAdd})" : " (non-stackable)";
                Debug.Log($"Added test item: {item.ItemName}{stackInfo}");
            }
        }
    }
    
    // Методы для кнопок в UI (можно вызывать из Inspector)
    public void AddRandomItemButton()
    {
        AddRandomItem();
    }
    
    public void ClearInventoryButton()
    {
        ClearInventory();
    }
    
    public void ToggleInventoryButton()
    {
        _inventoryUI.ToggleInventory();
    }
    
    // Информация о состоянии инвентаря для отладки
    public void PrintInventoryInfo()
    {
        Debug.Log("=== INVENTORY INFO ===");
        Debug.Log($"Inventory size: {_inventory.Size}");
        
        int usedSlots = 0;
        int totalItems = 0;
        
        for (int i = 0; i < _inventory.Size; i++)
        {
            InventorySlot slot = _inventory.GetSlot(i);
            if (!slot.IsEmpty())
            {
                usedSlots++;
                totalItems += slot.Quantity;
                Debug.Log($"Slot {i}: {slot.Quantity}x {slot.Item.ItemName}");
            }
        }
        
        Debug.Log($"Used slots: {usedSlots}/{_inventory.Size}");
        Debug.Log($"Total items: {totalItems}");
        Debug.Log("=====================");
    }
    
    // Отладка UI
    private void DebugUI()
    {
        Debug.Log("=== UI DEBUG ===");
        PrintInventoryInfo();
        //_inventoryUI.ForceRefreshAllSlots();
    }
}