using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using FightSystem.Enemy;
using FightSystem.Character;
using EchoRift.SaveLoadSystem;
using static EchoRift.SaveLoadSystem.SaveFileNames;

public class FightDataLoader : MonoBehaviour
{
    [Title("�����")]
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private RectTransform spawnParent;

    [Title("���������")]
    [SerializeField] private GameObject characterPrefab;
    [SerializeField] private RectTransform characterSpawnParent;

    private void Awake()
    {
        LoadFightData();
        LoadCharactersData();
    }

    [Button("��������� ������")]
    private void LoadFightData()
    {
        FightData fightData = SaveLoadSystem.Load<FightData>(ENEMY_SAVE, GAME_DIRECTORY);

        if (fightData?.enemies == null || fightData.enemies.Count == 0)
        {
            Debug.LogWarning("[�����] ������ ������ ���� ��� ����� ���.");
            return;
        }

        foreach (var enemySettings in fightData.enemies)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnParent);
            Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
            enemyComponent.InitializeFromSettings(enemySettings);
            Debug.Log($"[����] ������: {enemySettings.Name}");
        }
    }

    [Button("��������� ����������")]
    private void LoadCharactersData()
    {
        CharacterDataWrapper characterData = SaveLoadSystem.Load<CharacterDataWrapper>(CHARACTER_SAVE, GAME_DIRECTORY);

        if (characterData?.characters == null || characterData.characters.Count == 0)
        {
            Debug.LogWarning("[���������] ������ ���������� ���� ��� ����� ���.");
            return;
        }

        foreach (var characterSettings in characterData.characters)
        {
            GameObject newCharacter = Instantiate(characterPrefab, characterSpawnParent);
            Character characterComponent = newCharacter.GetComponent<Character>();
            characterComponent.InitializeFromSettings(characterSettings);
            Debug.Log($"[��������] ������: {characterSettings.Name}");
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
