#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using UnityEngine.UI;
using TMPro;

public class InventoryGenerator : EditorWindow
{
    [Header("Inventory Settings")]
    private int inventoryWidth = 9;
    private int inventoryHeight = 4;
    private Vector2 slotSize = new Vector2(60, 60);
    private Vector2 slotSpacing = new Vector2(5, 5);
    
    [Header("UI Settings")]
    private Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 0.8f);
    private Color slotColor = new Color(0.3f, 0.3f, 0.3f, 1f);
    private Color normalColor = Color.white;
    private Color highlightColor = new Color(1f, 1f, 0.8f, 1f);
    private Color selectedColor = new Color(0.8f, 1f, 0.8f, 1f);
    
    [Header("References")]
    private Sprite slotBackgroundSprite;
    private Font textFont;
    
    private Canvas targetCanvas;
    private bool autoFindCanvas = true;
    
    [MenuItem("Tools/Inventory Generator")]
    public static void ShowWindow()
    {
        GetWindow<InventoryGenerator>("Inventory Generator");
    }
    
    private void OnGUI()
    {
        GUILayout.Label("Inventory Generator", EditorStyles.boldLabel);
        EditorGUILayout.Space();
        
        DrawInventorySettings();
        EditorGUILayout.Space();
        
        DrawUISettings();
        EditorGUILayout.Space();
        
        DrawReferences();
        EditorGUILayout.Space();
        
        DrawButtons();
    }
    
    private void DrawInventorySettings()
    {
        GUILayout.Label("Inventory Settings", EditorStyles.boldLabel);
        
        inventoryWidth = EditorGUILayout.IntField("Width (slots)", inventoryWidth);
        inventoryHeight = EditorGUILayout.IntField("Height (slots)", inventoryHeight);
        
        EditorGUILayout.Space();
        slotSize = EditorGUILayout.Vector2Field("Slot Size", slotSize);
        slotSpacing = EditorGUILayout.Vector2Field("Slot Spacing", slotSpacing);
        
        EditorGUILayout.HelpBox($"Total slots: {inventoryWidth * inventoryHeight}", MessageType.Info);
    }
    
    private void DrawUISettings()
    {
        GUILayout.Label("Visual Settings", EditorStyles.boldLabel);
        
        backgroundColor = EditorGUILayout.ColorField("Background Color", backgroundColor);
        slotColor = EditorGUILayout.ColorField("Slot Background", slotColor);
        normalColor = EditorGUILayout.ColorField("Normal Color", normalColor);
        highlightColor = EditorGUILayout.ColorField("Highlight Color", highlightColor);
        selectedColor = EditorGUILayout.ColorField("Selected Color", selectedColor);
    }
    
    private void DrawReferences()
    {
        GUILayout.Label("References", EditorStyles.boldLabel);
        
        autoFindCanvas = EditorGUILayout.Toggle("Auto Find Canvas", autoFindCanvas);
        
        if (!autoFindCanvas)
        {
            targetCanvas = (Canvas)EditorGUILayout.ObjectField("Target Canvas", targetCanvas, typeof(Canvas), true);
        }
        
        slotBackgroundSprite = (Sprite)EditorGUILayout.ObjectField("Slot Background Sprite", slotBackgroundSprite, typeof(Sprite), false);
        textFont = (Font)EditorGUILayout.ObjectField("Text Font", textFont, typeof(Font), false);
        
        if (slotBackgroundSprite == null)
        {
            EditorGUILayout.HelpBox("No slot sprite assigned. Will use Unity's default UI sprite.", MessageType.Warning);
        }
    }
    
    private void DrawButtons()
    {
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Generate Complete Inventory System", GUILayout.Height(30)))
        {
            GenerateInventorySystem();
        }
        
        EditorGUILayout.Space();
        
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Generate UI Only"))
        {
            GenerateInventoryUI();
        }
        
        if (GUILayout.Button("Generate Data Only"))
        {
            GenerateInventoryData();
        }
        EditorGUILayout.EndHorizontal();
        
        EditorGUILayout.Space();
        
        if (GUILayout.Button("Create Sample Items"))
        {
            CreateSampleItems();
        }
    }
    
    private void GenerateInventorySystem()
    {
        GameObject inventoryObject = GenerateInventoryData();
        GenerateInventoryUI(inventoryObject);
        
        // Добавляем тестер
        inventoryObject.AddComponent<InventoryTester>();
        
        Debug.Log("Complete inventory system generated successfully!");
    }
    
    private GameObject GenerateInventoryData()
    {
        // Создаём объект для инвентаря
        GameObject inventoryObject = new GameObject("InventorySystem");
        
        // Добавляем компонент Inventory
        Inventory inventory = inventoryObject.AddComponent<Inventory>();
        
        // Устанавливаем размер инвентаря через Reflection (так как поле приватное)
        var sizeField = typeof(Inventory).GetField("_inventorySize", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        sizeField?.SetValue(inventory, inventoryWidth * inventoryHeight);
        
        Debug.Log($"Inventory data component created with {inventoryWidth * inventoryHeight} slots");
        return inventoryObject;
    }
    
    private void GenerateInventoryUI(GameObject inventoryObject = null)
    {
        // Находим или создаём Canvas
        Canvas canvas = FindOrCreateCanvas();
        if (canvas == null) return;
        
        // Создаём UI структуру
        GameObject inventoryPanel = CreateInventoryPanel(canvas.transform);
        GameObject slotsContainer = CreateSlotsContainer(inventoryPanel.transform);
        GameObject slotPrefab = CreateSlotPrefab();
        
        // Добавляем InventoryUI компонент
        InventoryUI inventoryUI = inventoryPanel.AddComponent<InventoryUI>();
        
        // Устанавливаем ссылки через Reflection
        SetPrivateField(inventoryUI, "_inventoryPanel", inventoryPanel);
        SetPrivateField(inventoryUI, "_slotsContainer", slotsContainer.transform);
        SetPrivateField(inventoryUI, "_slotPrefab", slotPrefab);
        
        // Создаём кнопку закрытия
        CreateCloseButton(inventoryPanel.transform, inventoryUI);
        
        Debug.Log("Inventory UI generated successfully!");
        
        // Сохраняем префаб слота
        SaveSlotPrefab(slotPrefab);
    }
    
    private Canvas FindOrCreateCanvas()
    {
        Canvas canvas = null;
        
        if (autoFindCanvas)
        {
            canvas = FindObjectOfType<Canvas>();
        }
        else
        {
            canvas = targetCanvas;
        }
        
        if (canvas == null)
        {
            // Создаём новый Canvas
            GameObject canvasObject = new GameObject("Canvas");
            canvas = canvasObject.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasObject.AddComponent<CanvasScaler>();
            canvasObject.AddComponent<GraphicRaycaster>();
            
            Debug.Log("New Canvas created");
        }
        
        return canvas;
    }
    
    private GameObject CreateInventoryPanel(Transform parent)
    {
        GameObject panel = new GameObject("InventoryPanel");
        panel.transform.SetParent(parent, false);
        
        // RectTransform
        RectTransform rectTransform = panel.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0.5f);
        rectTransform.anchorMax = new Vector2(0.5f, 0.5f);
        rectTransform.sizeDelta = new Vector2(
            inventoryWidth * slotSize.x + (inventoryWidth - 1) * slotSpacing.x + 40,
            inventoryHeight * slotSize.y + (inventoryHeight - 1) * slotSpacing.y + 80
        );
        
        // Background Image
        Image backgroundImage = panel.AddComponent<Image>();
        backgroundImage.color = backgroundColor;
        backgroundImage.sprite = slotBackgroundSprite;
        backgroundImage.type = Image.Type.Sliced;
        
        return panel;
    }
    
    private GameObject CreateSlotsContainer(Transform parent)
    {
        GameObject container = new GameObject("SlotsContainer");
        container.transform.SetParent(parent, false);
        
        // RectTransform
        RectTransform rectTransform = container.AddComponent<RectTransform>();
        rectTransform.anchorMin = Vector2.zero;
        rectTransform.anchorMax = Vector2.one;
        rectTransform.offsetMin = new Vector2(20, 40);
        rectTransform.offsetMax = new Vector2(-20, -20);
        
        // Grid Layout Group
        GridLayoutGroup gridLayout = container.AddComponent<GridLayoutGroup>();
        gridLayout.cellSize = slotSize;
        gridLayout.spacing = slotSpacing;
        gridLayout.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        gridLayout.constraintCount = inventoryWidth;
        gridLayout.childAlignment = TextAnchor.MiddleCenter;
        
        return container;
    }
    
    private GameObject CreateSlotPrefab()
    {
        GameObject slotPrefab = new GameObject("InventorySlot");
        
        // RectTransform
        RectTransform rectTransform = slotPrefab.AddComponent<RectTransform>();
        rectTransform.sizeDelta = slotSize;
        
        // Background Image
        Image backgroundImage = slotPrefab.AddComponent<Image>();
        backgroundImage.color = slotColor;
        backgroundImage.sprite = slotBackgroundSprite;
        
        // Button
        Button button = slotPrefab.AddComponent<Button>();
        
        // Item Icon
        GameObject iconObject = new GameObject("ItemIcon");
        iconObject.transform.SetParent(slotPrefab.transform, false);
        
        RectTransform iconRect = iconObject.AddComponent<RectTransform>();
        iconRect.anchorMin = Vector2.zero;
        iconRect.anchorMax = Vector2.one;
        iconRect.offsetMin = new Vector2(5, 5);
        iconRect.offsetMax = new Vector2(-5, -5);
        
        Image itemIcon = iconObject.AddComponent<Image>();
        itemIcon.color = Color.white;
        itemIcon.enabled = false;
        
        // Quantity Text
        GameObject textObject = new GameObject("QuantityText");
        textObject.transform.SetParent(slotPrefab.transform, false);
        
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(1, 0);
        textRect.anchorMax = new Vector2(1, 0);
        textRect.anchoredPosition = new Vector2(-5, 5);
        textRect.sizeDelta = new Vector2(30, 20);
        
        TextMeshProUGUI quantityText = textObject.AddComponent<TextMeshProUGUI>();
        quantityText.text = "";
        quantityText.fontSize = 12;
        quantityText.color = Color.white;
        quantityText.alignment = TextAlignmentOptions.BottomRight;
        quantityText.enabled = false;
        
        if (textFont != null)
        {
            quantityText.font = TMP_FontAsset.CreateFontAsset(textFont);
        }
        
        // InventorySlotUI Component
        InventorySlotUI slotUI = slotPrefab.AddComponent<InventorySlotUI>();
        
        // Устанавливаем ссылки через Reflection
        SetPrivateField(slotUI, "_backgroundImage", backgroundImage);
        SetPrivateField(slotUI, "_itemIcon", itemIcon);
        SetPrivateField(slotUI, "_quantityText", quantityText);
        SetPrivateField(slotUI, "_slotButton", button);
        SetPrivateField(slotUI, "_normalColor", normalColor);
        SetPrivateField(slotUI, "_highlightColor", highlightColor);
        SetPrivateField(slotUI, "_selectedColor", selectedColor);
        
        return slotPrefab;
    }
    
    private void CreateCloseButton(Transform parent, InventoryUI inventoryUI)
    {
        GameObject closeButton = new GameObject("CloseButton");
        closeButton.transform.SetParent(parent, false);
        
        RectTransform rectTransform = closeButton.AddComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(1, 1);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.anchoredPosition = new Vector2(-10, -10);
        rectTransform.sizeDelta = new Vector2(30, 30);
        
        Image buttonImage = closeButton.AddComponent<Image>();
        buttonImage.color = Color.red;
        
        Button button = closeButton.AddComponent<Button>();
        
        // Текст "X"
        GameObject textObject = new GameObject("Text");
        textObject.transform.SetParent(closeButton.transform, false);
        
        RectTransform textRect = textObject.AddComponent<RectTransform>();
        textRect.anchorMin = Vector2.zero;
        textRect.anchorMax = Vector2.one;
        textRect.offsetMin = Vector2.zero;
        textRect.offsetMax = Vector2.zero;
        
        TextMeshProUGUI buttonText = textObject.AddComponent<TextMeshProUGUI>();
        buttonText.text = "X";
        buttonText.fontSize = 16;
        buttonText.color = Color.white;
        buttonText.alignment = TextAlignmentOptions.Center;
        
        // Устанавливаем ссылку на кнопку закрытия
        SetPrivateField(inventoryUI, "_closeButton", button);
    }
    
    private void SaveSlotPrefab(GameObject slotPrefab)
    {
        // Создаём папку для префабов
        string folderPath = "Assets/Prefabs";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Prefabs");
        }
        
        // Сохраняем префаб
        string prefabPath = $"{folderPath}/InventorySlot.prefab";
        PrefabUtility.SaveAsPrefabAsset(slotPrefab, prefabPath);
        
        Debug.Log($"Slot prefab saved to: {prefabPath}");
    }
    
    private void CreateSampleItems()
    {
        // Создаём папку для предметов
        string folderPath = "Assets/Items";
        if (!AssetDatabase.IsValidFolder(folderPath))
        {
            AssetDatabase.CreateFolder("Assets", "Items");
        }
        
        // Примеры предметов
        string[] itemNames = { "Health Potion", "Mana Potion", "Iron Sword", "Wood", "Stone", "Apple", "Gold Coin" };
        ItemType[] itemTypes = { ItemType.Consumable, ItemType.Consumable, ItemType.Weapon, 
                                ItemType.Material, ItemType.Material, ItemType.Consumable, ItemType.Material };
        int[] stackSizes = { 10, 10, 1, 64, 64, 16, 999 };
        
        for (int i = 0; i < itemNames.Length; i++)
        {
            Item newItem = CreateInstance<Item>();
            
            // Устанавливаем свойства через Reflection
            SetPrivateField(newItem, "_itemName", itemNames[i]);
            SetPrivateField(newItem, "_description", $"A {itemNames[i].ToLower()} item for testing.");
            SetPrivateField(newItem, "_id", i + 1);
            SetPrivateField(newItem, "_itemType", itemTypes[i]);
            SetPrivateField(newItem, "_maxStackSize", stackSizes[i]);
            SetPrivateField(newItem, "_isStackable", stackSizes[i] > 1);
            
            // Сохраняем как ScriptableObject
            string assetPath = $"{folderPath}/{itemNames[i].Replace(" ", "")}.asset";
            AssetDatabase.CreateAsset(newItem, assetPath);
        }
        
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        
        Debug.Log($"Created {itemNames.Length} sample items in {folderPath}");
    }
    
    // Вспомогательный метод для установки приватных полей
    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        
        if (field != null)
        {
            field.SetValue(obj, value);
        }
        else
        {
            Debug.LogWarning($"Field '{fieldName}' not found in {obj.GetType().Name}");
        }
    }
}
#endif