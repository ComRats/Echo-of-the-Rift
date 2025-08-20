using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveLoadSystem
{
    private static Dictionary<string, object> cache = new();

    public static string GetPath(string fileName)
    {
        return Path.Combine(Application.persistentDataPath, $"{fileName}.json");
    }

    /// <summary>
    /// Универсальное сохранение любых данных в JSON.
    /// Обновляет кеш и файл.
    /// </summary>
    public static void Save<T>(string fileName, T data)
    {
        string path = GetPath(fileName);
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);

        cache[fileName] = data; // обновляем кеш
    }

    /// <summary>
    /// Универсальная загрузка любых данных из JSON.
    /// При первом вызове читает файл, дальше берёт из кеша.
    /// </summary>
    public static T Load<T>(string fileName) where T : new()
    {
        if (cache.TryGetValue(fileName, out object cached) && cached is T cachedData)
        {
            return cachedData;
        }

        string path = GetPath(fileName);

        if (!File.Exists(path))
        {
            var newData = new T();
            cache[fileName] = newData;
            return newData;
        }

        string json = File.ReadAllText(path);
        T data = JsonUtility.FromJson<T>(json);

        cache[fileName] = data;
        return data;
    }

    /// <summary>
    /// Проверка существования файла сохранения.
    /// </summary>
    public static bool Exists(string fileName)
    {
        return File.Exists(GetPath(fileName));
    }

    /// <summary>
    /// Удаление сохранения (и из кеша тоже).
    /// </summary>
    public static void Delete(string fileName)
    {
        string path = GetPath(fileName);
        if (File.Exists(path))
            File.Delete(path);

        if (cache.ContainsKey(fileName))
            cache.Remove(fileName);
    }

    /// <summary>
    /// Очистить весь кеш (например при смене профиля игрока).
    /// </summary>
    public static void ClearCache()
    {
        cache.Clear();
    }
}
