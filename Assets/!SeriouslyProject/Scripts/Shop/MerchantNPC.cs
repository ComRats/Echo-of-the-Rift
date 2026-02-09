using UnityEngine;
using PixelCrushers.DialogueSystem;

namespace EchoRift.Shop
{
    /// <summary>
    /// NPC-торговец с магазином
    /// </summary>
    public class MerchantNPC : MonoBehaviour, IShopkeeper, ITalkable
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
            if (shopUI == null)
            {
                Debug.LogError("[MerchantNPC] ShopUI не найден в сцене!");
            }
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

        public void Talk(Collider2D trigger, DialogueSystemTrigger dialogueData, DialogueSystemTrigger conversation)
        {
            // Можно открыть магазин через диалог или сразу
            // Пока оставляем заглушку для интеграции с Dialogue System
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
