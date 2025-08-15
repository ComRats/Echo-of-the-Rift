using FightSystem.Data;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Team : MonoBehaviour
{
    public List<CharactersSettings> characters = new List<CharactersSettings>();
}

[Serializable]
public class CharactersSettings
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
    [FoldoutGroup("Параметры вручную")] public string _name;
    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")][TextArea(3, 10)] public string _description;
    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")] public int _damage;
    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")] public int _priority;
    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")] public int _maxMana;
    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")] public int _mana;
    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")] public int _maxHealth;
    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")] public int _health;
    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")] public int _heal;
    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")] public int _armor;

    [HideIf("useCharacterData")]
    [FoldoutGroup("Параметры вручную")]
    [LabelText("Путь до спрайта (Resources)")]
    public string spritePath;

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

