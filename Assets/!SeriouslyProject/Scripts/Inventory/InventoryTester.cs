using UnityEngine;
using Zenject;

/// <summary>
/// Тестовый скрипт для общей функциональности инвентаря.
/// </summary>
public class InventoryTester : MonoBehaviour
{
    [Header("Тестовые предметы")]
    [SerializeField] private Item[] _testItems;

    [Header("Настройки теста")]
    [SerializeField] private KeyCode _toggleInventoryKey = KeyCode.Tab;
    [SerializeField] private KeyCode _addRandomItemKey = KeyCode.Q;
    [SerializeField] private KeyCode _clearInventoryKey = KeyCode.C;

    private Inventory _inventory;
    private InventoryUI _inventoryUI;

    [Inject]
    private void Construct(Inventory inventory, InventoryUI inventoryUI)
    {
        _inventory = inventory;
        _inventoryUI = inventoryUI;
    }

    private void Start()
    {
        AddTestItems();
    }

    private void Update()
    {
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
            _inventory.ClearInventory();
        }
    }

    private void AddRandomItem()
    {
        if (_testItems == null || _testItems.Length == 0) return;

        Item randomItem = _testItems[Random.Range(0, _testItems.Length)];
        int randomQuantity = randomItem.IsStackable ? Random.Range(1, 11) : 1;
        
        _inventory.AddItem(randomItem, randomQuantity);
    }

    private void AddTestItems()
    {
        if (_testItems == null || _testItems.Length == 0) return;

        foreach (Item item in _testItems)
        {
            if (item != null)
            {
                int quantityToAdd = item.IsStackable ? Random.Range(1, 6) : 1;
                _inventory.AddItem(item, quantityToAdd);
            }
        }
    }
}