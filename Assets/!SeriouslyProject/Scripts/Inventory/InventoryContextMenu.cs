using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using EchoRift.Shop;

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

    private DraggableItem currentItem;
    private InventorySlot currentSlot;
    private List<GameObject> activeButtons = new List<GameObject>();

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
    }

    private void OnDestroy()
    {
        // Очищаем все кнопки при уничтожении объекта
        ClearButtons();
        
        // Обнуляем ссылки
        currentItem = null;
        currentSlot = null;        
    }

    private void Update()
    {
        // Закрываем меню при клике ЛКМ вне меню
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

        // Проверяем, открыт ли магазин
        bool isShopMode = shopUI != null && shopUI.IsShopMode;

        if (isShopMode && currentSlot != null)
        {
            // Режим магазина - создаём кнопки покупки/продажи
            CreateShopButtons(item, currentSlot);
        }
        else
        {
            // Обычный режим - создаём стандартные кнопки
            CreateButtonsForItem(item);
        }

        // Позиционируем меню
        contextMenuPanel.SetActive(true);
        RectTransform menuRect = contextMenuPanel.GetComponent<RectTransform>();
        menuRect.position = position + offset;

        // Проверяем, чтобы меню не выходило за границы экрана
        ClampMenuToScreen(menuRect);
    }

    public void Hide()
    {
        if (contextMenuPanel != null)
        {
            contextMenuPanel.SetActive(false);
        }
        
        // Очищаем кнопки перед обнулением ссылок
        ClearButtons();
        
        currentItem = null;
        currentSlot = null;
    }

    private void CreateButtonsForItem(DraggableItem item)
    {
        ItemType itemType = item.itemData.itemType;

        Debug.Log($"Создание кнопок для предмета: {item.itemData.itemName}, Тип: {itemType}");

        // Кнопка "Использовать" для еды и зелий
        if ((itemType & ItemType.Food) != 0 || (itemType & ItemType.Potion) != 0)
        {
            Debug.Log("Добавлена кнопка 'Использовать'");
            CreateButton("Использовать", () => UseItem(item));
        }

        // Кнопка "Экипировать" для оружия, брони, амулетов и шлемов
        if ((itemType & (ItemType.Weapon | ItemType.Armor | ItemType.Amulet | ItemType.Helmet)) != 0)
        {
            Debug.Log("Добавлена кнопка 'Экипировать'");
            CreateButton("Экипировать", () => EquipItem(item));
        }

        // Кнопка "Выбросить" для всех предметов
        CreateButton("Выбросить", () => DropItem(item));

    }

    private void CreateShopButtons(DraggableItem item, InventorySlot slot)
    {
        if (shopUI == null) return;

        // Определяем, в каком инвентаре находится предмет
        bool isMerchantItem = shopUI.IsMerchantSlot(slot);
        bool isPlayerItem = shopUI.IsPlayerShopSlot(slot);

        if (isMerchantItem)
        {
            // Кнопки покупки
            CreateBuyButtons(item);
        }
        else if (isPlayerItem)
        {
            // Кнопки продажи
            CreateSellButtons(item);
        }
    }

    private void CreateBuyButtons(DraggableItem item)
    {
        if (shopUI == null || shopUI.ShopManager == null) return;
        
        int buyPrice = shopUI.ShopManager.GetBuyPrice(item.itemData);

        // Кнопка "Купить 1"
        CreateButton($"Купить 1 ({buyPrice} монет)", () => BuyItem(item, 1));

        // Кнопка "Купить 5" (если стакается)
        if (item.itemData.isStackable && item.count >= 5)
        {
            CreateButton($"Купить 5 ({buyPrice * 5} монет)", () => BuyItem(item, 5));
        }

        // Кнопка "Купить 10" (если стакается)
        if (item.itemData.isStackable && item.count >= 10)
        {
            CreateButton($"Купить 10 ({buyPrice * 10} монет)", () => BuyItem(item, 10));
        }

        // Кнопка "Купить всё" (если стакается и не бесконечный запас)
        if (item.itemData.isStackable && item.count > 1 && item.count < 999)
        {
            CreateButton($"Купить всё ({buyPrice * item.count} монет)", () => BuyItem(item, item.count));
        }

        // Кнопка "Информация"
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
        
        // Подсчитываем общее количество этого предмета во всех слотах
        int totalCount = GetTotalItemCount(item.itemData);

        // Кнопка "Продать 1"
        CreateButton($"Продать 1 ({sellPrice} монет)", () => SellItem(item, 1));

        // Кнопка "Продать 5" (если есть)
        if (totalCount >= 5)
        {
            CreateButton($"Продать 5 ({sellPrice * 5} монет)", () => SellItem(item, 5));
        }

        // Кнопка "Продать 10" (если есть)
        if (totalCount >= 10)
        {
            CreateButton($"Продать 10 ({sellPrice * 10} монет)", () => SellItem(item, 10));
        }

        // Кнопка "Продать всё" - продаёт ВСЕ экземпляры из всех слотов
        if (totalCount > 1)
        {
            CreateButton($"Продать всё ({sellPrice * totalCount} монет)", () => SellAllItems(item.itemData, totalCount));
        }

        // Кнопка "Информация"
        CreateButton("Информация", () => ShowItemInfo(item.itemData));
    }

    private void CreateButton(string buttonText, System.Action onClick)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, contextMenuPanel.transform);
        ContextMenuButton menuButton = buttonObj.GetComponent<ContextMenuButton>();

        if (menuButton != null)
        {
            // Если onClick == null, делаем кнопку неактивной (для информационных сообщений)
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

        // Проверяем правую границу
        if (corners[2].x > Screen.width)
        {
            position.x -= (corners[2].x - Screen.width);
        }

        // Проверяем левую границу
        if (corners[0].x < 0)
        {
            position.x -= corners[0].x;
        }

        // Проверяем верхнюю границу
        if (corners[1].y > Screen.height)
        {
            position.y -= (corners[1].y - Screen.height);
        }

        // Проверяем нижнюю границу
        if (corners[0].y < 0)
        {
            position.y -= corners[0].y;
        }

        menuRect.position = position;
    }

    #region Item Actions

    private void UseItem(DraggableItem item)
    {
        Debug.Log($"Использован предмет: {item.itemData.itemName}");

        // Применяем эффекты предмета здесь
        // Например: восстановление HP для еды, баффы для зелий и т.д.

        // Удаляем предмет из конкретного слота, с которым взаимодействовал игрок
        if (inventoryManager != null && item.parentAfterDrag != null)
        {
            InventorySlot slot = item.parentAfterDrag.GetComponent<InventorySlot>();
            if (slot != null)
            {
                inventoryManager.RemoveItemFromSlot(slot, 1);
            }
            else
            {
                // Fallback на старый метод, если слот не найден
                inventoryManager.RemoveItem(item.itemData.itemName, 1);
            }
        }
    }

    private void EquipItem(DraggableItem item)
    {
        if (inventoryManager == null)
        {
            Debug.LogWarning("InventoryManager не найден!");
            return;
        }

        // Проверяем, что parentAfterDrag инициализирован
        if (item.parentAfterDrag == null)
        {
            item.parentAfterDrag = item.transform.parent;
        }

        Debug.Log($"Экипировка предмета: {item.itemData.itemName}");

        // Находим подходящий слот экипировки
        InventorySlot targetEquipSlot = null;

        foreach (InventorySlot equipSlot in inventoryManager.equipmentSlots)
        {
            // Проверяем, подходит ли тип предмета для этого слота
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

        // Если слот экипировки пуст - просто перемещаем предмет
        if (targetEquipSlot.transform.childCount == 0)
        {
            item.transform.SetParent(targetEquipSlot.transform);
            item.transform.localPosition = Vector3.zero;
            item.parentAfterDrag = targetEquipSlot.transform;
        }
        else
        {
            // Если в слоте уже есть предмет - меняем их местами
            DraggableItem equippedItem = targetEquipSlot.GetComponentInChildren<DraggableItem>();

            if (equippedItem != null)
            {
                Transform itemOriginalParent = item.parentAfterDrag;

                // Проверяем, откуда пришёл предмет (из инвентаря или из другого слота экипировки)
                InventorySlot originalSlot = itemOriginalParent.GetComponent<InventorySlot>();
                bool isFromInventory = originalSlot != null && System.Array.IndexOf(inventoryManager.inventorySlots, originalSlot) >= 0;

                if (isFromInventory)
                {
                    // Если предмет из инвентаря - меняем их местами напрямую
                    equippedItem.transform.SetParent(itemOriginalParent);
                    equippedItem.transform.localPosition = Vector3.zero;
                    equippedItem.parentAfterDrag = itemOriginalParent;
                }
                else
                {
                    // Если предмет из другого слота экипировки - ищем пустой слот в инвентаре
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

                // Экипируем новый предмет
                item.transform.SetParent(targetEquipSlot.transform);
                item.transform.localPosition = Vector3.zero;
                item.parentAfterDrag = targetEquipSlot.transform;
            }
        }
    }

    private void DropItem(DraggableItem item)
    {
        Debug.Log($"Выброшен предмет: {item.itemData.itemName}");

        // Здесь можно добавить логику выбрасывания предмета в мир
        // Например: создание физического объекта предмета в игровом мире

        // Удаляем только 1 единицу предмета из конкретного слота
        if (inventoryManager != null && currentSlot != null)
        {
            inventoryManager.RemoveItemFromSlot(currentSlot, 1);
        }
        else if (inventoryManager != null && item.parentAfterDrag != null)
        {
            // Fallback: если currentSlot не установлен, пытаемся получить слот из parentAfterDrag
            InventorySlot slot = item.parentAfterDrag.GetComponent<InventorySlot>();
            if (slot != null)
            {
                inventoryManager.RemoveItemFromSlot(slot, 1);
            }
            else
            {
                // Последний fallback: удаляем через имя предмета
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

    /// <summary>
    /// Продаёт ВСЕ экземпляры предмета из всех слотов инвентаря
    /// </summary>
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

    /// <summary>
    /// Подсчитывает общее количество предмета во всех слотах инвентаря игрока
    /// </summary>
    private int GetTotalItemCount(ItemData itemData)
    {
        if (itemData == null || shopUI == null) return 0;

        int totalCount = 0;

        // Проходим по всем слотам игрока в магазине
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

        // Если ничего не нашли в слотах магазина, проверяем основной инвентарь через InventoryManager
        if (totalCount == 0 && inventoryManager != null)
        {
            totalCount = inventoryManager.GetItemCount(itemData.itemName);
        }

        return totalCount;
    }

    private void ShowItemInfo(ItemData item)
    {
        Debug.Log($"=== {item.itemName} ===\n{item.description}\nТип: {item.itemType}\nБазовая цена: {item.itemPrice}");
        // Здесь можно добавить отдельное окно с информацией о предмете
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
}