using UnityEngine;
using TMPro;

/// <summary>
/// Компонент на TextMeshProUGUI, который показывает информацию о предметах.
/// Вешается на TextMeshProUGUI.
/// </summary>
public class ItemDescriptionDisplay : MonoBehaviour
{
    private TextMeshProUGUI descriptionText;

    private void Awake()
    {
        descriptionText = GetComponent<TextMeshProUGUI>();

        if (descriptionText == null)
        {
            Debug.LogError("[ItemDescriptionDisplay] Компонент должен быть на объекте с TextMeshProUGUI!");
            enabled = false;
            return;
        }

        HideDescription();
    }

    /// <summary>
    /// Показывает информацию о предмете (обычный режим)
    /// </summary>
    public void ShowItem(DraggableItem item)
    {
        if (descriptionText == null || item == null || item.itemData == null)
        {
            HideDescription();
            return;
        }

        ItemData itemData = item.itemData;

        string itemName = itemData.itemGameName;
        string itemType = GetRussianItemType(itemData.itemType);
        string description = itemData.description;

        descriptionText.text = $"Название: {itemName}\nТип: {itemType}\nОписание:\n{description}";
        descriptionText.enabled = true;
    }

    /// <summary>
    /// Показывает предмет из магазина с ценой покупки
    /// </summary>
    public void ShowShopItem(ItemData itemData)
    {
        if (descriptionText == null || itemData == null)
        {
            HideDescription();
            return;
        }

        string itemName = itemData.itemGameName;
        string itemType = GetRussianItemType(itemData.itemType);
        string description = itemData.description;
        int price = itemData.itemPrice;

        descriptionText.text = $"Название: {itemName}\nТип: {itemType}\nЦена: {price} монет\nОписание:\n{description}";
        descriptionText.enabled = true;
    }

    /// <summary>
    /// Показывает предмет игрока с ценой продажи
    /// </summary>
    public void ShowPlayerItem(DraggableItem item)
    {
        if (descriptionText == null || item == null || item.itemData == null)
        {
            HideDescription();
            return;
        }

        ItemData itemData = item.itemData;

        string itemName = itemData.itemGameName;
        string itemType = GetRussianItemType(itemData.itemType);
        string description = itemData.description;
        int sellPrice = CalculateSellPrice(itemData.itemPrice);

        descriptionText.text = $"Название: {itemName}\nТип: {itemType}\nЦена продажи: {sellPrice} монет\nОписание:\n{description}";
        descriptionText.enabled = true;
    }

    /// <summary>
    /// Скрывает описание
    /// </summary>
    public void Hide()
    {
        if (descriptionText != null)
        {
            descriptionText.enabled = false;
            descriptionText.text = string.Empty;
        }
    }

    private void HideDescription()
    {
        if (descriptionText != null)
        {
            descriptionText.enabled = false;
            descriptionText.text = string.Empty;
        }
    }

    private int CalculateSellPrice(int buyPrice)
    {
        return Mathf.Max(1, buyPrice / 2);
    }

    private string GetRussianItemType(ItemType type)
    {
        if ((type & ItemType.Food) != 0)
            return "Еда";

        if ((type & ItemType.Potion) != 0)
            return "Зелье";

        if ((type & ItemType.Weapon) != 0)
            return "Оружие";

        if ((type & ItemType.Armor) != 0)
            return "Броня";

        if ((type & ItemType.Amulet) != 0)
            return "Амулет";

        if ((type & ItemType.Helmet) != 0)
            return "Шлем";

        return "Неизвестный предмет";
    }

    private void OnDisable()
    {
        HideDescription();
    }

    /// <summary>
    /// Показывает предмет с кастомной ценой и лейблом
    /// </summary>
    public void ShowShopItemWithCustomPrice(ItemData itemData, int price, string priceLabel)
    {
        if (descriptionText == null || itemData == null)
        {
            HideDescription();
            return;
        }

        string itemName = itemData.itemGameName;
        string itemType = GetRussianItemType(itemData.itemType);
        string description = itemData.description;

        descriptionText.text = $"Название: {itemName}\nТип: {itemType}\n{priceLabel}: {price} монет\nОписание:\n{description}";
        descriptionText.enabled = true;
    }
}
