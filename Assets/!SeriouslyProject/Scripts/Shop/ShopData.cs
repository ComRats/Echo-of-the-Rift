using UnityEngine;
using System.Collections.Generic;
using Sirenix.OdinInspector;

namespace EchoRift.Shop
{
    /// <summary>
    /// Данные о товаре в магазине
    /// </summary>
    [System.Serializable]
    public class ShopItem
    {
        [Required("Предмет обязателен!")]
        [PreviewField(50, ObjectFieldAlignment.Left)]
        public ItemData item;

        [MinValue(1)]
        [LabelText("Количество")]
        public int quantity = 1;

        [MinValue(0)]
        [LabelText("Цена покупки")]
        [InfoBox("0 = использовать цену из ItemData")]
        public int buyPrice = 0;

        [MinValue(0)]
        [LabelText("Цена продажи")]
        [InfoBox("0 = 50% от цены покупки")]
        public int sellPrice = 0;

        [LabelText("Бесконечный запас")]
        public bool infiniteStock = false;

        /// <summary>
        /// Получить цену покупки (игрок покупает у торговца)
        /// </summary>
        public int GetBuyPrice()
        {
            return buyPrice > 0 ? buyPrice : item.itemPrice;
        }

        /// <summary>
        /// Получить цену продажи (игрок продаёт торговцу)
        /// </summary>
        public int GetSellPrice()
        {
            if (sellPrice > 0) return sellPrice;
            int basePrice = GetBuyPrice();
            return Mathf.Max(1, basePrice / 2);
        }
    }

    /// <summary>
    /// Конфигурация магазина торговца
    /// </summary>
    [CreateAssetMenu(fileName = "New Shop", menuName = "Shop/Shop Data")]
    public class ShopData : ScriptableObject
    {
        [TitleGroup("Информация о магазине")]
        [LabelText("Название магазина")]
        public string shopName = "Магазин";

        [TitleGroup("Информация о магазине")]
        [LabelText("Описание")]
        [TextArea(2, 4)]
        public string shopDescription;

        [TitleGroup("Товары")]
        [ListDrawerSettings(ShowIndexLabels = true, ListElementLabelName = "item")]
        public List<ShopItem> items = new List<ShopItem>();

        [TitleGroup("Настройки")]
        [LabelText("Торговец покупает предметы")]
        [InfoBox("Если выключено, игрок не сможет продавать предметы")]
        public bool acceptsPlayerItems = true;

        [TitleGroup("Настройки")]
        [ShowIf("acceptsPlayerItems")]
        [LabelText("Процент выкупа")]
        [Range(10, 100)]
        [InfoBox("Процент от базовой цены предмета при продаже игроком")]
        public int buybackPercentage = 50;

        /// <summary>
        /// Валидация данных магазина в редакторе
        /// </summary>
        private void OnValidate()
        {
            ValidateDuplicateItems();
        }

        /// <summary>
        /// Проверка на дубликаты предметов в списке
        /// </summary>
        private void ValidateDuplicateItems()
        {
            if (items == null || items.Count == 0)
                return;

            var seenItems = new System.Collections.Generic.HashSet<ItemData>();
            var duplicates = new System.Collections.Generic.List<string>();

            for (int i = 0; i < items.Count; i++)
            {
                if (items[i].item == null)
                    continue;

                if (seenItems.Contains(items[i].item))
                {
                    duplicates.Add($"'{items[i].item.itemName}' (индекс {i})");
                }
                else
                {
                    seenItems.Add(items[i].item);
                }
            }

            if (duplicates.Count > 0)
            {
                Debug.LogWarning($"[ShopData: {shopName}] Обнаружены дубликаты предметов: {string.Join(", ", duplicates)}. Это может привести к проблемам!");
            }
        }

        /// <summary>
        /// Найти товар в магазине по ItemData
        /// </summary>
        public ShopItem FindShopItem(ItemData itemData)
        {
            return items.Find(x => x.item == itemData);
        }

        /// <summary>
        /// Проверить, есть ли товар в наличии
        /// </summary>
        public bool HasItemInStock(ItemData itemData)
        {
            ShopItem shopItem = FindShopItem(itemData);
            if (shopItem == null) return false;
            return shopItem.infiniteStock || shopItem.quantity > 0;
        }

        /// <summary>
        /// Получить цену продажи для любого предмета (если торговец принимает)
        /// </summary>
        public int GetSellPriceForItem(ItemData itemData)
        {
            ShopItem shopItem = FindShopItem(itemData);
            if (shopItem != null)
            {
                return shopItem.GetSellPrice();
            }

            if (acceptsPlayerItems)
            {
                return Mathf.Max(1, (itemData.itemPrice * buybackPercentage) / 100);
            }

            return 0;
        }
    }
}
