using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;
using Unity.VectorGraphics;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
{
    [Header("UI")]
    [Tooltip("SVG Image компонент для отображения иконки")]
    public SVGImage image;
    public TextMeshProUGUI countText;

    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public ItemData itemData;
    public int count = 1;

    private InventoryManager inventoryManager;
    private InventoryContextMenu contextMenu;

    private float snapDistance = 70f;
    private float snapDistanceSqr;

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();

        if (contextMenu == null)
            contextMenu = FindObjectOfType<InventoryContextMenu>();
    }

    private void Start()
    {
        snapDistanceSqr = snapDistance * snapDistance;
    }

    public void InitialiseItem(ItemData newItem, int amount)
    {
        itemData = newItem;
        
        // Устанавливаем SVG иконку (импортированную как Sprite)
        if (image != null && newItem.icon != null)
        {
            image.sprite = newItem.icon;
            image.preserveAspect = true;
        }
        
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
        {
            contextMenu.Hide();
        }

        parentAfterDrag = transform.parent;

        Transform canvasTransform = GetComponentInParent<Canvas>().transform;
        transform.SetParent(canvasTransform);
        transform.SetAsLastSibling();

        image.raycastTarget = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Разрешаем перетаскивание только левой кнопкой мыши
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Разрешаем перетаскивание только левой кнопкой мыши
        if (eventData.button != PointerEventData.InputButton.Left)
            return;

        image.raycastTarget = true;

        if (transform.parent == GetComponentInParent<Canvas>().transform)
        {
            CheckForNearbySlot(eventData);
        }

        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;

        // Синхронизируем после перетаскивания
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

        // Проверяем слоты инвентаря
        foreach (InventorySlot slot in inventoryManager.inventorySlots)
        {
            float distSqr = (transform.position - slot.transform.position).sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                closestSlot = slot;
            }
        }

        // Проверяем слоты экипировки
        foreach (InventorySlot slot in inventoryManager.equipmentSlots)
        {
            float distSqr = (transform.position - slot.transform.position).sqrMagnitude;
            if (distSqr < minDistanceSqr)
            {
                minDistanceSqr = distSqr;
                closestSlot = slot;
            }
        }

        // Если нашли близкий слот и тип подходит - дропаем
        if (closestSlot != null && minDistanceSqr <= snapDistanceSqr)
        {
            if (closestSlot.IsTypeAllowed(this))
            {
                closestSlot.OnDrop(eventData);
            }
        }
    }
}