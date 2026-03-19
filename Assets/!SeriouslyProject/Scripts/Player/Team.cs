using FightSystem.Data;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public List<CharactersSettings> characters = new List<CharactersSettings>();

    public void AddCharacter(CharacterData characterData)
    {
        if (characterData == null)
        {
            Debug.LogWarning("[Team] CharacterData is null");
            return;
        }

        CharactersSettings newCharacter = new CharactersSettings
        {
            useCharacterData = true,
            characterData = characterData,
            characterDataName = characterData.name
        };

        characters.Add(newCharacter);
    }

    public void AddCharacter(string characterDataName)
    {
        CharacterData data = Resources.Load<CharacterData>("CharacterData/" + characterDataName);

        if (data == null)
        {
            Debug.LogError($"[Team] CharacterData {characterDataName} not found");
            return;
        }

        AddCharacter(data);
    }

    public TeamSaveData CreateSaveData()
    {
        var saveData = new TeamSaveData();
        saveData.charactersData = new List<CharacterSaveData>();

        foreach (var character in characters)
        {
            var src = (character.useCharacterData && character.RuntimeData != null)
                ? (IData)character.RuntimeData
                : (IData)character;

            var charData = new CharacterSaveData
            {
                characterDataName = character.characterDataName,
                useCharacterData = character.useCharacterData,
                Name = src.Name,
                Health = src.Health,
                MaxHealth = src.MaxHealth,
                Mana = src.Mana,
                MaxMana = src.MaxMana,
                Level = src.Level,
                CurrentXP = src.CurrentXP,
                MaxXP = src.MaxXP,
                Damage = src.Damage,
                Heal = src.Heal,
                Armor = src.Armor,
                XpReward = src.XpReward
            };
            saveData.charactersData.Add(charData);
        }

        return saveData;
    }

    public void LoadFromSaveData(TeamSaveData saveData)
    {
        if (saveData == null || saveData.charactersData == null) return;

        characters.Clear();

        foreach (var charData in saveData.charactersData)
        {
            CharactersSettings character = new CharactersSettings();

            character.useCharacterData = charData.useCharacterData;
            character.characterDataName = charData.characterDataName;

            if (character.useCharacterData)
            {
                character.characterData = Resources.Load<CharacterData>("CharacterData/" + charData.characterDataName);

                if (character.characterData == null)
                {
                    Debug.LogError($"[Team] CharacterData {charData.characterDataName} not found in Resources/CharacterData");
                    continue;
                }
            }

            characters.Add(character);

            // Теперь RuntimeData гарантированно создаётся
            if (character.useCharacterData && character.RuntimeData != null)
            {
                character.RuntimeData.Health = charData.Health;
                character.RuntimeData.MaxHealth = charData.MaxHealth;
                character.RuntimeData.Mana = charData.Mana;
                character.RuntimeData.MaxMana = charData.MaxMana;
                character.RuntimeData.Level = charData.Level;
                character.RuntimeData.CurrentXP = charData.CurrentXP;
                character.RuntimeData.MaxXP = charData.MaxXP;
                character.RuntimeData.Damage = charData.Damage;
                character.RuntimeData.Heal = charData.Heal;
                character.RuntimeData.Armor = charData.Armor;
                character.RuntimeData.XpReward = charData.XpReward;
            }
        }
    }
}

[Serializable]
public class TeamSaveData
{
    public List<CharacterSaveData> charactersData;
}

[Serializable]
public class CharacterSaveData
{
    public string characterDataName;
    public bool useCharacterData;
    public string Name;
    public int Health;
    public int MaxHealth;
    public int Mana;
    public int MaxMana;
    public int Level;
    public int CurrentXP;
    public int MaxXP;
    public int Damage;
    public int Heal;
    public int Armor;
    public int XpReward;
}

[Serializable]
public class CharactersSettings : IData
{
    [Space(1)]
    [LabelWidth(200)]
    [LabelText("Использовать данные персонажа")]
    public bool useCharacterData = true;

    [Space(1)]
    [LabelWidth(200)]
    [LabelText("Имя файла данных персонажа (Resources)")]
    public string characterDataName;

    [ShowIf("useCharacterData")]
    [LabelText("Данные персонажа")]
    [InlineEditor(InlineEditorModes.GUIOnly)]
    public CharacterData characterData;

    // Runtime копия для изменений во время игры
    [System.NonSerialized]
    private CharacterDataRuntime runtimeData;

    // Геттер для runtime данных
    public CharacterDataRuntime RuntimeData
    {
        get
        {
            if (useCharacterData && characterData != null && runtimeData == null)
            {
                runtimeData = CharacterDataRuntime.CreateFromScriptableObject(characterData);
            }
            return runtimeData;
        }
    }

    public void ResetRuntimeData()
    {
        runtimeData = null;
    }

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private string name;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [TextArea(3, 10)]
    [SerializeField] private string description;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _damage;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _magicDamage;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _priority;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _maxMana;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _mana;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _maxHealth;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _health;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _heal;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _armor;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _lucky;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _creteChance;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _level;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _currentXP;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _maxXP;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int _xpReward;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int damagePerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int maxHealthPerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int healPerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int armorPerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int maxManaPerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [SerializeField] private int xpRewardPerLevel = 1;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [LabelText("Спрайт персонажа")]
    [SerializeField] private Sprite sprite;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры персонажа")]
    [LabelText("Путь к спрайту (Resources)")]
    public string spritePath;

    public string Name
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.Name : name;
        set => name = value;
    }

    public string Description
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.Description : description;
        set => description = value;
    }

    public Sprite Sprite
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.Sprite : sprite;
        set => sprite = value;
    }

    public int Damage
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.Damage : _damage;
        set
        {
            _damage = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.Damage = value;
        }
    }

    public int MagicDamage
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.MagicDamage : _magicDamage;
        set
        {
            _magicDamage = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.MagicDamage = value;
        }
    }

    public int Priority
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.Priority : _priority;
        set
        {
            _priority = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.Priority = value;
        }
    }

    public int MaxMana
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.MaxMana : _maxMana;
        set
        {
            _maxMana = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.MaxMana = value;
        }
    }

    public int Mana
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.Mana : _mana;
        set
        {
            _mana = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.Mana = value;
        }
    }

    public int MaxHealth
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.MaxHealth : _maxHealth;
        set
        {
            _maxHealth = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.MaxHealth = value;
        }
    }

    public int Health
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.Health : _health;
        set
        {
            _health = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.Health = value;
        }
    }

    public int Heal
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.Heal : _heal;
        set
        {
            _heal = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.Heal = value;
        }
    }

    public int Armor
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.Armor : _armor;
        set
        {
            _armor = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.Armor = value;
        }
    }

    public int Lucky
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.Lucky : _lucky;
        set
        {
            _lucky = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.Lucky = value;
        }
    }

    public int CreteDamage
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.CreteDamage : _creteChance;
        set
        {
            _creteChance = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.CreteDamage = value;
        }
    }

    public int Level
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.Level : _level;
        set
        {
            _level = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.Level = value;
        }
    }

    public int CurrentXP
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.CurrentXP : _currentXP;
        set
        {
            _currentXP = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.CurrentXP = value;
        }
    }

    public int MaxXP
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.MaxXP : _maxXP;
        set
        {
            _maxXP = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.MaxXP = value;
        }
    }

    public int XpReward
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.XpReward : _xpReward;
        set
        {
            _xpReward = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.XpReward = value;
        }
    }

    public int DamagePerLevel
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.DamagePerLevel : damagePerLevel;
        set
        {
            damagePerLevel = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.DamagePerLevel = value;
        }
    }

    public int MaxHealthPerLevel
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.MaxHealthPerLevel : maxHealthPerLevel;
        set
        {
            maxHealthPerLevel = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.MaxHealthPerLevel = value;
        }
    }

    public int HealPerLevel
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.HealPerLevel : healPerLevel;
        set
        {
            healPerLevel = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.HealPerLevel = value;
        }
    }

    public int ArmorPerLevel
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.ArmorPerLevel : armorPerLevel;
        set
        {
            armorPerLevel = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.ArmorPerLevel = value;
        }
    }

    public int MaxManaPerLevel
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.MaxManaPerLevel : maxManaPerLevel;
        set
        {
            maxManaPerLevel = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.MaxManaPerLevel = value;
        }
    }

    public int XpRewardPerLevel
    {
        get => useCharacterData && RuntimeData != null ? RuntimeData.XpRewardPerLevel : xpRewardPerLevel;
        set
        {
            xpRewardPerLevel = value;
            if (useCharacterData && RuntimeData != null)
                RuntimeData.XpRewardPerLevel = value;
        }
    }

    [HideInInspector]
    public string AttackAnimationName { get => ""; set { } }

    public Sprite GetSprite()
    {
        if (useCharacterData && RuntimeData != null)
            return RuntimeData.Sprite;

        if (string.IsNullOrEmpty(spritePath))
            return sprite;

        return Resources.Load<Sprite>(spritePath);
    }

    public CharacterData GetCharacterData()
    {
        if (!useCharacterData || string.IsNullOrEmpty(characterDataName))
            return null;

        return Resources.Load<CharacterData>("CharacterData/" + characterDataName);
    }
}

