using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using EchoRift.Shop;
using AudioManager.Locator;
using AudioManager.Core;

public class InventoryContextMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject contextMenuPanel;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Shop References")]
    [SerializeField] private ShopUI shopUI;

    [Header("Settings")]
    [SerializeField] private Vector2 offset = new Vector2(10f, -10f);

    [Header("Audio")]
    [SerializeField] private string equipSoundName = "Equipment1";

    private DraggableItem currentItem;
    private InventorySlot currentSlot;
    private List<GameObject> activeButtons = new List<GameObject>();
    private IAudioManager audioManager;

    private void Awake()
    {
        if (contextMenuPanel != null)
        {
            contextMenuPanel.SetActive(false);
        }

        if (inventoryManager == null)
        {
            inventoryManager = FindObjectOfType<InventoryManager>();
        }

        audioManager = ServiceLocator.GetService();
    }

    private void OnDestroy()
    {
        ClearButtons();

        currentItem = null;
        currentSlot = null;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && contextMenuPanel.activeSelf)
        {
            if (!RectTransformUtility.RectangleContainsScreenPoint(
                contextMenuPanel.GetComponent<RectTransform>(),
                Input.mousePosition))
            {
                Hide();
            }
        }
    }

    public void Show(DraggableItem item, Vector2 position)
    {
        if (item == null || item.itemData == null) return;

        currentItem = item;
        currentSlot = item.transform.parent?.GetComponent<InventorySlot>();
        ClearButtons();

        bool isShopMode = shopUI != null && shopUI.IsShopMode;

        if (isShopMode && currentSlot != null)
        {
            CreateShopButtons(item, currentSlot);
        }
        else
        {
            CreateButtonsForItem(item);
        }

        contextMenuPanel.SetActive(true);
        RectTransform menuRect = contextMenuPanel.GetComponent<RectTransform>();
        menuRect.position = position + offset;

        ClampMenuToScreen(menuRect);
    }

    public void Hide()
    {
        if (contextMenuPanel != null)
        {
            contextMenuPanel.SetActive(false);
        }

        ClearButtons();

        currentItem = null;
        currentSlot = null;
    }

    private void CreateButtonsForItem(DraggableItem item)
    {
        ItemType itemType = item.itemData.itemType;

        Debug.Log($"Создание кнопок для предмета: {item.itemData.itemName}, Тип: {itemType}");

        if ((itemType & ItemType.Food) != 0 || (itemType & ItemType.Potion) != 0)
        {
            Debug.Log("Добавлена кнопка 'Использовать'");
            CreateButton("Использовать", () => UseItem(item));
        }

        if ((itemType & (ItemType.Weapon | ItemType.Armor | ItemType.Amulet | ItemType.Helmet)) != 0)
        {
            Debug.Log("Добавлена кнопка 'Экипировать'");
            CreateButton("Экипировать", () => EquipItem(item));
        }

        CreateButton("Выбросить", () => DropItem(item));

    }

    private void CreateShopButtons(DraggableItem item, InventorySlot slot)
    {
        if (shopUI == null) return;

        bool isMerchantItem = shopUI.IsMerchantSlot(slot);
        bool isPlayerItem = shopUI.IsPlayerShopSlot(slot);

        if (isMerchantItem)
        {
            CreateBuyButtons(item);
        }
        else if (isPlayerItem)
        {
            CreateSellButtons(item);
        }
    }

    private void CreateBuyButtons(DraggableItem item)
    {
        if (shopUI == null || shopUI.ShopManager == null) return;

        int buyPrice = shopUI.ShopManager.GetBuyPrice(item.itemData);

        CreateButton($"Купить 1 ({buyPrice} монет)", () => BuyItem(item, 1));

        if (item.itemData.isStackable && item.count >= 5)
        {
            CreateButton($"Купить 5 ({buyPrice * 5} монет)", () => BuyItem(item, 5));
        }

        if (item.itemData.isStackable && item.count >= 10)
        {
            CreateButton($"Купить 10 ({buyPrice * 10} монет)", () => BuyItem(item, 10));
        }

        if (item.itemData.isStackable && item.count > 1 && item.count < 999)
        {
            CreateButton($"Купить всё ({buyPrice * item.count} монет)", () => BuyItem(item, item.count));
        }

        CreateButton("Информация", () => ShowItemInfo(item.itemData));
    }

    private void CreateSellButtons(DraggableItem item)
    {
        if (shopUI == null || shopUI.ShopManager == null) return;

        if (!shopUI.ShopManager.CurrentShop.acceptsPlayerItems)
        {
            CreateButton("Торговец не покупает предметы", null);
            return;
        }

        int sellPrice = shopUI.ShopManager.GetSellPrice(item.itemData);

        int totalCount = GetTotalItemCount(item.itemData);

        CreateButton($"Продать 1 ({sellPrice} монет)", () => SellItem(item, 1));

        if (totalCount >= 5)
        {
            CreateButton($"Продать 5 ({sellPrice * 5} монет)", () => SellItem(item, 5));
        }

        if (totalCount >= 10)
        {
            CreateButton($"Продать 10 ({sellPrice * 10} монет)", () => SellItem(item, 10));
        }

        if (totalCount > 1)
        {
            CreateButton($"Продать всё ({sellPrice * totalCount} монет)", () => SellAllItems(item.itemData, totalCount));
        }

        CreateButton("Информация", () => ShowItemInfo(item.itemData));
    }

    private void CreateButton(string buttonText, System.Action onClick)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, contextMenuPanel.transform);
        ContextMenuButton menuButton = buttonObj.GetComponent<ContextMenuButton>();

        if (menuButton != null)
        {
            if (onClick == null)
            {
                menuButton.Initialize(buttonText, null);
                Button button = buttonObj.GetComponent<Button>();
                if (button != null)
                {
                    button.interactable = false;
                }
            }
            else
            {
                menuButton.Initialize(buttonText, () =>
                {
                    onClick?.Invoke();
                    Hide();
                });
            }
        }

        activeButtons.Add(buttonObj);
    }

    private void ClearButtons()
    {
        foreach (GameObject button in activeButtons)
        {
            if (button != null)
            {
                Destroy(button);
            }
        }
        activeButtons.Clear();
    }

    private void ClampMenuToScreen(RectTransform menuRect)
    {
        Vector3[] corners = new Vector3[4];
        menuRect.GetWorldCorners(corners);

        Vector3 position = menuRect.position;

        if (corners[2].x > Screen.width)
        {
            position.x -= (corners[2].x - Screen.width);
        }

        if (corners[0].x < 0)
        {
            position.x -= corners[0].x;
        }

        if (corners[1].y > Screen.height)
        {
            position.y -= (corners[1].y - Screen.height);
        }

        if (corners[0].y < 0)
        {
            position.y -= corners[0].y;
        }

        menuRect.position = position;
    }

    #region Item Actions

    private void UseItem(DraggableItem item)
    {
        if (item == null || item.itemData == null) return;

        TeamMember[] teamMembers = FindObjectsOfType<TeamMember>();
        TeamMember target = null;

        foreach (var member in teamMembers)
        {
            if (member.CanUseItemPublic(item.itemData))
            {
                target = member;
                break;
            }
        }

        if (target != null)
        {
            target.UseItemPublic(item.itemData, item);
        }
        else
        {
            Debug.Log($"[ContextMenu] Никто не нуждается в {item.itemData.itemName}");
        }
    }

    private void EquipItem(DraggableItem item)
    {
        if (inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager не найден!");
            return;
        }

        if (item.parentAfterDrag == null)
        {
            item.parentAfterDrag = item.transform.parent;
        }

        Debug.Log($"Экипировка предмета: {item.itemData.itemName}");

        InventorySlot targetEquipSlot = null;

        foreach (InventorySlot equipSlot in inventoryManager.equipmentSlots)
        {
            if (equipSlot.IsTypeAllowed(item))
            {
                targetEquipSlot = equipSlot;
                break;
            }
        }

        if (targetEquipSlot == null)
        {
            Debug.LogWarning($"Не найден подходящий слот для экипировки предмета типа {item.itemData.itemType}");
            return;
        }

        if (targetEquipSlot.transform.childCount == 0)
        {
            item.transform.SetParent(targetEquipSlot.transform);
            item.transform.localPosition = Vector3.zero;
            item.parentAfterDrag = targetEquipSlot.transform;

            PlayEquipSound();
        }
        else
        {
            DraggableItem equippedItem = targetEquipSlot.GetComponentInChildren<DraggableItem>();

            if (equippedItem != null)
            {
                Transform itemOriginalParent = item.parentAfterDrag;

                InventorySlot originalSlot = itemOriginalParent.GetComponent<InventorySlot>();
                bool isFromInventory = originalSlot != null && System.Array.IndexOf(inventoryManager.inventorySlots, originalSlot) >= 0;

                if (isFromInventory)
                {
                    equippedItem.transform.SetParent(itemOriginalParent);
                    equippedItem.transform.localPosition = Vector3.zero;
                    equippedItem.parentAfterDrag = itemOriginalParent;
                }
                else
                {
                    InventorySlot emptySlot = FindEmptyInventorySlot();

                    if (emptySlot != null)
                    {
                        equippedItem.transform.SetParent(emptySlot.transform);
                        equippedItem.transform.localPosition = Vector3.zero;
                        equippedItem.parentAfterDrag = emptySlot.transform;
                    }
                    else
                    {
                        Debug.LogWarning("Нет свободного места в инвентаре для обмена!");
                        return;
                    }
                }

                item.transform.SetParent(targetEquipSlot.transform);
                item.transform.localPosition = Vector3.zero;
                item.parentAfterDrag = targetEquipSlot.transform;

                PlayEquipSound();
            }
        }

        inventoryManager.SyncFromUI();
    }

    private void DropItem(DraggableItem item)
    {
        Debug.Log($"Выброшен предмет: {item.itemData.itemName}");

        if (inventoryManager != null && currentSlot != null)
        {
            inventoryManager.RemoveItemFromSlot(currentSlot, 1);
        }
        else if (inventoryManager != null && item.parentAfterDrag != null)
        {
            InventorySlot slot = item.parentAfterDrag.GetComponent<InventorySlot>();
            if (slot != null)
            {
                inventoryManager.RemoveItemFromSlot(slot, 1);
            }
            else
            {
                inventoryManager.RemoveItem(item.itemData.itemName, 1);
            }
        }
    }


    #endregion

    #region Shop Actions

    private void BuyItem(DraggableItem item, int quantity)
    {
        if (item == null || shopUI == null || shopUI.ShopManager == null) return;

        bool success = shopUI.ShopManager.BuyItem(item.itemData, quantity);

        if (success)
        {
            Debug.Log($"Успешно куплено: {item.itemData.itemName} x{quantity}");
            shopUI.OnItemTransactionComplete();
        }
    }

    private void SellItem(DraggableItem item, int quantity)
    {
        if (item == null || shopUI == null || shopUI.ShopManager == null) return;

        bool success = shopUI.ShopManager.SellItem(item.itemData, quantity);

        if (success)
        {
            Debug.Log($"Успешно продано: {item.itemData.itemName} x{quantity}");
            shopUI.OnItemTransactionComplete();
        }
    }

    private void SellAllItems(ItemData itemData, int totalCount)
    {
        if (itemData == null || shopUI == null || shopUI.ShopManager == null) return;

        bool success = shopUI.ShopManager.SellItem(itemData, totalCount);

        if (success)
        {
            Debug.Log($"Успешно продано всё: {itemData.itemName} x{totalCount}");
            shopUI.OnItemTransactionComplete();
        }
    }

    private int GetTotalItemCount(ItemData itemData)
    {
        if (itemData == null || shopUI == null) return 0;

        int totalCount = 0;

        foreach (var slot in shopUI.PlayerSlots)
        {
            if (slot == null) continue;

            DraggableItem draggableItem = slot.GetComponentInChildren<DraggableItem>();
            if (draggableItem != null && draggableItem.itemData != null)
            {
                // Сравниваем по имени предмета
                if (draggableItem.itemData.itemName == itemData.itemName)
                {
                    totalCount += draggableItem.count;
                }
            }
        }

        if (totalCount == 0 && inventoryManager != null)
        {
            totalCount = inventoryManager.GetItemCount(itemData.itemName);
        }

        return totalCount;
    }

    private void ShowItemInfo(ItemData item)
    {
        Debug.Log($"=== {item.itemName} ===\n{item.description}\nТип: {item.itemType}\nБазовая цена: {item.itemPrice}");
    }

    #endregion

    private InventorySlot FindEmptyInventorySlot()
    {
        if (inventoryManager == null) return null;

        foreach (InventorySlot slot in inventoryManager.inventorySlots)
        {
            if (slot.transform.childCount == 0)
            {
                return slot;
            }
        }

        return null;
    }

    private void PlayEquipSound()
    {
        ServiceLocator.GetService().PlayOneShot(equipSoundName);
    }
}