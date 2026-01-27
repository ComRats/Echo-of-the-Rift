using UnityEngine.SceneManagement;
using System.Collections.Generic;
using AudioManager.Locator;
using AudioManager.Provider;
using FightSystem.Data;
using EchoRift;
using System;
using UnityEngine;
using Zenject;
using EchoRift.SaveLoadSystem;

[DisallowMultipleComponent]
public class GlobalLoader : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private List<SerializableScene> notShowScene;
    [SerializeField] private AudioManagerSettings settings;

    public static GlobalLoader Instance => instance;
    private static GlobalLoader instance;

    [Inject, HideInInspector] public Player playerInstance;
    [Inject, HideInInspector] public MainUI mainUI;

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
        
        mainUI.inventoryManager.SaveInventory();
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
        
        mainUI.inventoryManager.SaveInventory();
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

#if !UNITY_EDITOR
    private void OnApplicationQuit()
    {
        SavePlayer();
        SaveGlobal();
        
        if (inventoryManager != null)
        {
            inventoryManager.SaveInventory();
        }
    }
#endif

    public void CameraSettingsInitialize()
    {
        if (playerInstance.cameraSettings != null)
            playerInstance.cameraSettings.Initialize();
    }
    
    public void SaveInventory()
    {
       mainUI.inventoryManager.SaveInventory();     
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
