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
        image.sprite = newItem.icon;
        count = amount;
        RefreshCount();
        
        // Инициализируем parentAfterDrag текущим родителем
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
        // Проверяем клик правой кнопкой мыши
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
        // Закрываем контекстное меню при начале перетаскивания
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
        transform.position = Input.mousePosition;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        image.raycastTarget = true;

        if (transform.parent == GetComponentInParent<Canvas>().transform)
        {
             CheckForNearbySlot(eventData);
        }

        transform.SetParent(parentAfterDrag);
        transform.localPosition = Vector3.zero;
    }

    private void CheckForNearbySlot(PointerEventData eventData)
    {
        if (inventoryManager == null) return;

        InventorySlot closestSlot = null;
        float minDistanceSqr = float.MaxValue;

        for (int i = 0; i < inventoryManager.inventorySlots.Length; i++)
        {
            InventorySlot slot = inventoryManager.inventorySlots[i];
            Vector3 direction = transform.position - slot.transform.position;
            float distanceSqr = direction.sqrMagnitude;
            
            if (distanceSqr < minDistanceSqr)
            {
                minDistanceSqr = distanceSqr;
                closestSlot = slot;
            }
        }

        if (closestSlot != null && minDistanceSqr <= snapDistanceSqr)
        {
            closestSlot.OnDrop(eventData);
        }
    }
}