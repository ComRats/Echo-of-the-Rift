using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class InventoryContextMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject contextMenuPanel;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private InventoryManager inventoryManager;

    [Header("Settings")]
    [SerializeField] private Vector2 offset = new Vector2(10f, -10f);

    private DraggableItem currentItem;
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
        ClearButtons();

        // Создаем кнопки в зависимости от типа предмета
        CreateButtonsForItem(item);

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
        currentItem = null;
        ClearButtons();
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

    private void CreateButton(string buttonText, System.Action onClick)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, contextMenuPanel.transform);
        ContextMenuButton menuButton = buttonObj.GetComponent<ContextMenuButton>();

        if (menuButton != null)
        {
            menuButton.Initialize(buttonText, () =>
            {
                onClick?.Invoke();
                Hide();
            });
        }

        activeButtons.Add(buttonObj);
    }

    private void ClearButtons()
    {
        foreach (GameObject button in activeButtons)
        {
            Destroy(button);
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

        // Удаляем один предмет через InventoryManager
        if (inventoryManager != null)
        {
            inventoryManager.RemoveItem(item.itemData.itemName, 1);
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

        Destroy(item.gameObject);
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