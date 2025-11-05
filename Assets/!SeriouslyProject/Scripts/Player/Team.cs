using FightSystem.Data;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public List<CharactersSettings> characters = new List<CharactersSettings>();
}

[Serializable]
public class CharactersSettings : IData
{
    [Space(1)]
    [LabelWidth(200)]
    [LabelText("Использовать шаблон персонажа")]
    public bool useCharacterData = true;
    [Space(1)]
    [LabelWidth(200)]
    [LabelText("Имя шаблона персонажа (ресурсы)")]
    public string characterDataName;

    [ShowIf("useCharacterData")]
    [LabelText("Шаблон персонажа")]
    [InlineEditor(InlineEditorModes.GUIOnly)]
    public CharacterData characterData;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private string name;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [TextArea(3, 10)]
    [SerializeField] private string description;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _damage;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _priority;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _maxMana;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _mana;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _maxHealth;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _health;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _heal;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _armor;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _lucky;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _creteChance;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _level;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _currentXP;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _maxXP;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _xpReward;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int damagePerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int maxHealthPerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int healPerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int armorPerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int maxManaPerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int xpRewardPerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [LabelText("Спрайт персонажа")]
    [SerializeField] private Sprite sprite;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [LabelText("Путь до спрайта (Resources)")]
    public string spritePath;

    public string Name { get => name; set => name = value; }
    public string Description { get => description; set => description = value; }
    public Sprite Sprite { get => sprite; set => sprite = value; }
    public int Damage { get => _damage; set => _damage = value; }
    public int Priority { get => _priority; set => _priority = value; }
    public int MaxMana { get => _maxMana; set => _maxMana = value; }
    public int Mana { get => _mana; set => _mana = value; }
    public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public int Health { get => _health; set => _health = value; }
    public int Heal { get => _heal; set => _heal = value; }
    public int Armor { get => _armor; set => _armor = value; }
    public int Lucky { get => _lucky; set => _lucky = value; }
    public int CreteChance { get => _creteChance; set => _creteChance = value; }
    public int Level { get => _level; set => _level = value; }
    public int CurrentXP { get => _currentXP; set => _currentXP = value; }
    public int MaxXP { get => _maxXP; set => _maxXP = value; }
    public int XpReward { get => _xpReward; set => _xpReward = value; }
    public int DamagePerLevel { get => damagePerLevel; set => damagePerLevel = value; }
    public int MaxHealthPerLevel { get => maxHealthPerLevel; set => maxHealthPerLevel = value; }
    public int HealPerLevel { get => healPerLevel; set => healPerLevel = value; }
    public int ArmorPerLevel { get => armorPerLevel; set => armorPerLevel = value; }
    public int MaxManaPerLevel { get => maxManaPerLevel; set => maxManaPerLevel = value; }
    public int XpRewardPerLevel { get => xpRewardPerLevel; set => xpRewardPerLevel = value; }

    public Sprite GetSprite()
    {
        if (string.IsNullOrEmpty(spritePath)) return null;
        return Resources.Load<Sprite>(spritePath);
    }

    public CharacterData GetCharacterData()
    {
        if (!useCharacterData || string.IsNullOrEmpty(characterDataName))
            return null;

        return Resources.Load<CharacterData>("CharacterData/" + characterDataName);
    }
}

