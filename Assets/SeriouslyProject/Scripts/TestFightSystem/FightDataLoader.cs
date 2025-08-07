using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Sirenix.OdinInspector;
using FightSystem.Data;
using FightSystem.Enemy;
using FightSystem.Character;

public class FightDataLoader : MonoBehaviour
{
    [Title("Враги")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private RectTransform spawnParent;

    [Title("Персонажи")]
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private RectTransform characterSpawnParent;

    private const string EnemySavePath = "enemies_data.json";
    private const string CharacterSavePath = "characters_data.json";

    private void Awake()
    {
        LoadFightData();
        LoadCharactersData();
    }

    [Button("Загрузить врагов из JSON")]
    private void LoadFightData()
    {
        string path = Path.Combine(Application.persistentDataPath, EnemySavePath);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[ВРАГИ] Файл не найден: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        Debug.Log($"[ВРАГИ] JSON загружен: {json}");

        FightData fightData = JsonUtility.FromJson<FightData>(json);

        if (fightData?.enemies == null || fightData.enemies.Count == 0)
        {
            Debug.LogWarning("[ВРАГИ] Список врагов пуст или ошибка десериализации.");
            return;
        }

        foreach (var enemySettings in fightData.enemies)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnParent);
            Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
            enemyComponent.InitializeFromSettings(enemySettings);
            Debug.Log($"[ВРАГ] Создан: {enemySettings._name}");
        }
    }

    [Button("Загрузить персонажей из JSON")]
    private void LoadCharactersData()
    {
        string path = Path.Combine(Application.persistentDataPath, CharacterSavePath);

        if (!File.Exists(path))
        {
            Debug.LogWarning($"[ПЕРСОНАЖИ] Файл не найден: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        Debug.Log($"[ПЕРСОНАЖИ] JSON загружен: {json}");

        CharacterDataWrapper characterData = JsonUtility.FromJson<CharacterDataWrapper>(json);

        if (characterData?.characters == null || characterData.characters.Count == 0)
        {
            Debug.LogWarning("[ПЕРСОНАЖИ] Список персонажей пуст или ошибка десериализации.");
            return;
        }

        foreach (var characterSettings in characterData.characters)
        {
            GameObject newCharacter = Instantiate(characterPrefab, characterSpawnParent);
            Character characterComponent = newCharacter.GetComponent<Character>();
            characterComponent.InitializeFromSettings(characterSettings);
            Debug.Log($"[ПЕРСОНАЖ] Создан: {characterSettings._name}");
        }
    }
}

[System.Serializable]
public class FightData
{
    public List<EnemyesSettings> enemies;
}

[System.Serializable]
public class CharacterDataWrapper
{
    public List<CharactersSettings> characters;
}
