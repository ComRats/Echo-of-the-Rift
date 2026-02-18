using UnityEngine;
using Sirenix.OdinInspector;

[System.Serializable]
public class CharacterAbility
{
    [Title("Ability Info")]
    public BattleAbility ability;
    
    [Title("Unlock Requirements")]
    [Tooltip("Минимальный уровень для разблокировки способности")]
    public int requiredLevel = 1;
    
    [Title("Ability Type")]
    public AbilityType abilityType = AbilityType.Physical;
    
    [Title("UI Settings")]
    [Tooltip("Иконка способности для кнопки")]
    public Sprite abilityIcon;
    
    [Tooltip("Активна ли способность по умолчанию (можно использовать в бою)")]
    public bool isActiveByDefault = true;

    public bool IsUnlocked(int characterLevel)
    {
        return characterLevel >= requiredLevel;
    }
}

public enum AbilityType
{
    Physical,   // Физическая атака
    Magic,      // Магическая атака
    Defense,    // Защита/блок
    Support     // Поддержка/хил
}
