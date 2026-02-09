using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("UI")]
    public Image image;
    public TextMeshProUGUI countText;

    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public ItemData itemData;
    public int count = 1;

    private InventoryManager inventoryManager;
    private InventoryContextMenu contextMenu;
    private ShopManager shopManager;
    private ShopContextMenu shopContextMenu;

    private float snapDistance = 70f;
    private float snapDistanceSqr;

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();

        if (contextMenu == null)
            contextMenu = FindObjectOfType<InventoryContextMenu>();

        if (shopManager == null)
            shopManager = FindObjectOfType<ShopManager>();

        if (shopContextMenu == null)
            shopContextMenu = FindObjectOfType<ShopContextMenu>();
    }

    private void Start()
    {
        snapDistanceSqr = snapDistance * snapDistance;
    }

    public void InitialiseItem(ItemData newItem, int amount)
    {
        itemData = newItem;
        image.sprite = newItem.icon;
        count = amount;
        RefreshCount();

        parentAfterDrag = transform.parent;
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            OpenContextMenu(eventData.position);
        }
    }

    private void OpenContextMenu(Vector2 position)
    {
        // Если магазин открыт - показываем меню продажи
        if (shopManager != null && shopManager.IsOpen)
        {
            if (shopContextMenu != null)
            {
                shopContextMenu.ShowSellMenu(this, position);
            }
            return;
        }

        // Иначе - обычное контекстное меню
        if (contextMenu != null)
        {
            contextMenu.Show(this, position);
        }
        else
        {
            Debug.LogWarning("InventoryContextMenu не найдено в сцене!");
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        // Разрешаем перетаскивание только левой кнопкой мыши
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        if (contextMenu != null)
            contextMenu.Hide();

        if (shopContextMenu != null)
            shopContextMenu.Hide();

        parentAfterDrag = transform.parent;

        Transform canvasTransform = GetComponentInParent<Canvas>().transform;
        transform.SetParent(canvasTransform);
        transform.SetAsLastSibling();

        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Перетаскивание только левой кнопкой мыши
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Завершение перетаскивания только для левой кнопки мыши
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        image.raycastTarget = true;

        if (transform.parent == GetComponentInParent<Canvas>().transform)
        {
            CheckForNearbySlot(eventData);
        }

        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;

        if (inventoryManager != null)
        {
            inventoryManager.SyncFromUI();
        }
    }

    private void CheckForNearbySlot(PointerEventData eventData)
    {
        if (inventoryManager == null) return;

        InventorySlot closestSlot = null;
        float minDistanceSqr = float.MaxValue;

        foreach (InventorySlot slot in inventoryManager.inventorySlots)
        {
            float distSqr = (transform.position - slot.transform.position).sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                closestSlot = slot;
            }
        }

        foreach (InventorySlot slot in inventoryManager.equipmentSlots)
        {
            float distSqr = (transform.position - slot.transform.position).sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                closestSlot = slot;
            }
        }

        if (closestSlot != null && minDistanceSqr <= snapDistanceSqr)
        {
            if (closestSlot.IsTypeAllowed(this))
            {
                closestSlot.OnDrop(eventData);
            }
        }
    }
}