using UnityEngine;

public class ShopSlot : MonoBehaviour
{
    private ShopItem currentItem;

    public ShopItem CurrentItem => currentItem;
    public bool IsEmpty => currentItem == null;

    public void SetItem(ShopItem item)
    {
        currentItem = item;

        if (item != null)
        {
            item.transform.SetParent(transform);
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
        }
    }

    public void Clear()
    {
        if (currentItem != null)
        {
            Destroy(currentItem.gameObject);
            currentItem = null;
        }
    }
}