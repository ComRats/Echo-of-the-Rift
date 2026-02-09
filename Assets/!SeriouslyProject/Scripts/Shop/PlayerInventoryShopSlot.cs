using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class PlayerShopItem : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("UI")]
    [SerializeField] private Image iconImage;
    [SerializeField] private TextMeshProUGUI countText;
    [SerializeField] private TextMeshProUGUI priceText;

    private ItemData itemData;
    private int slotIndex;
    private int count;
    private ShopManager shopManager;
    private ShopContextMenu contextMenu;

    public ItemData ItemData => itemData;
    public int SlotIndex => slotIndex;
    public int Count => count;

    public void Initialize(ItemData item, int index, int amount, ShopManager manager, ShopContextMenu menu)
    {
        itemData = item;
        slotIndex = index;
        count = amount;
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

        if (countText != null)
        {
            countText.text = count.ToString();
            countText.gameObject.SetActive(count > 1);
        }

        if (priceText != null)
        {
            int sellPrice = shopManager.ShopData.GetSellPrice(itemData);
            priceText.text = $"{sellPrice}";
        }
    }

    public void SetCount(int newCount)
    {
        count = newCount;
        UpdateVisuals();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (contextMenu != null)
            {
                contextMenu.ShowSellMenu(this, eventData.position);
            }
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (shopManager != null && itemData != null)
        {
            shopManager.ShowPlayerItemDescription(itemData);
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