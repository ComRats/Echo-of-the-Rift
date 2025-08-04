using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Sirenix.OdinInspector;
using FightSystem.Data;
using FightSystem.Enemy;

public class FightDataLoader : MonoBehaviour
{
    [SerializeField] private GameObject enemyPrefab;
    [SerializeField] private RectTransform spawnParent;

    private void Awake()
    {
        LoadFightData();
    }

    [Button("Загрузить врагов из JSON")]
    private void LoadFightData()
    {
        string path = Path.Combine(Application.persistentDataPath, "enemies_data.json");

        if (!File.Exists(path))
        {
            Debug.LogWarning($"Файл не найден: {path}");
            return;
        }

        string json = File.ReadAllText(path);
        Debug.Log($"JSON загружен: {json}");

        FightData fightData = JsonUtility.FromJson<FightData>(json);

        if (fightData == null)
        {
            Debug.LogError("Ошибка десериализации fightData!");
            return;
        }

        if (fightData.enemies == null || fightData.enemies.Count == 0)
        {
            Debug.LogWarning("Список врагов пуст!");
            return;
        }

        Debug.Log($"Врагов для создания: {fightData.enemies.Count}");

        foreach (var enemySettings in fightData.enemies)
        {
            GameObject newEnemy = Instantiate(enemyPrefab, spawnParent);
            Enemy enemyComponent = newEnemy.GetComponent<Enemy>();

            enemyComponent.InitializeFromSettings(enemySettings);
            Debug.Log($"Создан враг: {enemySettings._name}");
        }
    }

}

[System.Serializable]
public class FightData
{
    public List<EnemyesSettings> enemies;
}
