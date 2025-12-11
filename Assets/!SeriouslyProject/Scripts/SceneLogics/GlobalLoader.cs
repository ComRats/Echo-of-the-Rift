using EchoRift;
using FightSystem.Data;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;
using static UnityEngine.Rendering.DebugUI;

[DisallowMultipleComponent]
public class GlobalLoader : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private List<SerializableScene> notShowScene;

    public static GlobalLoader Instance => instance;
    private static GlobalLoader instance;

    [Inject] public Player playerInstance;
    [Inject] public MainUI mainUI;

    private Vector3? overridePosition = null;
    private bool isStart;

    private int selectedTongueIndex = 0;
    public int SelectedTongueIndex
    {
        get => selectedTongueIndex;
        set => selectedTongueIndex = value;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;

        LoadGlobal();
        LoadPlayerData();

    }

    public void Show()
    {
        playerInstance.Show();
        mainUI.Show();
    }

    public void Hide()
    {
        playerInstance.Hide();
        mainUI.Hide();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        LoadPlayer();
        CameraSettingsInitialize();
    }


    public void SavePlayer()
    {
        if (playerInstance == null) return;

        var data = new PlayerData
        {
            Position = playerInstance.transform.position,
            Rotation = playerInstance.transform.rotation
        };

        SaveLoadSystem.Save($"playerSave_{SceneManager.GetActiveScene().name}", data);
        SaveLoadSystem.Save("playerData", playerInstance.playerSaver);
    }

    private void LoadPlayerData()
    {
        if (SaveLoadSystem.Exists("playerData"))
        {
            //Debug.LogWarning(playerInstance);
            playerInstance.playerSaver = SaveLoadSystem.Load<Player.PlayerSaver>("playerData");
        }
        else
        {
            var characterData = Resources.Load<CharacterData>("CharacterData/Human");
            playerInstance.playerSaver = new Player.PlayerSaver();
            playerInstance.playerSaver.LoadFrom(characterData);
            SaveLoadSystem.Save("playerData", playerInstance.playerSaver);
        }
    }

    private void LoadPlayer()
    {
        if (playerInstance == null) return;

        if (overridePosition.HasValue)
        {
            playerInstance.transform.position = overridePosition.Value;
            overridePosition = null;
            return;
        }

        string fileName = $"playerSave_{SceneManager.GetActiveScene().name}";
        string filePath = SaveLoadSystem.GetPath(fileName);

        if (!SaveLoadSystem.Exists(fileName))
        {
            ResetPlayerTransform();
            return;
        }

        var data = SaveLoadSystem.Load<PlayerData>(fileName);

        if (data == null || isStart)
        {
            ResetPlayerTransform();
            return;
        }

        playerInstance.transform.SetPositionAndRotation(data.Position, data.Rotation);
        Debug.LogError(SaveLoadSystem.GetPath(fileName));
    }

    private void ResetPlayerTransform()
    {
        playerInstance.transform.position = playerInstance.startPosition;
        playerInstance.transform.rotation = Quaternion.identity;
    }

    public void SaveGlobal()
    {
        var data = new GlobalData
        {
            selectedTongueIndex = selectedTongueIndex,
            sceneIndex = SceneManager.GetActiveScene().buildIndex
        };
        SaveLoadSystem.Save("globalSave", data);
    }

    private void LoadGlobal()
    {
        var data = SaveLoadSystem.Load<GlobalData>("globalSave");
        selectedTongueIndex = data.selectedTongueIndex;
        isStart = data.isStart;
    }

    public void LoadToScene(string sceneToLoad, Vector3 positionToLoad)
    {
        overridePosition = positionToLoad;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadToScene(string sceneToLoad/*, Vector3 positionToLoad*/)
    {
        //overridePosition = positionToLoad;
        sceneLoader.LoadAsync(sceneToLoad);
    }

    //private void OnDestroy()
    //{
    //    SceneManager.sceneLoaded -= OnSceneLoaded;
    //    SavePlayer();
    //    SaveGlobal();
    //}

#if !UNITY_EDITOR
    private void OnApplicationQuit()
    {
        SavePlayer();
        SaveGlobal();
    }
#endif

    public void CameraSettingsInitialize()
    {
        if (playerInstance.cameraSettings != null)
            playerInstance.cameraSettings.Initialize();
    }

    [Serializable]
    private class PlayerData
    {
        public Vector3 Position;
        public Quaternion Rotation;
    }

    [Serializable]
    public class GlobalData
    {
        public int selectedTongueIndex;
        public int sceneIndex;
        public string SceneIndex
        {
            get => SceneUtility.GetScenePathByBuildIndex(sceneIndex);
            set
            {
                sceneIndex = SceneUtility.GetBuildIndexByScenePath(value);
            }
        }

        public bool isStart;
    }
}
