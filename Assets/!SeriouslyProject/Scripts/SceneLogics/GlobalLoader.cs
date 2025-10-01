using System;
using UnityEngine;
using UnityEngine.SceneManagement;

[DisallowMultipleComponent]
public class GlobalLoader : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    public static GlobalLoader Instance => instance;
    private static GlobalLoader instance;

    private Player playerInstance;
    private Vector3? overridePosition = null;

    // --- глобальные данные ---
    private int selectedTongueIndex = 0;
    public int SelectedTongueIndex
    {
        get => selectedTongueIndex;
        set
        {
            selectedTongueIndex = value;
            SaveGlobal(); // при изменении сразу сохраняем
        }
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        playerInstance = Instantiate(playerPrefab.GetComponent<Player>());
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        LoadGlobal(); // загружаем глобальные данные при старте

        Debug.Log(SaveLoadSystem.GetPath($"playerSave_{SceneManager.GetActiveScene().name}"));
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SavePlayer();
        SaveGlobal();
    }

    private void OnApplicationQuit()
    {
        SavePlayer();
        SaveGlobal();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DontDestroyOnLoad(playerInstance);
        LoadPlayer();

        playerInstance.GetComponentInChildren<CameraSettings>().Initialize();
        Debug.Log("OnSceneLoaded");
    }

    // -----------------------
    // Сохранение/загрузка игрока
    private void SavePlayer()
    {
        if (playerInstance == null) return;

        var data = new PlayerData
        {
            Position = playerInstance.transform.position,
            Rotation = playerInstance.transform.rotation
        };

        string fileName = $"playerSave_{SceneManager.GetActiveScene().name}";
        SaveLoadSystem.Save(fileName, data);
    }

    private void LoadPlayer()
    {
        if (playerInstance == null) return;


        if (overridePosition != null)
        {
            playerInstance.transform.position = overridePosition.Value;
            overridePosition = null;
            return;
        }

        string fileName = $"playerSave_{SceneManager.GetActiveScene().name}";
        var data = SaveLoadSystem.Load<PlayerData>(fileName);
        if (data == null)
            return;
        else 
            playerInstance.transform.position = playerInstance.startPosition;

        if (fileName != null)
            playerInstance.transform.SetPositionAndRotation(data.Position, data.Rotation);

        //playerInstance.GetComponentInChildren<CameraSettings>().Initialize();
    }

    // -----------------------
    // Сохранение/загрузка глобальных данных
    private void SaveGlobal()
    {
        var data = SaveLoadSystem.Load<GlobalData>("globalSave");
        data.SelectedTongueIndex = selectedTongueIndex;
        SaveLoadSystem.Save("globalSave", data);
    }

    private void LoadGlobal()
    {
        var data = SaveLoadSystem.Load<GlobalData>("globalSave");
        selectedTongueIndex = data.SelectedTongueIndex;
    }

    // Переход между сценами
    public void LoadToScene(string sceneToLoad, Vector3 positionToLoad)
    {
        overridePosition = positionToLoad;
        SceneManager.LoadScene(sceneToLoad);
    }

    // Классы данных
    [Serializable]
    private class PlayerData
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    [Serializable]
    private class GlobalData
    {
        public int SelectedTongueIndex;
    }
}
