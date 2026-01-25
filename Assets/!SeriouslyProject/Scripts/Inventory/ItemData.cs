using UnityEngine;

public enum ItemType { Generic, Weapon, Armor, Amulet }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;
    public Sprite icon;
    public ItemType itemType;
    
    [Header("Stack Settings")]
    public bool isStackable = true;
    public int maxStackSize = 10;
}