using UnityEngine;

[System.Flags]
public enum ItemType 
{ 
    None = 0,
    Food = 1 << 0,      // 1
    Potion = 1 << 1,    // 2
    Weapon = 1 << 2,    // 4
    Armor = 1 << 3,     // 8
    Amulet = 1 << 4,    // 16
    Helmet = 1 << 5     // 32
}

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public string itemGameName;
    public Sprite icon;
    
    [Header("Item Type")]
    [Tooltip("Можно выбрать несколько типов. Например: Food | Weapon для съедобного оружия")]
    public ItemType itemType;

    [Header("Stack Settings")]
    public bool isStackable = true;
    public int maxStackSize = 10;
}