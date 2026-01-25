using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public ItemType allowedType = ItemType.Generic;

    [SerializeField] private InventoryManager inventoryManager;

    public void OnDrop(PointerEventData eventData)
    {
        if (inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager не назначен в InventorySlot!");
            return;
        }

        GameObject dropped = eventData.pointerDrag;
        DraggableItem draggableItem = dropped.GetComponent<DraggableItem>();
        
        if (draggableItem == null || !IsTypeAllowed(draggableItem)) return;

        if (transform.childCount == 0)
        {
            draggableItem.parentAfterDrag = transform;
        }
        else
        {
            DraggableItem currentItem = transform.GetComponentInChildren<DraggableItem>();

            if (CanStackItems(draggableItem, currentItem))
            {
                ProcessStackItems(currentItem, draggableItem);
                return;
            }

            SwapItems(draggableItem, currentItem);
        }
    }

    private bool IsTypeAllowed(DraggableItem item)
    {
        if (allowedType == ItemType.Generic) return true;
        return item.itemData.itemType == allowedType;
    }

    private bool CanStackItems(DraggableItem item1, DraggableItem item2)
    {
        return item1.itemData == item2.itemData &&
               item1.itemData.isStackable &&
               item1.itemData.maxStackSize > item2.count;
    }

    private void ProcessStackItems(DraggableItem currentItem, DraggableItem newItem)
    {
        int total = currentItem.count + newItem.count;
        int maxStack = currentItem.itemData.maxStackSize;
        
        if (total <= maxStack)
        {
            currentItem.count = total;
            currentItem.RefreshCount();
            Destroy(newItem.gameObject);
        }
        else
        {
            currentItem.count = maxStack;
            currentItem.RefreshCount();
            newItem.count = total - maxStack;
            newItem.RefreshCount();
            TryMoveItemToEmptySlot(newItem);
        }
    }
    
    private void TryMoveItemToEmptySlot(DraggableItem item)
    {
        if (inventoryManager == null) return;
        
        foreach (InventorySlot slot in inventoryManager.inventorySlots)
        {
            if (slot.transform.childCount == 0)
            {
                item.parentAfterDrag = slot.transform;
                item.transform.SetParent(slot.transform);
                item.transform.localPosition = Vector3.zero;
                return;
            }
        }
    }

    private void SwapItems(DraggableItem newItem, DraggableItem oldItem)
    {
        Transform newItemOriginalParent = newItem.parentAfterDrag;
        Transform oldItemOriginalParent = oldItem.transform.parent;

        newItem.parentAfterDrag = oldItemOriginalParent;
        oldItem.parentAfterDrag = newItemOriginalParent;
        
        oldItem.transform.SetParent(newItemOriginalParent);
        oldItem.transform.localPosition = Vector3.zero;
    }
}