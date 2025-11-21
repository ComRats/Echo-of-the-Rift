using System.Collections.Generic;
using Sirenix.OdinInspector;
using FightSystem.Data;
using UnityEngine;
using System;
using System.IO;
using Zenject;
using EchoRift;

[RequireComponent(typeof(Collider2D))]
public class FightTrigger : MonoBehaviour
{
    [Header("OtherSettings")]
    [SerializeField] private string nextSceneToLoad = "TestScene";
    [SerializeField] private Vector3 nextPositionToLoad;

    [Header("EnemyFightSettings")]
    [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = true)]
    [SerializeField] private List<EnemyesSettings> enemies = new List<EnemyesSettings>();

    private const string EnemySavePath = "enemies_data";

    [Header("CharcterFightSettings")]
    [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = true)]
    private List<CharactersSettings> characters = new List<CharactersSettings>();

    private const string CharacterSavePath = "characters_data";

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            characters = collision.GetComponent<Team>().characters;
            EnterTrigger();
        }
    }

    private void EnterTrigger()
    {
        SaveEnemiesToFile();
        SaveCharactersToFile();

        //Убирать UI и Игрока

        GlobalLoader.Instance.LoadToScene(nextSceneToLoad, nextPositionToLoad);
    }

    [Button("Сохранить врагов в файл")]
    private void SaveEnemiesToFile()
    {
        FightData data = new FightData { enemies = this.enemies };
        SaveLoadSystem.Save(EnemySavePath, data);
        Debug.Log($"[FightTrigger] Враги сохранены: {Path.Combine(Application.persistentDataPath, CharacterSavePath)}");
    }

    private void SaveCharactersToFile()
    {
        CharacterDataWrapper data = new CharacterDataWrapper { characters = this.characters };
        SaveLoadSystem.Save(CharacterSavePath, data);
        Debug.LogError($"[FightTrigger] Персонажи сохранены: {Path.Combine(Application.persistentDataPath, CharacterSavePath)}");
    }

    [Serializable]
    private class CharacterDataWrapper
    {
        public List<CharactersSettings> characters;
    }
}

[Serializable]
public class EnemyesSettings
{
    [Space(1)]
    [LabelWidth(200)]
    [LabelText("Использовать шаблон врага")]
    public bool useEnemyData = true;

    [Space(1)]
    [LabelWidth(200)]
    [LabelText("Имя шаблона врага (ресурсы)")]
    public string enemyDataName;

    [ShowIf("useEnemyData")]
    [LabelText("Шаблон врага")]
    [InlineEditor(InlineEditorModes.GUIOnly)]
    public EnemyData enemyData;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private string name;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [TextArea(3, 10)]
    [SerializeField] private string description;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _damage;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _priority;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _maxMana;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _mana;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _maxHealth;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _health;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _heal;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _armor;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _lucky;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _creteChance;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _level;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _currentXP;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _maxXP;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int _xpReward;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int damagePerLevel = 1;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int maxHealthPerLevel = 1;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int healPerLevel = 1;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int armorPerLevel = 1;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int maxManaPerLevel = 1;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [SerializeField] private int xpRewardPerLevel = 1;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [LabelText("Спрайт врага")]
    [SerializeField] private Sprite sprite;

    [HideIf("useEnemyData")]
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

    public EnemyData GetEnemyData()
    {
        if (!useEnemyData || string.IsNullOrEmpty(enemyDataName))
            return null;

        return Resources.Load<EnemyData>("EnemyData/" + enemyDataName);
    }
}
