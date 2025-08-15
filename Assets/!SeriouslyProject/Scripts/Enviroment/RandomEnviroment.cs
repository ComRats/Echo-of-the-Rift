using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class RandomEnviroment : MonoBehaviour
{
    [Title("Ссылки на объекты")]
    [SerializeField] private GameObject backGrass;
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
        if (backGrass == null)
        {
            Debug.LogWarning("BackGrass не назначен!");
            return;
        }

        SpriteRenderer spriteRenderer = backGrass.GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogWarning("У объекта BackGrass отсутствует SpriteRenderer!");
            return;
        }

        Bounds bounds = spriteRenderer.bounds;

        for (int i = 0; i < spawnCount; i++)
        {
            // Случайная позиция внутри bounds по X и Y
            float randomX = Random.Range(bounds.min.x, bounds.max.x);
            float randomY = Random.Range(bounds.min.y, bounds.max.y);
            float z = backGrass.transform.position.z; // Или другое значение для слоя

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
