using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// ScriptableObject, служащий центральной базой данных для всех предметов в игре.
/// </summary>
[CreateAssetMenu(fileName = "База данных предметов", menuName = "Инвентарь/База данных предметов")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<Item> _items = new List<Item>();

    private Dictionary<int, Item> _itemLookup;
    private Dictionary<ItemType, List<Item>> _itemsByType;
    private Dictionary<ItemRarity, List<Item>> _itemsByRarity;
    
    /// <summary>
    /// Возвращает список всех предметов только для чтения.
    /// </summary>
    public IReadOnlyList<Item> AllItems => _items;

    private void OnEnable()
    {
        Initialize();
    }

    /// <summary>
    /// Инициализирует базу данных, создавая словари для быстрого доступа.
    /// </summary>
    public void Initialize()
    {
        _itemLookup = new Dictionary<int, Item>();
        _itemsByType = new Dictionary<ItemType, List<Item>>();
        _itemsByRarity = new Dictionary<ItemRarity, List<Item>>();

        foreach (var item in _items)
        {
            if (item == null) continue;

            // Initialize lookup by ID
            if (_itemLookup.ContainsKey(item.Id))
            {
                Debug.LogWarning($"[База данных] Найден дубликат ID '{item.Id}' для предмета '{item.ItemName}'.");
            }
            else
            {
                _itemLookup[item.Id] = item;
            }

            // Initialize lookup by type
            if (!_itemsByType.ContainsKey(item.ItemType))
            {
                _itemsByType[item.ItemType] = new List<Item>();
            }
            _itemsByType[item.ItemType].Add(item);

            // Initialize lookup by rarity
            if (!_itemsByRarity.ContainsKey(item.Rarity))
            {
                _itemsByRarity[item.Rarity] = new List<Item>();
            }
            _itemsByRarity[item.Rarity].Add(item);
        }
        
        Debug.Log($"[База данных] Инициализировано {_itemLookup.Count} уникальных предметов.");
    }

    /// <summary>
    /// Получает предмет из базы данных по его ID.
    /// </summary>
    public Item GetItemById(int id)
    {
        if (_itemLookup == null)
        {
            Initialize();
        }
        
        _itemLookup.TryGetValue(id, out Item item);
        
        if (item == null)
        {
            Debug.LogWarning($"[База данных] Предмет с ID {id} не найден!");
        }
        
        return item;
    }

    /// <summary>
    /// Получает все предметы указанного типа.
    /// </summary>
    public IReadOnlyList<Item> GetItemsByType(ItemType itemType)
    {
        if (_itemsByType == null) Initialize();
        
        return _itemsByType.TryGetValue(itemType, out var items) ? items : new List<Item>();
    }

    /// <summary>
    /// Получает все предметы указанной редкости.
    /// </summary>
    public IReadOnlyList<Item> GetItemsByRarity(ItemRarity rarity)
    {
        if (_itemsByRarity == null) Initialize();

        return _itemsByRarity.TryGetValue(rarity, out var items) ? items : new List<Item>();
    }
    
    /// <summary>
    /// Получает все предметы, совместимые с указанным подтипом экипировки.
    /// </summary>
    public IEnumerable<Item> GetItemsByEquipmentSubtype(EquipmentSubtype subtype)
    {
        return _items.Where(item => item != null && item.EquipmentSubtype == subtype);
    }

    /// <summary>
    /// Проверяет, существует ли предмет с указанным ID.
    /// </summary>
    public bool HasItem(int id)
    {
        if (_itemLookup == null) Initialize();
        return _itemLookup.ContainsKey(id);
    }
}