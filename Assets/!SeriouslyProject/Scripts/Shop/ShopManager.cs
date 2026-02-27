using UnityEngine;
using System;
using AudioManager.Core;
using AudioManager.Locator;

namespace EchoRift.Shop
{
    /// <summary>
    /// Менеджер системы магазина
    /// Управляет транзакциями купли-продажи
    /// </summary>
    public class ShopManager : MonoBehaviour
    {
        private InventoryManager playerInventory;
        private PlayerWallet playerWallet;
        private ShopData currentShop;

        public ShopData CurrentShop => currentShop;
        public bool IsShopOpen => currentShop != null;

        public event Action<ShopData> OnShopOpened;
        public event Action OnShopClosed;
        public event Action<ItemData, int, int> OnItemBought;  // item, quantity, totalPrice
        public event Action<ItemData, int, int> OnItemSold;    // item, quantity, totalPrice

        private IAudioManager service;

        private void Start()
        {
            service = ServiceLocator.GetService();
        }

        public void Initialize(InventoryManager inventory, PlayerWallet wallet)
        {
            playerInventory = inventory;
            playerWallet = wallet;
            //Debug.Log($"[ShopManager] Инициализирован. InventoryManager: {(inventory != null ? "OK" : "NULL")}, PlayerWallet: {(wallet != null ? "OK" : "NULL")}");
        }

        /// <summary>
        /// Открыть магазин
        /// </summary>
        public void OpenShop(ShopData shopData)
        {
            Debug.Log($"[ShopManager] OpenShop вызван с shopData: {(shopData != null ? shopData.shopName : "NULL")}");
            
            if (shopData == null)
            {
                Debug.LogError("[ShopManager] ShopData is null!");
                return;
            }

            if (playerInventory == null)
            {
                Debug.LogError("[ShopManager] playerInventory не инициализирован! Вызовите Initialize() перед открытием магазина.");
                return;
            }

            if (playerWallet == null)
            {
                Debug.LogError("[ShopManager] playerWallet не инициализирован! Вызовите Initialize() перед открытием магазина.");
                return;
            }

            currentShop = shopData;
            Debug.Log($"[ShopManager] Вызов события OnShopOpened. Подписчиков: {(OnShopOpened != null ? OnShopOpened.GetInvocationList().Length : 0)}");
            OnShopOpened?.Invoke(shopData);
            Debug.Log($"[ShopManager] Открыт магазин: {shopData.shopName}");
        }

        /// <summary>
        /// Закрыть магазин
        /// </summary>
        public void CloseShop()
        {
            if (currentShop == null) return;

            Debug.Log($"[ShopManager] Закрыт магазин: {currentShop.shopName}");
            currentShop = null;
            OnShopClosed?.Invoke();
        }

        /// <summary>
        /// Купить предмет у торговца
        /// </summary>
        public bool BuyItem(ItemData item, int quantity = 1)
        {
            if (!IsShopOpen)
            {
                Debug.LogWarning("[ShopManager] Магазин не открыт!");
                return false;
            }

            if (item == null || quantity <= 0)
            {
                Debug.LogWarning("[ShopManager] Некорректные параметры покупки!");
                return false;
            }

            ShopItem shopItem = currentShop.FindShopItem(item);
            if (shopItem == null)
            {
                Debug.LogWarning($"[ShopManager] Предмет {item.itemName} не продаётся в этом магазине!");
                return false;
            }

            // Проверка наличия товара
            if (!shopItem.infiniteStock && shopItem.quantity < quantity)
            {
                Debug.LogWarning($"[ShopManager] Недостаточно товара! Доступно: {shopItem.quantity}");
                return false;
            }

            // Проверка места в инвентаре
            bool canAdd = playerInventory.CanAddItem(item.itemName, quantity);
            Debug.Log($"[ShopManager] Проверка покупки: {item.itemName} (GameName: {item.itemGameName}). Можно добавить: {canAdd}, Количество: {quantity}");
            
            if (!canAdd)
            {
                Debug.LogWarning("[ShopManager] Недостаточно места в инвентаре!");
                return false;
            }

            // Расчёт стоимости
            int pricePerItem = shopItem.GetBuyPrice();
            int totalPrice = pricePerItem * quantity;

            // Проверка денег
            if (!playerWallet.HasEnoughCoins(totalPrice))
            {
                Debug.LogWarning($"[ShopManager] Недостаточно денег! Нужно: {totalPrice}, Есть: {playerWallet.Coins}");
                return false;
            }

            // Выполнение транзакции
            if (!playerWallet.TrySpendCoins(totalPrice))
            {
                return false;
            }

            Debug.Log($"[ShopManager] Добавление предмета: {item.itemName}");
            if (!playerInventory.AddItem(item.itemName, quantity))
            {
                // Откат транзакции
                playerWallet.AddCoins(totalPrice);
                Debug.LogError("[ShopManager] Ошибка добавления предмета в инвентарь!");
                return false;
            }

            // Уменьшение запаса товара
            if (!shopItem.infiniteStock)
            {
                shopItem.quantity -= quantity;
            }
            service.PlayOneShot("Shop1");

            OnItemBought?.Invoke(item, quantity, totalPrice);
            Debug.Log($"[ShopManager] Куплено: {item.itemName} x{quantity} за {totalPrice} монет");
            return true;
        }

        /// <summary>
        /// Продать предмет торговцу
        /// </summary>
        public bool SellItem(ItemData item, int quantity = 1)
        {
            if (!IsShopOpen)
            {
                Debug.LogWarning("[ShopManager] Магазин не открыт!");
                return false;
            }

            if (item == null || quantity <= 0)
            {
                Debug.LogWarning("[ShopManager] Некорректные параметры продажи!");
                return false;
            }

            if (!currentShop.acceptsPlayerItems)
            {
                Debug.LogWarning("[ShopManager] Этот торговец не покупает предметы!");
                return false;
            }

            // Проверка наличия предмета у игрока
            int availableCount = playerInventory.GetItemCount(item.itemName);
            Debug.Log($"[ShopManager] Проверка продажи: {item.itemName} (GameName: {item.itemGameName}). Доступно: {availableCount}, Нужно: {quantity}");
            
            if (availableCount < quantity)
            {
                Debug.LogWarning($"[ShopManager] Недостаточно предметов для продажи! Доступно: {availableCount}, Нужно: {quantity}");
                return false;
            }

            // Расчёт стоимости
            int pricePerItem = currentShop.GetSellPriceForItem(item);
            int totalPrice = pricePerItem * quantity;
            service.PlayOneShot("Shop1");

            // Удаление предмета из инвентаря
            Debug.Log($"[ShopManager] Удаление предмета: {item.itemName}");
            if (!playerInventory.RemoveItem(item.itemName, quantity))
            {
                Debug.LogError("[ShopManager] Ошибка удаления предмета из инвентаря!");
                return false;
            }

            // Добавление денег игроку
            playerWallet.AddCoins(totalPrice);

            // Добавление предмета в магазин (если он там продаётся)
            ShopItem shopItem = currentShop.FindShopItem(item);
            if (shopItem != null && !shopItem.infiniteStock)
            {
                shopItem.quantity += quantity;
            }

            OnItemSold?.Invoke(item, quantity, totalPrice);
            Debug.Log($"[ShopManager] Продано: {item.itemName} x{quantity} за {totalPrice} монет");
            return true;
        }

        /// <summary>
        /// Получить цену покупки предмета
        /// </summary>
        public int GetBuyPrice(ItemData item)
        {
            if (!IsShopOpen || item == null) return 0;

            ShopItem shopItem = currentShop.FindShopItem(item);
            return shopItem?.GetBuyPrice() ?? 0;
        }

        /// <summary>
        /// Получить цену продажи предмета
        /// </summary>
        public int GetSellPrice(ItemData item)
        {
            if (!IsShopOpen || item == null) return 0;
            return currentShop.GetSellPriceForItem(item);
        }

        /// <summary>
        /// Очистить все подписки на события (для предотвращения утечек памяти)
        /// </summary>
        public void ClearEventSubscriptions()
        {
            OnShopOpened = null;
            OnShopClosed = null;
            OnItemBought = null;
            OnItemSold = null;
        }
    }
}
