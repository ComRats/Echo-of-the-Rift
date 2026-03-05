using FightSystem.Character;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamMember : MonoBehaviour
{
    [SerializeField] private Slider xpBar;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;
    [SerializeField] private TextMeshProUGUI nameText;
    [SerializeField] private Image characterIcon;

    private Character character;
    private CharactersSettings settings;
    private bool isInBattle = false;

    public void Initialize(CharactersSettings characterSettings, bool inBattle = false)
    {
        settings = characterSettings;
        isInBattle = inBattle;
        
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
            SubscribeToCharacterEvents();
            UpdateUI();
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

        IData data = isInBattle && character != null ? (IData)character : settings;

        if (nameText != null)
            nameText.text = data.Name;

        if (characterIcon != null && data.Sprite != null)
            characterIcon.sprite = data.Sprite;

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
        if (healthBar != null)
        {
            healthBar.minValue = 0;
            healthBar.maxValue = max > 0 ? max : 1; // Избегаем деления на 0
            healthBar.value = current;
        }
        
        if (healthText != null)
            healthText.text = $"{current}/{max}";
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
}
