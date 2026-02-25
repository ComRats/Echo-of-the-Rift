using System;
using System.Collections.Generic;
using UnityEngine;
using EchoRift.SaveLoadSystem;
using static EchoRift.SaveLoadSystem.SaveFileNames;

public class DebugRecorder : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool saveOnQuit = true;

    private DebugLogData currentLogData = new DebugLogData();

    public static DebugRecorder Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // �������� ��������� ������ ����, ���� ����� ���������� ������ � ��� �� ����
            // ���� ����� ������ ��� ����� ���� - ����� ������������ ��� � ����� � Start
            if (SaveLoadSystem.Exists(DEBUG_LOGS, GAME_DIRECTORY))
            {
                currentLogData = SaveLoadSystem.Load<DebugLogData>(DEBUG_LOGS, GAME_DIRECTORY);
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �������� ��������� � ���.
    /// </summary>
    /// <param name="tag">��������� (��������, "ScreenFader", "Player")</param>
    /// <param name="message">��������� ��� ��������</param>
    public void Log(string tag, object message)
    {
        var entry = new DebugEntry
        {
            timestamp = DateTime.Now.ToString("HH:mm:ss.fff"),
            tag = tag,
            message = message != null ? message.ToString() : "null"
        };

        currentLogData.entries.Add(entry);

        // �����������: ������� � ������� Unity, ����� ������ � �������� �������
        // Debug.Log($"[{entry.timestamp}] [{tag}] {entry.message}");
    }

    /// <summary>
    /// ������������� ��������� ������� ���� � ���� ����� ���� �������.
    /// </summary>
    [ContextMenu("Save Logs Now")]
    public void SaveLogs()
    {
        SaveLoadSystem.Save(DEBUG_LOGS, currentLogData, GAME_DIRECTORY);
        Debug.Log($"<color=green>Debug logs saved to: {GAME_DIRECTORY}/{DEBUG_LOGS}.json</color>");
    }

    [ContextMenu("Clear Logs")]
    public void ClearLogs()
    {
        currentLogData.entries.Clear();
        SaveLoadSystem.Save(DEBUG_LOGS, currentLogData, GAME_DIRECTORY);
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