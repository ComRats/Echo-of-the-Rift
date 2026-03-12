using FightSystem.Character;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using AudioManager.Locator;
using AudioManager.Core;
using FightSystem.Data;

public class TeamMember : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private bool debugMode = false;
    [SerializeField] private Slider xpBar;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image characterIcon;
    [SerializeField] private Image highlightImage;
    
    [Header("Настройки использования предметов")]
    [SerializeField] private Color highlightColor = new Color(0f, 1f, 0f, 0.3f);
    [SerializeField] private string healSoundName = "Heal";

    private Character character;
    private CharactersSettings settings;
    private bool isInBattle = false;

    public void Initialize(CharactersSettings characterSettings, bool inBattle = false)
    {
        settings = characterSettings;
        isInBattle = inBattle;
        
        if (debugMode)
            Debug.Log($"[TeamMember] Initialize called for {settings?.Name}, inBattle: {inBattle}, character: {(character != null ? character.Name : "null")}");
        
        if (highlightImage != null)
            highlightImage.gameObject.SetActive(false);
        
        UpdateUI();
        
        if (isInBattle && character != null)
        {
            SubscribeToCharacterEvents();
        }
    }

    public void SetCharacter(Character battleCharacter)
    {
        if (character != null)
        {
            UnsubscribeFromCharacterEvents();
        }
        
        character = battleCharacter;
        isInBattle = true;
        
        if (character != null)
        {
            if (debugMode)
                Debug.Log($"[TeamMember] SetCharacter called for {character.Name}, HP: {character.Health}/{character.MaxHealth}, isInBattle set to TRUE");
            SubscribeToCharacterEvents();
            UpdateUI();
        }
        else
        {
            Debug.LogWarning("[TeamMember] SetCharacter called with null character");
        }
    }

    private void SubscribeToCharacterEvents()
    {
        if (character != null)
        {
            character.OnXPChanged += UpdateXPUI;
            character.OnHealthChanged += UpdateHealthUI;
        }
    }

    private void UnsubscribeFromCharacterEvents()
    {
        if (character != null)
        {
            character.OnXPChanged -= UpdateXPUI;
            character.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void OnDestroy()
    {
        UnsubscribeFromCharacterEvents();
    }

    public void UpdateUI()
    {
        if (settings == null)
        {
            Debug.LogWarning("[TeamMember] Settings is null, cannot update UI");
            return;
        }

        // Определяем источник данных: в бою используем character, иначе settings
        IData data = isInBattle && character != null ? (IData)character : settings;
        
        if (debugMode)
        {
            string dataSource = isInBattle && character != null ? "Character (battle)" : "Settings";
            Debug.Log($"[TeamMember] UpdateUI for {data.Name}, isInBattle: {isInBattle}, character: {(character != null ? "exists" : "null")}, dataSource: {dataSource}, HP: {data.Health}/{data.MaxHealth}");
        }

        if (nameText != null)
            nameText.text = data.Name;

        // Спрайт всегда берем из settings (он не меняется в бою)
        if (characterIcon != null)
        {
            UnityEngine.Sprite iconSprite = settings.Sprite;
            if (iconSprite != null)
                characterIcon.sprite = iconSprite;
        }

        // HP и XP берем из data (в бою это character, вне боя - settings)
        UpdateXPUI(data.CurrentXP, data.MaxXP);
        UpdateHealthUI(data.Health, data.MaxHealth);
    }

    private void UpdateXPUI(int current, int max)
    {
        if (xpBar != null)
        {
            xpBar.minValue = 0;
            xpBar.maxValue = max > 0 ? max : 1; // Избегаем деления на 0
            xpBar.value = current;
        }
        
        if (xpText != null)
            xpText.text = $"{current}/{max}";

        if (levelText != null)
        {
            int level = isInBattle && character != null ? character.Level : settings.Level;
            levelText.text = $"Level {level}";
        }
    }

    private void UpdateHealthUI(int current, int max)
    {
        if (debugMode)
            Debug.Log($"[TeamMember] UpdateHealthUI called: {current}/{max}");
        
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = max > 0 ? max : 1; // Избегаем деления на 0
            healthBar.value = current;
            if (debugMode)
                Debug.Log($"[TeamMember] HealthBar updated: value={healthBar.value}, max={healthBar.maxValue}");
        }
        
        if (healthText != null)
        {
            healthText.text = $"{current}/{max}";
            if (debugMode)
                Debug.Log($"[TeamMember] HealthText updated: {healthText.text}");
        }
    }

    public void SyncFromBattle()
    {
        if (character != null && settings != null)
        {
            // Если используется ScriptableObject, обновляем его напрямую
            if (settings.useCharacterData && settings.characterData != null)
            {
                settings.characterData.Health = character.Health;
                settings.characterData.Mana = character.Mana;
                settings.characterData.CurrentXP = character.CurrentXP;
                settings.characterData.MaxXP = character.MaxXP;
                settings.characterData.Level = character.Level;
                settings.characterData.Damage = character.Damage;
                settings.characterData.MaxHealth = character.MaxHealth;
                settings.characterData.Heal = character.Heal;
                settings.characterData.Armor = character.Armor;
                settings.characterData.MaxMana = character.MaxMana;
                settings.characterData.XpReward = character.XpReward;
            }
            else
            {
                // Для ручных настроек используем сеттеры
                settings.Health = character.Health;
                settings.Mana = character.Mana;
                settings.CurrentXP = character.CurrentXP;
                settings.MaxXP = character.MaxXP;
                settings.Level = character.Level;
                settings.Damage = character.Damage;
                settings.MaxHealth = character.MaxHealth;
                settings.Heal = character.Heal;
                settings.Armor = character.Armor;
                settings.MaxMana = character.MaxMana;
                settings.XpReward = character.XpReward;
            }
        }
    }

    // ===== Drag and Drop для использования предметов =====

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (eventData.pointerDrag != null)
        {
            DraggableItem draggedItem = eventData.pointerDrag.GetComponent<DraggableItem>();
            if (draggedItem != null && CanUseItem(draggedItem.itemData))
            {
                ShowHighlight(true);
            }
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ShowHighlight(false);
    }

    private void ShowHighlight(bool show)
    {
        if (highlightImage != null)
        {
            highlightImage.gameObject.SetActive(show);
            if (show)
                highlightImage.color = highlightColor;
        }
    }

    private bool CanUseItem(ItemData item)
    {
        if (item == null || settings == null) return false;
        
        if (!item.itemType.HasFlag(ItemType.Food) && !item.itemType.HasFlag(ItemType.Potion))
            return false;

        IData data = isInBattle && character != null ? (IData)character : GetCharacterData();
        if (data == null) return false;

        bool needsHealth = item.healthRestore > 0 && data.Health < data.MaxHealth;
        bool needsMana = item.manaRestore > 0 && data.Mana < data.MaxMana;
        return needsHealth || needsMana;
    }

    public void OnDrop(PointerEventData eventData)
    {
        ShowHighlight(false);

        DraggableItem draggedItem = eventData.pointerDrag?.GetComponent<DraggableItem>();
        if (draggedItem == null || draggedItem.itemData == null)
            return;

        ItemData item = draggedItem.itemData;

        if (!item.itemType.HasFlag(ItemType.Food) && !item.itemType.HasFlag(ItemType.Potion))
        {
            if (debugMode)
                Debug.Log($"Предмет {item.itemName} нельзя использовать на персонаже");
            return;
        }

        UseItem(item, draggedItem);
    }

    private void UseItem(ItemData item, DraggableItem draggedItem)
    {
        if (settings == null)
        {
            Debug.LogWarning("Персонаж не найден");
            return;
        }

        IData data = isInBattle && character != null ? (IData)character : GetCharacterData();
        if (data == null) return;

        bool itemUsed = false;

        // Восстанавливаем HP
        if (item.healthRestore > 0 && data.Health < data.MaxHealth)
        {
            int oldHealth = data.Health;
            data.Health = Mathf.Min(data.Health + item.healthRestore, data.MaxHealth);
            int actualRestore = data.Health - oldHealth;
            
            if (debugMode)
                Debug.Log($"{data.Name} восстановил {actualRestore} HP");
            itemUsed = true;
        }

        // Восстанавливаем ману
        if (item.manaRestore > 0 && data.Mana < data.MaxMana)
        {
            int oldMana = data.Mana;
            data.Mana = Mathf.Min(data.Mana + item.manaRestore, data.MaxMana);
            int actualRestore = data.Mana - oldMana;
            
            if (debugMode)
                Debug.Log($"{data.Name} восстановил {actualRestore} маны");
            itemUsed = true;
        }

        if (itemUsed)
        {
            PlaySound(healSoundName);

            // Обновляем UI
            if (isInBattle && character != null)
            {
                if (debugMode)
                    Debug.Log($"[TeamMember] Updating battle character UI for {character.Name}");
                character.UpdateUI();
            }
            
            // Всегда обновляем UI TeamMember
            if (debugMode)
                Debug.Log($"[TeamMember] Updating TeamMember UI");
            UpdateUI();

            draggedItem.count--;

            if (draggedItem.count <= 0)
            {
                Destroy(draggedItem.gameObject);
            }
            else
            {
                draggedItem.RefreshCount();
            }

            InventoryManager inventoryManager = FindObjectOfType<InventoryManager>();
            if (inventoryManager != null)
            {
                inventoryManager.SyncFromUI();
            }
        }
        else
        {
            if (debugMode)
                Debug.Log($"{data.Name} не нуждается в восстановлении");
        }
    }

    private IData GetCharacterData()
    {
        if (settings == null) return null;
        
        if (settings.useCharacterData && settings.RuntimeData != null)
        {
            return settings.RuntimeData;
        }
        
        return settings;
    }

    private void PlaySound(string soundName)
    {
        var audioManager = ServiceLocator.GetService();
        if (audioManager != null)
        {
            audioManager.Play(soundName, ChildType.PARENT);
        }
    }
}
