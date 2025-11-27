using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
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

    private const string EnemySaveFile = "enemies_data";
    private const string CharacterSaveFile = "characters_data";

    private void Awake()
    {
        LoadFightData();
        LoadCharactersData();
    }

    [Button("Загрузить врагов")]
    private void LoadFightData()
    {
        FightData fightData = SaveLoadSystem.Load<FightData>(EnemySaveFile);

        if (fightData?.enemies == null || fightData.enemies.Count == 0)
        {
            Debug.LogWarning("[ВРАГИ] Список врагов пуст или файла нет.");
            return;
        }

        foreach (var enemySettings in fightData.enemies)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnParent);
            Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
            enemyComponent.InitializeFromSettings(enemySettings);
            Debug.Log($"[ВРАГ] Создан: {enemySettings.Name}");
        }
    }

    [Button("Загрузить персонажей")]
    private void LoadCharactersData()
    {
        CharacterDataWrapper characterData = SaveLoadSystem.Load<CharacterDataWrapper>(CharacterSaveFile);

        if (characterData?.characters == null || characterData.characters.Count == 0)
        {
            Debug.LogWarning("[ПЕРСОНАЖИ] Список персонажей пуст или файла нет.");
            return;
        }

        foreach (var characterSettings in characterData.characters)
        {
            GameObject newCharacter = Instantiate(characterPrefab, characterSpawnParent);
            Character characterComponent = newCharacter.GetComponent<Character>();
            characterComponent.InitializeFromSettings(characterSettings);
            Debug.Log($"[ПЕРСОНАЖ] Создан: {characterSettings.Name}");
        }
    }
}

[System.Serializable]
public class FightData
{
    public List<EnemiesSettings> enemies = new();
}

[System.Serializable]
public class CharacterDataWrapper
{
    public List<CharactersSettings> characters = new();
}
