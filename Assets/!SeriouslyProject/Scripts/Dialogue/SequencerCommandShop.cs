using UnityEngine;
using EchoRift.Shop;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    /// <summary>
    /// Sequencer команда для открытия магазина
    /// Использование: Shop(ShopDataName)
    /// Пример: Shop(GeneralStoreShop)
    /// </summary>
    public class SequencerCommandShop : SequencerCommand
    {
        public void Awake()
        {
            string shopDataName = GetParameter(0);

            if (string.IsNullOrEmpty(shopDataName))
            {
                Debug.LogError("[SequencerCommandShop] Не указано имя магазина!");
                Stop();
                return;
            }

            MainUI mainUI = GlobalLoader.Instance.mainUI;
            if (mainUI == null)
            {
                Debug.LogError("[SequencerCommandShop] MainUI не найден на сцене!");
                Stop();
                return;
            }

            ShopUI shopUI = mainUI.shopUI;
            if (shopUI == null)
            {
                Debug.LogError("[SequencerCommandShop] ShopUI не найден в MainUI!");
                Stop();
                return;
            }

            // Загрузить ShopData из Resources
            ShopData shopData = Resources.Load<ShopData>(shopDataName);
            if (shopData == null)
            {
                // Попытка загрузить из всех возможных путей в Resources
                ShopData[] allShops = Resources.LoadAll<ShopData>("");
                foreach (var shop in allShops)
                {
                    if (shop.name == shopDataName)
                    {
                        shopData = shop;
                        Debug.Log($"[SequencerCommandShop] ShopData '{shopDataName}' найден в альтернативном пути");
                        break;
                    }
                }

                if (shopData == null)
                {
                    Debug.LogError($"[SequencerCommandShop] ShopData '{shopDataName}' не найден в Resources! Убедитесь, что файл находится в папке Resources.");
                    Stop();
                    return;
                }
            }

            shopUI.OpenShop(shopData);
            Stop();
        }

    }
}