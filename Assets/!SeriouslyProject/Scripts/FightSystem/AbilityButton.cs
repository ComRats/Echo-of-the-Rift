using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Компонент для кнопки способности
/// Автоматически настраивается через AbilityManager
/// </summary>
[RequireComponent(typeof(Button))]
public class AbilityButton : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI abilityNameText;
    [SerializeField] private TextMeshProUGUI manaCostText;
    [SerializeField] private Image abilityIcon;
    [SerializeField] private Image backgroundImage;

    [Header("Visual Settings")]
    [SerializeField] private Color normalColor = Color.white;
    [SerializeField] private Color notEnoughManaColor = Color.gray;

    private Button button;
    private BattleAbility ability;
    private Base character;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    /// <summary>
    /// Настроить кнопку для способности
    /// </summary>
    public void Setup(BattleAbility ability, Base character, Sprite icon = null)
    {
        this.ability = ability;
        this.character = character;

        if (abilityNameText != null)
        {
            abilityNameText.text = ability.AbilityName;
        }

        if (manaCostText != null)
        {
            manaCostText.text = ability.ManaCost > 0 ? ability.ManaCost.ToString() : "";
        }

        if (abilityIcon != null && icon != null)
        {
            abilityIcon.sprite = icon;
            abilityIcon.enabled = true;
        }
        else if (abilityIcon != null)
        {
            abilityIcon.enabled = false;
        }

        UpdateVisuals();
    }

    /// <summary>
    /// Обновить визуальное состояние кнопки
    /// </summary>
    public void UpdateVisuals()
    {
        if (ability == null || character == null) return;

        bool canUse = ability.CanUse(character);
        
        if (button != null)
        {
            button.interactable = canUse;
        }

        if (backgroundImage != null)
        {
            backgroundImage.color = canUse ? normalColor : notEnoughManaColor;
        }
    }

    /// <summary>
    /// Получить способность кнопки
    /// </summary>
    public BattleAbility GetAbility()
    {
        return ability;
    }
}
