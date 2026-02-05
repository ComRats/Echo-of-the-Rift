using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public const ItemType AllTypes = ItemType.Food | ItemType.Potion | ItemType.Weapon | ItemType.Armor | ItemType.Amulet | ItemType.Helmet;

    public ItemType allowedType = AllTypes;

    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private ItemDescriptionDisplay descriptionDisplay;

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
                SyncAfterChange();
                return;
            }

            SwapItems(draggableItem, currentItem);
        }

        SyncAfterChange();
    }

    public bool IsTypeAllowed(DraggableItem item)
    {
        return (allowedType & item.itemData.itemType) != 0;
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
            if (slot.transform.childCount == 0 && slot.IsTypeAllowed(item))
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

        InventorySlot originalSlot = newItemOriginalParent.GetComponent<InventorySlot>();
        if (originalSlot != null && !originalSlot.IsTypeAllowed(oldItem))
        {
            return;
        }

        newItem.parentAfterDrag = oldItemOriginalParent;
        oldItem.parentAfterDrag = newItemOriginalParent;

        oldItem.transform.SetParent(newItemOriginalParent);
        oldItem.transform.localPosition = Vector3.zero;
    }

    private void SyncAfterChange()
    {
        if (inventoryManager != null)
        {
            Invoke(nameof(DoSync), 0.1f);
        }
    }

    private void DoSync()
    {
        inventoryManager.SyncFromUI();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (descriptionDisplay == null) return;

        DraggableItem item = GetComponentInChildren<DraggableItem>();

        if (item != null && item.itemData != null)
        {
            descriptionDisplay.ShowItem(item);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (descriptionDisplay != null)
        {
            descriptionDisplay.Hide();
        }
    }
}