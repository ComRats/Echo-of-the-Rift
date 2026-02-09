using UnityEngine;

namespace EchoRift.Shop
{
    /// <summary>
    /// Интерфейс для NPC-торговцев
    /// </summary>
    public interface IShopkeeper
    {
        /// <summary>
        /// Данные магазина торговца
        /// </summary>
        ShopData ShopData { get; }

        /// <summary>
        /// Открыть магазин для взаимодействия с игроком
        /// </summary>
        void OpenShop();

        /// <summary>
        /// Закрыть магазин
        /// </summary>
        void CloseShop();
    }
}
