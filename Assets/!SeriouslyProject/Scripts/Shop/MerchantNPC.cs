using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace EchoRift.Shop
{
    /// <summary>
    /// NPC-торговец с магазином
    /// </summary>
    public class MerchantNPC : MonoBehaviour, IShopkeeper
    {
        [Header("Shop Settings")]
        [SerializeField] private ShopData shopData;

        [Header("Dialogue")]
        [SerializeField] private DialogueSystemTrigger dialogueTrigger;

        private ShopUI shopUI;

        public ShopData ShopData => shopData;

        private void Awake()
        {
            shopUI = FindObjectOfType<ShopUI>();
        }

        public void OpenShop()
        {
            if (shopData == null)
            {
                Debug.LogError($"[MerchantNPC] ShopData не назначен для {gameObject.name}!");
                return;
            }

            if (shopUI == null)
            {
                Debug.LogError("[MerchantNPC] ShopUI не найден!");
                return;
            }

            shopUI.OpenShop(shopData);
        }

        public void CloseShop()
        {
            shopUI?.CloseShop();
        }

        private void OnValidate()
        {
            if (shopData == null)
            {
                Debug.LogWarning($"[MerchantNPC] Не назначен ShopData для {gameObject.name}!");
            }
        }
    }
}
