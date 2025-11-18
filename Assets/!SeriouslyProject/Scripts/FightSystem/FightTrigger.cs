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

    private const string EnemySavePath = "enemies_data.json";

    [Header("CharcterFightSettings")]
    [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = true)]
    private List<CharactersSettings> characters = new List<CharactersSettings>();

    private const string CharacterSavePath = "characters_data.json";

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
        Debug.Log($"[FightTrigger] Персонажи сохранены: {Path.Combine(Application.persistentDataPath, CharacterSavePath)}");
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
    [FoldoutGroup("Параметры вручную")] public string _name;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")][TextArea(3, 10)] public string _description;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _damage;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _priority;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _maxMana;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _mana;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _maxHealth;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _health;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _heal;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _armor;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _xpReward;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [LabelText("Путь до спрайта (Resources)")]
    public string spritePath;

    public Sprite GetSprite()
    {
        if (string.IsNullOrEmpty(spritePath)) return null;
        return Resources.Load<Sprite>(spritePath);
    }

    public EnemyData GetEnemyData()
    {
        if (!useEnemyData || string.IsNullOrEmpty(enemyDataName))
            return null;

        return Resources.Load<EnemyData>("EnemyData/"+enemyDataName);
    }
}
