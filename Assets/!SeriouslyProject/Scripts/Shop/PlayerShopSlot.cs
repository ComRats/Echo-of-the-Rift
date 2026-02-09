using UnityEngine;

public class PlayerShopSlot : MonoBehaviour
{
    private PlayerShopItem currentItem;

    public PlayerShopItem CurrentItem => currentItem;
    public bool IsEmpty => currentItem == null;

    public void SetItem(PlayerShopItem item)
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