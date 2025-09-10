using UnityEngine;

public class DragDropTester : MonoBehaviour
{
    [Header("Test Settings")]
    [SerializeField] private KeyCode _addEquippableItemKey = KeyCode.E;
    [SerializeField] private KeyCode _addWeaponKey = KeyCode.W;
    [SerializeField] private KeyCode _addArmorKey = KeyCode.R;
    [SerializeField] private KeyCode _debugDragDropKey = KeyCode.F2;
    
    [Header("Test Equipment Items")]
    [SerializeField] private Item[] _testWeapons;
    [SerializeField] private Item[] _testArmor;
    [SerializeField] private Item[] _testEquipment;
    
    [Header("References")]
    [SerializeField] private Inventory _inventory;
    [SerializeField] private InventoryUI _inventoryUI;
    
    private void Start()
    {
        // Автоматически находим компоненты
        if (_inventory == null)
            _inventory = FindObjectOfType<Inventory>();
            
        if (_inventoryUI == null)
            _inventoryUI = FindObjectOfType<InventoryUI>();
        
        if (_inventory == null)
        {
            Debug.LogError("Inventory not found!");
            return;
        }
        
        if (_inventoryUI == null)
        {
            Debug.LogError("InventoryUI not found!");
            return;
        }
        
        Debug.Log("=== DRAG & DROP SYSTEM CONTROLS ===");
        Debug.Log($"{_addEquippableItemKey} - Add Random Equipment Item");
        Debug.Log($"{_addWeaponKey} - Add Random Weapon");
        Debug.Log($"{_addArmorKey} - Add Random Armor");
        Debug.Log($"{_debugDragDropKey} - Debug Drag & Drop State");
        Debug.Log("MOUSE - Left click and drag to move items");
        Debug.Log("RIGHT CLICK - Quick use/unequip items");
        Debug.Log("===================================");
    }
    
    private void Update()
    {
        if (Input.GetKeyDown(_addEquippableItemKey))
        {
            AddRandomEquippableItem();
        }
        
        if (Input.GetKeyDown(_addWeaponKey))
        {
            AddRandomWeapon();
        }
        
        if (Input.GetKeyDown(_addArmorKey))
        {
            AddRandomArmor();
        }
        
        if (Input.GetKeyDown(_debugDragDropKey))
        {
            DebugDragDropState();
        }
    }
    
    private void AddRandomEquippableItem()
    {
        if (_testEquipment == null || _testEquipment.Length == 0)
        {
            Debug.LogWarning("No test equipment items assigned!");
            return;
        }
        
        Item randomEquipment = _testEquipment[Random.Range(0, _testEquipment.Length)];
        bool success = _inventory.AddItem(randomEquipment, 1);
        
        if (success)
        {
            Debug.Log($"Added equipment item: {randomEquipment.ItemName} (Type: {randomEquipment.ItemType})");
            Debug.Log("Try dragging it to an equipment slot!");
        }
        else
        {
            Debug.Log("Failed to add equipment item - inventory might be full");
        }
    }
    
    private void AddRandomWeapon()
    {
        if (_testWeapons == null || _testWeapons.Length == 0)
        {
            Debug.LogWarning("No test weapons assigned!");
            return;
        }
        
        Item randomWeapon = _testWeapons[Random.Range(0, _testWeapons.Length)];
        bool success = _inventory.AddItem(randomWeapon, 1);
        
        if (success)
        {
            Debug.Log($"Added weapon: {randomWeapon.ItemName} (Type: {randomWeapon.ItemType})");
            Debug.Log("Try dragging it to an equipment slot!");
        }
        else
        {
            Debug.Log("Failed to add weapon - inventory might be full");
        }
    }
    
    private void AddRandomArmor()
    {
        if (_testArmor == null || _testArmor.Length == 0)
        {
            Debug.LogWarning("No test armor assigned!");
            return;
        }
        
        Item randomArmor = _testArmor[Random.Range(0, _testArmor.Length)];
        bool success = _inventory.AddItem(randomArmor, 1);
        
        if (success)
        {
            Debug.Log($"Added armor: {randomArmor.ItemName} (Type: {randomArmor.ItemType})");
            Debug.Log("Try dragging it to an equipment slot!");
        }
        else
        {
            Debug.Log("Failed to add armor - inventory might be full");
        }
    }
    
    private void DebugDragDropState()
    {
        Debug.Log("=== DRAG & DROP DEBUG ===");
        
        // Информация об инвентаре
        Debug.Log($"Inventory Size: {_inventory.Size}");
        Debug.Log($"Equipment Slots: {_inventory.EquipmentSize}");
        
        // Проверяем содержимое обычных слотов
        int equippableItemsCount = 0;
        for (int i = 0; i < _inventory.Size; i++)
        {
            var slot = _inventory.GetSlot(i);
            if (!slot.IsEmpty())
            {
                bool canEquip = slot.Item.ItemType == ItemType.Equipment || 
                               slot.Item.ItemType == ItemType.Weapon || 
                               slot.Item.ItemType == ItemType.Armor;
                               
                Debug.Log($"Slot {i}: {slot.Item.ItemName} (Type: {slot.Item.ItemType}, Can Equip: {canEquip})");
                
                if (canEquip) equippableItemsCount++;
            }
        }
        
        // Проверяем содержимое слотов экипировки
        int equippedItemsCount = 0;
        for (int i = 0; i < _inventory.EquipmentSize; i++)
        {
            var slot = _inventory.GetEquipmentSlot(i);
            if (!slot.IsEmpty())
            {
                Debug.Log($"Equipment Slot {i}: {slot.Item.ItemName} (Type: {slot.Item.ItemType})");
                equippedItemsCount++;
            }
        }
        
        Debug.Log($"Summary: {equippableItemsCount} equippable items in inventory, {equippedItemsCount} items equipped");
        
        // Инструкции по использованию
        if (equippableItemsCount > 0)
        {
            Debug.Log("INSTRUCTIONS: Left-click and drag equippable items to equipment slots!");
        }
        else
        {
            Debug.Log($"INSTRUCTIONS: Press {_addEquippableItemKey}, {_addWeaponKey}, or {_addArmorKey} to add equippable items first!");
        }
        
        if (equippedItemsCount > 0)
        {
            Debug.Log("You can also drag equipped items back to inventory or right-click to quickly unequip!");
        }
        
        Debug.Log("========================");
    }
    
    // Методы для кнопок UI (если нужно)
    public void AddRandomEquippableItemButton()
    {
        AddRandomEquippableItem();
    }
    
    public void AddRandomWeaponButton()
    {
        AddRandomWeapon();
    }
    
    public void AddRandomArmorButton()
    {
        AddRandomArmor();
    }
    
    public void DebugDragDropStateButton()
    {
        DebugDragDropState();
    }
    
    // Метод для создания тестовых предметов программно (если нет ScriptableObject'ов)
    [ContextMenu("Create Test Equipment Items")]
    private void CreateTestEquipmentItems()
    {
        Debug.Log("Creating test equipment items programmatically...");
        
        // Этот метод поможет, если у вас нет готовых ScriptableObject'ов для тестирования
        // В реальном проекте лучше создавать предметы через CreateAssetMenu
        
        Debug.Log("Please create equipment items using 'Create > Inventory > Item' in the Project window");
        Debug.Log("Set their ItemType to Equipment, Weapon, or Armor to make them equippable");
    }
}