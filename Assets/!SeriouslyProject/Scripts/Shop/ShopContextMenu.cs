using UnityEngine;
using System.Collections.Generic;

public class ShopContextMenu : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject menuPanel;
    [SerializeField] private GameObject buttonPrefab;
    [SerializeField] private ShopManager shopManager;

    [Header("Settings")]
    [SerializeField] private Vector2 offset = new Vector2(10f, -10f);

    private List<GameObject> activeButtons = new List<GameObject>();
    private ShopItem currentShopItem;
    private PlayerShopItem currentPlayerItem;

    private void Awake()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && menuPanel != null && menuPanel.activeSelf)
        {
            RectTransform menuRect = menuPanel.GetComponent<RectTransform>();
            if (!RectTransformUtility.RectangleContainsScreenPoint(menuRect, Input.mousePosition))
            {
                Hide();
            }
        }
    }

    /// <summary>
    /// ���� ������� ��� ������ ��������
    /// </summary>
    public void ShowBuyMenu(ShopItem item, Vector2 position)
    {
        if (item == null || item.ItemData == null) return;

        currentShopItem = item;
        currentPlayerItem = null;

        ClearButtons();

        int price = shopManager.ShopData.GetBuyPrice(item.ItemData);
        CreateButton($"������ ({price} �����)", OnBuyClicked);

        ShowMenu(position);
    }

    /// <summary>
    /// ���� ������� ��� �������� ������ (�� InventoryData)
    /// </summary>
    public void ShowSellMenu(PlayerShopItem item, Vector2 position)
    {
        if (item == null || item.ItemData == null) return;

        currentPlayerItem = item;
        currentShopItem = null;

        ClearButtons();

        int sellPrice = shopManager.ShopData.GetSellPrice(item.ItemData);
        CreateButton($"������� ({sellPrice} �����)", OnSellClicked);

        ShowMenu(position);
    }

    /// <summary>
    /// ���� ������� ��� �������� ������ �� DraggableItem (�� ���������� ���������)
    /// </summary>
    public void ShowSellMenu(DraggableItem item, Vector2 position)
    {
        if (item == null || item.itemData == null) return;

        // ��� DraggableItem �� �� ����� ������ ������� ������ ����� ShopManager
        // ������� ������ ���������� ���������� � ������
        ClearButtons();

        int sellPrice = shopManager.ShopData.GetSellPrice(item.itemData);
        CreateButton($"������� ({sellPrice} �����)", () => OnSellDraggableItem(item));

        ShowMenu(position);
    }

    private void OnSellDraggableItem(DraggableItem item)
    {
        if (item != null && shopManager != null)
        {
            shopManager.SellItemFromInventory(item);
        }
        Hide();
    }

    public void Hide()
    {
        if (menuPanel != null)
        {
            menuPanel.SetActive(false);
        }

        currentShopItem = null;
        currentPlayerItem = null;
        ClearButtons();
    }

    private void ShowMenu(Vector2 position)
    {
        if (menuPanel == null) return;

        menuPanel.SetActive(true);

        RectTransform menuRect = menuPanel.GetComponent<RectTransform>();
        menuRect.position = position + offset;

        ClampToScreen(menuRect);
    }

    private void CreateButton(string text, System.Action onClick)
    {
        GameObject buttonObj = Instantiate(buttonPrefab, menuPanel.transform);
        ContextMenuButton menuButton = buttonObj.GetComponent<ContextMenuButton>();

        if (menuButton != null)
        {
            menuButton.Initialize(text, () =>
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
            if (button != null)
            {
                Destroy(button);
            }
        }
        activeButtons.Clear();
    }

    private void OnBuyClicked()
    {
        if (currentShopItem != null && shopManager != null)
        {
            shopManager.BuyItem(currentShopItem);
        }
    }

    private void OnSellClicked()
    {
        if (currentPlayerItem != null && shopManager != null)
        {
            shopManager.SellItem(currentPlayerItem);
        }
    }

    private void ClampToScreen(RectTransform menuRect)
    {
        Vector3[] corners = new Vector3[4];
        menuRect.GetWorldCorners(corners);

        Vector3 position = menuRect.position;

        if (corners[2].x > Screen.width)
            position.x -= (corners[2].x - Screen.width);

        if (corners[0].x < 0)
            position.x -= corners[0].x;

        if (corners[1].y > Screen.height)
            position.y -= (corners[1].y - Screen.height);

        if (corners[0].y < 0)
            position.y -= corners[0].y;

        menuRect.position = position;
    }
}