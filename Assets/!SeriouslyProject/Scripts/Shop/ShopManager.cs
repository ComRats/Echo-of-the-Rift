using UnityEngine;
using System;
using AudioManager.Core;
using AudioManager.Locator;

namespace EchoRift.Shop
{
    public class ShopManager : MonoBehaviour
    {
        private InventoryManager playerInventory;
        private PlayerWallet playerWallet;
        private ShopData currentShop;

        public ShopData CurrentShop => currentShop;
        public bool IsShopOpen => currentShop != null;

        public event Action<ShopData> OnShopOpened;
        public event Action OnShopClosed;
        public event Action<ItemData, int, int> OnItemBought;
        public event Action<ItemData, int, int> OnItemSold;

        private IAudioManager service;

        private void Start()
        {
            service = ServiceLocator.GetService();
        }

        public void Initialize(InventoryManager inventory, PlayerWallet wallet)
        {
            playerInventory = inventory;
            playerWallet = wallet;
        }

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

        public void CloseShop()
        {
            if (currentShop == null) return;

            Debug.Log($"[ShopManager] Закрыт магазин: {currentShop.shopName}");
            currentShop = null;
            OnShopClosed?.Invoke();
        }

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

            if (!shopItem.infiniteStock && shopItem.quantity < quantity)
            {
                Debug.LogWarning($"[ShopManager] Недостаточно товара! Доступно: {shopItem.quantity}");
                return false;
            }

            bool canAdd = playerInventory.CanAddItem(item.itemName, quantity);
            Debug.Log($"[ShopManager] Проверка покупки: {item.itemName} (GameName: {item.itemGameName}). Можно добавить: {canAdd}, Количество: {quantity}");
            
            if (!canAdd)
            {
                Debug.LogWarning("[ShopManager] Недостаточно места в инвентаре!");
                return false;
            }

            int pricePerItem = shopItem.GetBuyPrice();
            int totalPrice = pricePerItem * quantity;

            if (!playerWallet.HasEnoughCoins(totalPrice))
            {
                Debug.LogWarning($"[ShopManager] Недостаточно денег! Нужно: {totalPrice}, Есть: {playerWallet.Coins}");
                return false;
            }

            if (!playerWallet.TrySpendCoins(totalPrice))
            {
                return false;
            }

            Debug.Log($"[ShopManager] Добавление предмета: {item.itemName}");
            if (!playerInventory.AddItem(item.itemName, quantity))
            {
                playerWallet.AddCoins(totalPrice);
                Debug.LogError("[ShopManager] Ошибка добавления предмета в инвентарь!");
                return false;
            }

            if (!shopItem.infiniteStock)
            {
                shopItem.quantity -= quantity;
            }

            if (service != null)
            {
                service.PlayOneShot("Shop1");
            }

            OnItemBought?.Invoke(item, quantity, totalPrice);
            Debug.Log($"[ShopManager] Куплено: {item.itemName} x{quantity} за {totalPrice} монет");
            return true;
        }

        public bool SellItem(ItemData item, int quantity = 1, int preferredInventorySlotIndex = -1)
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

            int availableCount = playerInventory.GetItemCount(item.itemName);
            Debug.Log($"[ShopManager] Проверка продажи: {item.itemName} (GameName: {item.itemGameName}). Доступно: {availableCount}, Нужно: {quantity}");
            
            if (availableCount < quantity)
            {
                Debug.LogWarning($"[ShopManager] Недостаточно предметов для продажи! Доступно: {availableCount}, Нужно: {quantity}");
                return false;
            }

            int pricePerItem = currentShop.GetSellPriceForItem(item);
            int totalPrice = pricePerItem * quantity;

            Debug.Log($"[ShopManager] Удаление предмета: {item.itemName}");
            bool removed = playerInventory.RemoveItem(item.itemName, quantity);

            if (!removed)
            {
                Debug.LogError("[ShopManager] Ошибка удаления предмета из инвентаря!");
                return false;
            }

            playerWallet.AddCoins(totalPrice);

            ShopItem shopItem = currentShop.FindShopItem(item);
            if (shopItem != null && !shopItem.infiniteStock)
            {
                shopItem.quantity += quantity;
            }

            if (service != null)
            {
                service.PlayOneShot("Shop1");
            }

            OnItemSold?.Invoke(item, quantity, totalPrice);
            Debug.Log($"[ShopManager] Продано: {item.itemName} x{quantity} за {totalPrice} монет");
            return true;
        }

        public int GetBuyPrice(ItemData item)
        {
            if (!IsShopOpen || item == null) return 0;

            ShopItem shopItem = currentShop.FindShopItem(item);
            return shopItem?.GetBuyPrice() ?? 0;
        }

        public int GetSellPrice(ItemData item)
        {
            if (!IsShopOpen || item == null) return 0;
            return currentShop.GetSellPriceForItem(item);
        }

        public void ClearEventSubscriptions()
        {
            OnShopOpened = null;
            OnShopClosed = null;
            OnItemBought = null;
            OnItemSold = null;
        }
    }
}
