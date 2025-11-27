using UnityEngine;
using Zenject;

/// <summary>
/// Тестовый скрипт для демонстрации и отладки функциональности перетаскивания в инвентаре.
/// </summary>
public class DragDropTester : MonoBehaviour
{
    [Header("Настройки теста")]
    [SerializeField] private KeyCode _addEquippableItemKey = KeyCode.E;
    [SerializeField] private KeyCode _addWeaponKey = KeyCode.W;
    [SerializeField] private KeyCode _addArmorKey = KeyCode.R;
    [SerializeField] private KeyCode _debugDragDropKey = KeyCode.F2;

    [Header("Тестовые предметы экипировки")]
    [SerializeField] private Item[] _testWeapons;
    [SerializeField] private Item[] _testArmor;
    [SerializeField] private Item[] _testEquipment;

    [SerializeField] private Inventory inventory;

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
        if (_testEquipment == null || _testEquipment.Length == 0) return;
        
        Item randomEquipment = _testEquipment[Random.Range(0, _testEquipment.Length)];
        inventory.AddItem(randomEquipment, 1);
    }
    
    private void AddRandomWeapon()
    {
        if (_testWeapons == null || _testWeapons.Length == 0) return;
        
        Item randomWeapon = _testWeapons[Random.Range(0, _testWeapons.Length)];
        inventory.AddItem(randomWeapon, 1);
    }
    
    private void AddRandomArmor()
    {
        if (_testArmor == null || _testArmor.Length == 0) return;
        
        Item randomArmor = _testArmor[Random.Range(0, _testArmor.Length)];
        inventory.AddItem(randomArmor, 1);
    }
    
    private void DebugDragDropState()
    {
        Debug.Log("=== DRAG & DROP DEBUG ===");
        // Добавьте сюда соответствующую отладочную информацию
    }
}