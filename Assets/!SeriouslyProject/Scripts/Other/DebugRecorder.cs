using System;
using System.Collections.Generic;
using UnityEngine;
using EchoRift.SaveLoadSystem;

public class DebugRecorder : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string folderName = "DebugLogs";
    [SerializeField] private string fileName = "SessionLog";
    [SerializeField] private bool saveOnQuit = true;

    private DebugLogData currentLogData = new DebugLogData();

    public static DebugRecorder Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Пытаемся загрузить старые логи, если хотим продолжать запись в тот же файл
            // Если нужно каждый раз новый файл - можно генерировать имя с датой в Start
            if (SaveLoadSystem.Exists(fileName, folderName))
            {
                currentLogData = SaveLoadSystem.Load<DebugLogData>(fileName, folderName);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Записать сообщение в лог.
    /// </summary>
    /// <param name="tag">Категория (например, "ScreenFader", "Player")</param>
    /// <param name="message">Сообщение или значение</param>
    public void Log(string tag, object message)
    {
        var entry = new DebugEntry
        {
            timestamp = DateTime.Now.ToString("HH:mm:ss.fff"),
            tag = tag,
            message = message != null ? message.ToString() : "null"
        };

        currentLogData.entries.Add(entry);

        // Опционально: выводим в консоль Unity, чтобы видеть в реальном времени
        // Debug.Log($"[{entry.timestamp}] [{tag}] {entry.message}");
    }

    /// <summary>
    /// Принудительно сохранить текущие логи в файл через твою систему.
    /// </summary>
    [ContextMenu("Save Logs Now")]
    public void SaveLogs()
    {
        SaveLoadSystem.Save(fileName, currentLogData, folderName);
        Debug.Log($"<color=green>Debug logs saved to: {folderName}/{fileName}.json</color>");
    }

    [ContextMenu("Clear Logs")]
    public void ClearLogs()
    {
        currentLogData.entries.Clear();
        SaveLoadSystem.Save(fileName, currentLogData, folderName);
        Debug.Log("Logs cleared.");
    }

    [ContextMenu("Open Save Folder")]
    public void OpenSaveFolder()
    {
        string path = Application.persistentDataPath;
        Application.OpenURL("file://" + path);
    }

    private void OnApplicationQuit()
    {
        if (saveOnQuit)
        {
            Log("System", "Session Ended");
            SaveLogs();
        }
    }

    [Serializable]
    public class DebugLogData
    {
        public List<DebugEntry> entries = new List<DebugEntry>();
    }

    [Serializable]
    public class DebugEntry
    {
        public string timestamp;
        public string tag;
        public string message;
    }
}