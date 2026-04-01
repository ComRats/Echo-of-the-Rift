using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using EchoRift.EchoRiftSaveLoadSystem;
using static EchoRift.EchoRiftSaveLoadSystem.SaveFileNames;
using System;

/// <summary>
/// Добавь этот компонент на любой NPC или объект сцены.
/// Он автоматически сохраняет и восстанавливает активность объекта.
/// Никаких переменных в Dialogue System создавать не нужно.
/// 
/// Использование:
///   1. Добавь компонент на объект
///   2. Если нужно скрыть объект из кода/диалога — вызови gameObject.SetActive(false)
///      и сразу после этого PersistentObject.SaveAll() или просто сохрани игру как обычно
/// </summary>
public class PersistentObject : MonoBehaviour
{
    // Все зарегистрированные объекты на текущей сцене
    private static readonly List<PersistentObject> all = new();

    [Tooltip("Уникальный ID объекта. Заполняется автоматически, не меняй вручную.")]
    [SerializeField] private string persistentId;

#if UNITY_EDITOR
    [UnityEditor.MenuItem("CONTEXT/PersistentObject/Regenerate ID")]
    private static void RegenerateId(UnityEditor.MenuCommand cmd)
    {
        var comp = (PersistentObject)cmd.context;
        comp.persistentId = comp.BuildId();
        UnityEditor.EditorUtility.SetDirty(comp);
        Debug.Log($"[PersistentObject] Новый ID для {comp.gameObject.name}: {comp.persistentId}");
    }
#endif

    private string BuildId()
    {
        string scene = gameObject.scene.name;
        Vector3 pos = transform.position;
        return $"{scene}_{gameObject.name}_{pos.x:F1}_{pos.y:F1}_{pos.z:F1}";
    }

    private void OnValidate()
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(persistentId) && !UnityEditor.PrefabUtility.IsPartOfPrefabAsset(this))
        {
            persistentId = BuildId();
            UnityEditor.EditorUtility.SetDirty(this);
        }
#endif
    }

    private void Awake()
    {
        if (string.IsNullOrEmpty(persistentId))
            persistentId = BuildId();

        all.Add(this);
    }

    private void OnDestroy()
    {
        all.Remove(this);
    }

    private void Start()
    {
        // Восстанавливаем состояние при старте
        Load();
    }

    private static string GetSaveFileName()
    {
        return $"sceneObjects_{SceneManager.GetActiveScene().name}";
    }

    /// <summary>
    /// Сохраняет состояние этого объекта немедленно.
    /// </summary>
    public void Save()
    {
        var data = SceneObjectsData.Load();
        data.Set(persistentId, gameObject.activeSelf);
        data.SaveToDisk();
    }

    /// <summary>
    /// Восстанавливает состояние объекта из сохранения.
    /// </summary>
    public void Load()
    {
        var data = SceneObjectsData.Load();
        if (data.TryGet(persistentId, out bool active))
            gameObject.SetActive(active);
    }

    /// <summary>
    /// Сохраняет все PersistentObject на сцене. Вызывается из GlobalLoader при сохранении.
    /// </summary>
    public static void SaveAll()
    {
        var data = SceneObjectsData.Load();
        foreach (var obj in all)
        {
            if (obj != null)
                data.Set(obj.persistentId, obj.gameObject.activeSelf);
        }
        data.SaveToDisk();
    }

    /// <summary>
    /// Восстанавливает состояние ВСЕХ PersistentObject на сцене, включая изначально выключенные.
    /// Вызывается из GlobalLoader после загрузки сцены.
    /// </summary>
    public static void LoadAll()
    {
        // FindObjectsOfType с includeInactive=true находит даже выключенные объекты
        var allObjects = UnityEngine.Object.FindObjectsOfType<PersistentObject>(includeInactive: true);
        foreach (var obj in allObjects)
        {
            if (obj != null)
                obj.Load();
        }
    }
}

/// <summary>
/// Хранилище состояний объектов для одной сцены.
/// </summary>
[Serializable]
public class SceneObjectsData
{
    public List<string> keys = new();
    public List<bool> values = new();

    private static SceneObjectsData cached;
    private static string cachedScene;

    public static SceneObjectsData Load()
    {
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;

        // Инвалидируем кэш при смене сцены
        if (cachedScene != scene)
        {
            cached = null;
            cachedScene = scene;
        }

        if (cached != null) return cached;

        string fileName = $"sceneObjects_{scene}";
        cached = SaveLoadSystem.Load<SceneObjectsData>(fileName, GAME_DIRECTORY);
        return cached;
    }

    public void Set(string key, bool value)
    {
        int idx = keys.IndexOf(key);
        if (idx >= 0)
            values[idx] = value;
        else
        {
            keys.Add(key);
            values.Add(value);
        }
    }

    public bool TryGet(string key, out bool value)
    {
        int idx = keys.IndexOf(key);
        if (idx >= 0)
        {
            value = values[idx];
            return true;
        }
        value = true;
        return false;
    }

    public void SaveToDisk()
    {
        string scene = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
        string fileName = $"sceneObjects_{scene}";
        SaveLoadSystem.Save(fileName, this, GAME_DIRECTORY);
        cached = this;
    }

    /// <summary>
    /// Сбрасывает кэш — вызывай при старте новой игры.
    /// </summary>
    public static void ResetCache()
    {
        cached = null;
        cachedScene = null;
    }
}
