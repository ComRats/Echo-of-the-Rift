using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class DraggableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [Header("UI")]
    public Image image;
    public TextMeshProUGUI countText;

    [HideInInspector] public Transform parentAfterDrag;
    [HideInInspector] public ItemData itemData;
    public int count = 1;

    private InventoryManager inventoryManager;

    private float snapDistance = 70f;
    private float snapDistanceSqr;

    private void Awake()
    {
        if (inventoryManager == null)
            inventoryManager = FindObjectOfType<InventoryManager>();
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
    }

    public void RefreshCount()
    {
        countText.text = count.ToString();
        bool textActive = count > 1;
        countText.gameObject.SetActive(textActive);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
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