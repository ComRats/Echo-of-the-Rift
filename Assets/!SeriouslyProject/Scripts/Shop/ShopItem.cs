using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class ShopItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI stockText;
    [SerializeField] private TextMeshProUGUI priceText;

    private ItemData itemData;
    private ShopItemEntry shopEntry;
    private ShopManager shopManager;
    private ShopContextMenu contextMenu;

    public ItemData ItemData => itemData;
    public ShopItemEntry ShopEntry => shopEntry;

    public void Initialize(ShopItemEntry entry, ShopManager manager, ShopContextMenu menu)
    {
        shopEntry = entry;
        itemData = entry.item;
        shopManager = manager;
        contextMenu = menu;

        UpdateVisuals();
    }

    public void UpdateVisuals()
    {
        if (itemData == null) return;

        if (iconImage != null)
        {
            iconImage.sprite = itemData.icon;
        }

        if (priceText != null)
        {
            priceText.text = $"{itemData.itemPrice}";
        }

        if (stockText != null)
        {
            if (shopEntry.stock == -1)
            {
                stockText.text = "∞";
            }
            else
            {
                stockText.text = shopEntry.currentStock.ToString();
                stockText.gameObject.SetActive(shopEntry.currentStock > 0);
            }
        }

        if (iconImage != null && !shopEntry.HasStock)
        {
            iconImage.color = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        }
        else if (iconImage != null)
        {
            iconImage.color = Color.white;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (contextMenu != null && shopEntry.HasStock)
            {
                contextMenu.ShowBuyMenu(this, eventData.position);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (shopManager != null && itemData != null)
        {
            shopManager.ShowItemDescription(itemData, true);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (shopManager != null)
        {
            shopManager.HideItemDescription();
        }
    }
}