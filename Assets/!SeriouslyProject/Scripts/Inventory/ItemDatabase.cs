using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// База данных всех предметов в игре
/// Должна находиться в папке Resources для доступа через Resources.Load
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Inventory/Item Database")]
public class ItemDatabase : ScriptableObject
{
    [SerializeField] private List<Item> _allItems = new List<Item>();
    private Dictionary<int, Item> _itemLookup;

    private static ItemDatabase _instance;
    
    /// <summary>
    /// Singleton instance доступная из любого места
    /// </summary>
    public static ItemDatabase Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<ItemDatabase>("ItemDatabase");
                if (_instance == null)
                {
                    Debug.LogError("ItemDatabase not found in Resources folder! " +
                                 "Create it via: Assets → Create → Inventory → Item Database " +
                                 "and place it in Resources folder.");
                }
                else
                {
                    _instance.Initialize();
                }
            }
            return _instance;
        }
    }

    private void OnEnable()
    {
        Initialize();
    }

    /// <summary>
    /// Инициализирует словарь для быстрого поиска предметов по ID
    /// </summary>
    private void Initialize()
    {
        _itemLookup = new Dictionary<int, Item>();
        
        foreach (var item in _allItems)
        {
            if (item != null)
            {
                if (_itemLookup.ContainsKey(item.ID))
                {
                    Debug.LogWarning($"Duplicate item ID found: {item.ID} for item {item.ItemName}. " +
                                   "This may cause issues with save/load system!");
                }
                else
                {
                    _itemLookup[item.ID] = item;
                }
            }
        }
        
        Debug.Log($"ItemDatabase initialized with {_itemLookup.Count} items");
    }

    /// <summary>
    /// Получить предмет по ID
    /// </summary>
    /// <param name="id">Уникальный идентификатор предмета</param>
    /// <returns>Предмет или null если не найден</returns>
    public Item GetItemById(int id)
    {
        if (_itemLookup == null)
        {
            Initialize();
        }
        
        _itemLookup.TryGetValue(id, out Item item);
        
        if (item == null)
        {
            Debug.LogWarning($"Item with ID {id} not found in ItemDatabase!");
        }
        
        return item;
    }

    /// <summary>
    /// Получить все предметы определенного типа
    /// </summary>
    public List<Item> GetItemsByType(ItemType itemType)
    {
        var result = new List<Item>();
        foreach (var item in _allItems)
        {
            if (item != null && item.ItemType == itemType)
            {
                result.Add(item);
            }
        }
        return result;
    }

    /// <summary>
    /// Получить все предметы определенной редкости
    /// </summary>
    public List<Item> GetItemsByRarity(ItemRarity rarity)
    {
        var result = new List<Item>();
        foreach (var item in _allItems)
        {
            if (item != null && item.Rarity == rarity)
            {
                result.Add(item);
            }
        }
        return result;
    }

    /// <summary>
    /// Получить все предметы экипировки определенного подтипа
    /// </summary>
    public List<Item> GetItemsByEquipmentSubtype(EquipmentSubtype subtype)
    {
        var result = new List<Item>();
        foreach (var item in _allItems)
        {
            if (item != null && item.EquipmentSubtype == subtype)
            {
                result.Add(item);
            }
        }
        return result;
    }

    /// <summary>
    /// Проверить существование предмета по ID
    /// </summary>
    public bool HasItem(int id)
    {
        if (_itemLookup == null)
        {
            Initialize();
        }
        return _itemLookup.ContainsKey(id);
    }

    /// <summary>
    /// Получить все предметы
    /// </summary>
    public List<Item> GetAllItems()
    {
        return new List<Item>(_allItems);
    }

    /// <summary>
    /// Получить количество предметов в базе
    /// </summary>
    public int GetItemCount()
    {
        return _allItems.Count;
    }

#if UNITY_EDITOR
    /// <summary>
    /// Добавить новый предмет в базу данных (только в редакторе)
    /// </summary>
    public void AddItem(Item item)
    {
        if (item == null)
        {
            Debug.LogWarning("Cannot add null item to database!");
            return;
        }
        
        if (_itemLookup != null && _itemLookup.ContainsKey(item.ID))
        {
            Debug.LogWarning($"Item with ID {item.ID} already exists in database!");
            return;
        }
        
        _allItems.Add(item);
        
        if (_itemLookup != null)
        {
            _itemLookup[item.ID] = item;
        }
        
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"Added item {item.ItemName} (ID: {item.ID}) to database");
    }

    /// <summary>
    /// Удалить предмет из базы данных (только в редакторе)
    /// </summary>
    public void RemoveItem(Item item)
    {
        if (item == null) return;
        
        _allItems.Remove(item);
        
        if (_itemLookup != null && _itemLookup.ContainsKey(item.ID))
        {
            _itemLookup.Remove(item.ID);
        }
        
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"Removed item {item.ItemName} from database");
    }

    /// <summary>
    /// Автоматически найти все предметы в проекте (только в редакторе)
    /// </summary>
    [ContextMenu("Auto-populate from project")]
    public void AutoPopulateFromProject()
    {
        _allItems.Clear();
        
        string[] guids = UnityEditor.AssetDatabase.FindAssets("t:Item");
        foreach (string guid in guids)
        {
            string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
            Item item = UnityEditor.AssetDatabase.LoadAssetAtPath<Item>(path);
            if (item != null)
            {
                _allItems.Add(item);
            }
        }
        
        Initialize();
        UnityEditor.EditorUtility.SetDirty(this);
        Debug.Log($"Found and added {_allItems.Count} items to database");
    }
    
    /// <summary>
    /// Проверить на дублирующиеся ID
    /// </summary>
    [ContextMenu("Check for duplicate IDs")]
    public void CheckForDuplicateIDs()
    {
        var idSet = new HashSet<int>();
        var duplicates = new List<Item>();
        
        foreach (var item in _allItems)
        {
            if (item != null)
            {
                if (idSet.Contains(item.ID))
                {
                    duplicates.Add(item);
                }
                else
                {
                    idSet.Add(item.ID);
                }
            }
        }
        
        if (duplicates.Count > 0)
        {
            Debug.LogWarning($"Found {duplicates.Count} items with duplicate IDs:");
            foreach (var item in duplicates)
            {
                Debug.LogWarning($"- {item.ItemName} (ID: {item.ID})");
            }
        }
        else
        {
            Debug.Log("✓ No duplicate IDs found!");
        }
    }

    /// <summary>
    /// Проверить на пустые ID (ID = 0)
    /// </summary>
    [ContextMenu("Check for empty IDs")]
    public void CheckForEmptyIDs()
    {
        var emptyIdItems = new List<Item>();
        
        foreach (var item in _allItems)
        {
            if (item != null && item.ID == 0)
            {
                emptyIdItems.Add(item);
            }
        }
        
        if (emptyIdItems.Count > 0)
        {
            Debug.LogWarning($"Found {emptyIdItems.Count} items with ID = 0:");
            foreach (var item in emptyIdItems)
            {
                Debug.LogWarning($"- {item.ItemName}");
            }
        }
        else
        {
            Debug.Log("✓ No items with empty IDs found!");
        }
    }

    /// <summary>
    /// Автоматически назначить уникальные ID всем предметам
    /// </summary>
    [ContextMenu("Auto-assign unique IDs")]
    public void AutoAssignUniqueIDs()
    {
        int nextId = 1;
        var usedIds = new HashSet<int>();
        
        // Собираем уже используемые ID
        foreach (var item in _allItems)
        {
            if (item != null && item.ID > 0)
            {
                usedIds.Add(item.ID);
            }
        }
        
        // Назначаем ID предметам, у которых их нет
        foreach (var item in _allItems)
        {
            if (item != null && item.ID == 0)
            {
                // Находим первый свободный ID
                while (usedIds.Contains(nextId))
                {
                    nextId++;
                }
                
                // Используем SetEditorData для установки ID
                var serializedObject = new UnityEditor.SerializedObject(item);
                serializedObject.FindProperty("_id").intValue = nextId;
                serializedObject.ApplyModifiedProperties();
                
                usedIds.Add(nextId);
                Debug.Log($"Assigned ID {nextId} to {item.ItemName}");
                nextId++;
            }
        }
        
        Initialize();
        UnityEditor.EditorUtility.SetDirty(this);
        UnityEditor.AssetDatabase.SaveAssets();
        Debug.Log("✓ Unique IDs assigned successfully!");
    }

    /// <summary>
    /// Показать статистику базы данных
    /// </summary>
    [ContextMenu("Show Database Statistics")]
    public void ShowStatistics()
    {
        if (_allItems.Count == 0)
        {
            Debug.Log("Database is empty!");
            return;
        }
        
        var typeCount = new Dictionary<ItemType, int>();
        var rarityCount = new Dictionary<ItemRarity, int>();
        
        foreach (var item in _allItems)
        {
            if (item == null) continue;
            
            if (!typeCount.ContainsKey(item.ItemType))
                typeCount[item.ItemType] = 0;
            typeCount[item.ItemType]++;
            
            if (!rarityCount.ContainsKey(item.Rarity))
                rarityCount[item.Rarity] = 0;
            rarityCount[item.Rarity]++;
        }
        
        Debug.Log("=== Item Database Statistics ===");
        Debug.Log($"Total items: {_allItems.Count}");
        Debug.Log("\nBy Type:");
        foreach (var kvp in typeCount)
        {
            Debug.Log($"  {kvp.Key}: {kvp.Value}");
        }
        Debug.Log("\nBy Rarity:");
        foreach (var kvp in rarityCount)
        {
            Debug.Log($"  {kvp.Key}: {kvp.Value}");
        }
    }
#endif
}