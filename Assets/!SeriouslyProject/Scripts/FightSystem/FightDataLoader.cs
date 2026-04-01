using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FightSystem.Enemy;
using FightSystem.Character;
using EchoRift.EchoRiftSaveLoadSystem;
using static EchoRift.EchoRiftSaveLoadSystem.SaveFileNames;

public class FightDataLoader : MonoBehaviour
{
    [SerializeField] private bool debugMode = false;
    
    [Title("Враги")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private RectTransform spawnParent;

    [Title("Персонажи")]
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private RectTransform characterSpawnParent;

    private void Awake()
    {
        LoadFightData();
        LoadCharactersData();
    }

    [Button("Загрузить врагов")]
    private void LoadFightData()
    {
        FightData fightData = SaveLoadSystem.Load<FightData>(ENEMY_SAVE, GAME_DIRECTORY);

        if (fightData?.enemies == null || fightData.enemies.Count == 0)
        {
            Debug.LogWarning("[Враги] Данные врагов пусты или равны нулю.");
            return;
        }

        foreach (var enemySettings in fightData.enemies)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnParent);
            Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
            enemyComponent.InitializeFromSettings(enemySettings);
            if (debugMode)
                Debug.Log($"[Враг] Создан: {enemySettings.Name}");
        }
    }

    [Button("Загрузить персонажей")]
    private void LoadCharactersData()
    {
        if (GlobalLoader.Instance != null && 
            GlobalLoader.Instance.playerInstance != null)
        {
            var team = GlobalLoader.Instance.playerInstance.team;
            if (team != null && team.characters != null && team.characters.Count > 0)
            {
                if (debugMode)
                    Debug.Log("[Персонажи] Загрузка из Team с runtime данными");
                foreach (var characterSettings in team.characters)
                {
                    GameObject newCharacter = Instantiate(characterPrefab, characterSpawnParent);
                    Character characterComponent = newCharacter.GetComponent<Character>();
                    characterComponent.InitializeFromSettings(characterSettings);
                    if (debugMode)
                        Debug.Log($"[Персонаж] Создан: {characterSettings.Name} HP:{characterSettings.Health}/{characterSettings.MaxHealth} XP:{characterSettings.CurrentXP}/{characterSettings.MaxXP}");
                }
                return;
            }
        }

        Debug.LogWarning("[Персонажи] GlobalLoader не найден, загрузка из файла");
        CharacterDataWrapper characterData = SaveLoadSystem.Load<CharacterDataWrapper>(CHARACTER_SAVE, GAME_DIRECTORY);

        if (characterData?.characters == null || characterData.characters.Count == 0)
        {
            Debug.LogWarning("[Персонажи] Данные персонажей пусты или равны нулю.");
            return;
        }

        foreach (var characterSettings in characterData.characters)
        {
            GameObject newCharacter = Instantiate(characterPrefab, characterSpawnParent);
            Character characterComponent = newCharacter.GetComponent<Character>();
            characterComponent.InitializeFromSettings(characterSettings);
            if (debugMode)
                Debug.Log($"[Персонаж] Создан: {characterSettings.Name}");
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
