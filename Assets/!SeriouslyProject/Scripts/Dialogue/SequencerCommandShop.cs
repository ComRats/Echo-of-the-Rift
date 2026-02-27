using UnityEngine;
using EchoRift.Shop;
using Zenject;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    /// <summary>
    /// Sequencer команда для открытия магазина
    /// Использование: Shop(ShopDataName)
    /// Пример: Shop(GeneralStoreShop)
    /// </summary>
    public class SequencerCommandShop : SequencerCommand
    {
        [Inject] MainUI mainUI;
        public void Awake()
        {
            string shopDataName = GetParameter(0);

            if (string.IsNullOrEmpty(shopDataName))
            {
                Debug.LogError("[SequencerCommandShop] Не указано имя магазина!");
                Stop();
                return;
            }

            // Найти MainUI на сцене
            if (mainUI == null)
            {
                MainUI mainUI = Object.FindObjectOfType<MainUI>();
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
                Debug.LogError($"[SequencerCommandShop] ShopData '{shopDataName}' не найден в Resources!");
                Stop();
                return;
            }

            shopUI.OpenShop(shopData);
            Stop();
        }

    }
}