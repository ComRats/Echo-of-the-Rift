using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RandomEnviroment : MonoBehaviour
{
    [Title("Объекты для спавна")]
    [SerializeField] private List<GameObject> backGrass;
    [SerializeField] private List<GameObject> stone;
    [SerializeField] private List<GameObject> grass;
    [SerializeField] private List<GameObject> flowers;

    [Title("Настройки спавна")]
    [SerializeField, Min(1)] private int spawnCount = 100;

    [SerializeField, Range(0f, 1f)] private float stoneSpawnChance = 0.5f;
    [SerializeField, Range(0f, 1f)] private float grassSpawnChance = 0.5f;
    [SerializeField, Range(0f, 1f)] private float flowerSpawnChance = 0.5f;

    [Button("Сгенерировать окружение")]
    private void GenerateEnvironment()
    {
        if (backGrass == null || backGrass.Count == 0)
        {
            Debug.LogWarning("BackGrass не назначен!");
            return;
        }

        // Объединяем combined bounds из всех backGrass объектов
        Bounds combinedBounds = new Bounds();
        bool boundsInitialized = false;

        foreach (var bg in backGrass)
        {
            if (bg == null) continue;

            SpriteRenderer sr = bg.GetComponent<SpriteRenderer>();
            if (sr == null)
            {
                Debug.LogWarning($"У объекта {bg.name} отсутствует SpriteRenderer!");
                continue;
            }

            if (!boundsInitialized)
            {
                combinedBounds = sr.bounds;
                boundsInitialized = true;
            }
            else
            {
                combinedBounds.Encapsulate(sr.bounds);
            }
        }

        if (!boundsInitialized)
        {
            Debug.LogWarning("Не удалось собрать bounds!");
            return;
        }

        float z = backGrass[0].transform.position.z;

        for (int i = 0; i < spawnCount; i++)
        {
            float randomX = Random.Range(combinedBounds.min.x, combinedBounds.max.x);
            float randomY = Random.Range(combinedBounds.min.y, combinedBounds.max.y);

            Vector3 spawnPos = new Vector3(randomX, randomY, z);

            float roll = Random.value;
            GameObject prefabToSpawn = null;

            if (roll < stoneSpawnChance && stone.Count > 0)
            {
                prefabToSpawn = stone[Random.Range(0, stone.Count)];
            }
            else if (roll < stoneSpawnChance + grassSpawnChance && grass.Count > 0)
            {
                prefabToSpawn = grass[Random.Range(0, grass.Count)];
            }
            else if (roll < stoneSpawnChance + grassSpawnChance + flowerSpawnChance && flowers.Count > 0)
            {
                prefabToSpawn = flowers[Random.Range(0, flowers.Count)];
            }

            if (prefabToSpawn != null)
            {
                Instantiate(prefabToSpawn, spawnPos, Quaternion.identity, transform);
            }
        }
    }
}
