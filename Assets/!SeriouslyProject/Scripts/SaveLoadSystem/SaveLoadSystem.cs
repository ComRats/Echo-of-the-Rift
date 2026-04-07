using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace EchoRift.EchoRiftSaveLoadSystem
{
    public static class SaveLoadSystem
    {
        private static Dictionary<string, object> cache = new();

        public static string GetPath(string fileName)
        {
            return Path.Combine(Application.persistentDataPath, $"{fileName}.json");
        }

        public static string GetPath(string fileName, string folderName = "")
        {
            string root = Application.persistentDataPath;

            if (!string.IsNullOrEmpty(folderName))
            {
                root = Path.Combine(root, folderName);

                if (!Directory.Exists(root))
                {
                    Directory.CreateDirectory(root);
                }
            }

            return Path.Combine(root, $"{fileName}.json");
        }

        public static void Save<T>(string fileName, T data)
        {
            string path = GetPath(fileName);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            cache[fileName] = data;
        }

        public static void Save<T>(string fileName, T data, string folderName = "")
        {
            string path = GetPath(fileName, folderName);
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(path, json);

            string cacheKey = string.IsNullOrEmpty(folderName) ? fileName : $"{folderName}/{fileName}";
            cache[cacheKey] = data;
        }

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

        public static T Load<T>(string fileName, string folderName = "") where T : new()
        {
            string cacheKey = string.IsNullOrEmpty(folderName) ? fileName : $"{folderName}/{fileName}";

            if (cache.TryGetValue(cacheKey, out object cached) && cached is T cachedData)
            {
                return cachedData;
            }

            string path = GetPath(fileName, folderName);

            if (!File.Exists(path))
            {
                var newData = new T();
                cache[cacheKey] = newData;
                return newData;
            }

            string json = File.ReadAllText(path);
            T data = JsonUtility.FromJson<T>(json);

            cache[cacheKey] = data;
            return data;
        }

        public static bool Exists(string fileName)
        {
            return File.Exists(GetPath(fileName));
        }

        public static bool Exists(string fileName, string folderName = "")
        {
            return File.Exists(GetPath(fileName, folderName));
        }

        public static void Delete(string fileName)
        {
            string path = GetPath(fileName);
            if (File.Exists(path))
                File.Delete(path);

            if (cache.ContainsKey(fileName))
                cache.Remove(fileName);
        }

        public static void ClearCache()
        {
            cache.Clear();
        }

        public static void ClearAllSaves()
        {
            string dir = Application.persistentDataPath;

            if (Directory.Exists(dir))
            {
                foreach (string file in Directory.GetFiles(dir, "*.json"))
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException e)
                    {
                        Debug.LogWarning($"�� ������� ������� {file}: {e.Message}");
                    }
                }
            }

            ClearCache();
        }

        public static void ClearAllSaves(string folderName = "")
        {
            string dir = string.IsNullOrEmpty(folderName)
                ? Application.persistentDataPath
                : Path.Combine(Application.persistentDataPath, folderName);

            if (Directory.Exists(dir))
            {
                string[] files = Directory.GetFiles(dir, "*.json");
                foreach (string file in files)
                {
                    try
                    {
                        File.Delete(file);
                    }
                    catch (IOException e)
                    {
                        Debug.LogWarning($"�� ������� ������� {file}: {e.Message}");
                    }
                }
            }

            ClearCache();
        }
    }
}