using UnityEngine;
using UnityEngine.EventSystems;

namespace EchoRift.Shop
{
    /// <summary>
    /// Слот инвентаря для магазина
    /// Открывает контекстное меню при ПКМ
    /// </summary>
    public class ShopInventorySlot : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] private InventoryContextMenu contextMenu;
        [SerializeField] private ItemDescriptionDisplay descriptionDisplay;

        private void Awake()
        {
            if (contextMenu == null)
            {
                contextMenu = FindObjectOfType<InventoryContextMenu>();
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right)
            {
                DraggableItem item = GetComponentInChildren<DraggableItem>();
                if (item != null && item.itemData != null)
                {
                    if (contextMenu != null)
                    {
                        contextMenu.Show(item, eventData.position);
                    }
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (descriptionDisplay == null) return;

            DraggableItem item = GetComponentInChildren<DraggableItem>();
            if (item != null && item.itemData != null)
            {
                descriptionDisplay.ShowItem(item);
            }
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            if (descriptionDisplay != null)
            {
                descriptionDisplay.Hide();
            }
        }
    }
}
