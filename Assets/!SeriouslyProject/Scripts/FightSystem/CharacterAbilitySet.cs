using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Ability Set", menuName = "Battle/Character Ability Set")]
public class CharacterAbilitySet : ScriptableObject
{
    [Title("Character Abilities")]
    [Tooltip("Все способности персонажа с требованиями уровня")]
    [ListDrawerSettings(ShowIndexLabels = true)]
    public List<CharacterAbility> abilities = new List<CharacterAbility>();

    [Title("Active Abilities")]
    [Tooltip("Индексы способностей, которые игрок выбрал для использования в бою")]
    [InfoBox("Здесь можно управлять, какие способности доступны в бою")]
    public List<int> activeAbilityIndices = new List<int>();

    /// <summary>
    /// Получить все разблокированные способности для текущего уровня
    /// </summary>
    public List<CharacterAbility> GetUnlockedAbilities(int characterLevel)
    {
        return abilities.Where(a => a.IsUnlocked(characterLevel)).ToList();
    }

    public List<CharacterAbility> GetActiveAbilities(int characterLevel)
    {
        if (activeAbilityIndices.Count == 0)
        {
            return abilities
                .Where(a => a.IsUnlocked(characterLevel) && a.isActiveByDefault)
                .ToList();
        }

        List<CharacterAbility> result = new List<CharacterAbility>();
        foreach (int index in activeAbilityIndices)
        {
            if (index >= 0 && index < abilities.Count)
            {
                var ability = abilities[index];
                if (ability.IsUnlocked(characterLevel))
                {
                    result.Add(ability);
                }
            }
        }
        return result;
    }

    /// <summary>
    /// Получить способности по типу
    /// </summary>
    public List<CharacterAbility> GetAbilitiesByType(int characterLevel, AbilityType type)
    {
        return GetActiveAbilities(characterLevel)
            .Where(a => a.abilityType == type)
            .ToList();
    }

    /// <summary>
    /// Добавить способность в активные
    /// </summary>
    public void ActivateAbility(int abilityIndex)
    {
        if (!activeAbilityIndices.Contains(abilityIndex))
        {
            activeAbilityIndices.Add(abilityIndex);
        }
    }

    /// <summary>
    /// Убрать способность из активных
    /// </summary>
    public void DeactivateAbility(int abilityIndex)
    {
        activeAbilityIndices.Remove(abilityIndex);
    }
}
