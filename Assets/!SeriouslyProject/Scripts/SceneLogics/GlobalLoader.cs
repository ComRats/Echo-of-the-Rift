using UnityEngine.SceneManagement;
using System.Collections.Generic;
using AudioManager.Provider;
using FightSystem.Data;
using EchoRift;
using System;
using UnityEngine;
using Zenject;
using EchoRift.SaveLoadSystem;
using static EchoRift.SaveLoadSystem.SaveFileNames;
using PixelCrushers;

[DisallowMultipleComponent]
public class GlobalLoader : MonoBehaviour
{
    public SceneLoader fightSceneLoader;

    [SerializeField] private List<SerializableScene> notShowScene;
    [SerializeField] private AudioManagerSettings settings;

    public static GlobalLoader Instance => instance;
    private static GlobalLoader instance;

    [Inject, HideInInspector] public Player playerInstance;
    [Inject, HideInInspector] public MainUI mainUI;
    [Inject] private GameSettings gameSettings;

    private bool isStart;

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

        if (gameSettings != null)
            SceneLoader.GlobalLoadingSpeed = gameSettings.loadingSceneSpeed;

        playerInstance.SetListenerToEvents(OnConversationStart, OnConversationEnd);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnConversationStart(Transform actor)
    {
        mainUI.canOpenUI = false;
    }

    private void OnConversationEnd(Transform actor)
    {
        mainUI.canOpenUI = true;
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
        // Восстанавливаем dialogue переменные/квесты из сохранения при каждой загрузке сцены
        RestoreDialogueState();

        LoadPlayer();

        // Сбрасываем состояние инвентаря UI при загрузке новой сцены
        if (mainUI != null)
            mainUI.ResetUIState();

        // Обновляем UI команды после загрузки сцены
        if (mainUI != null && mainUI.teamManager != null)
        {
            mainUI.teamManager.UpdateTeamUI();
        }
    }

    private void RestoreDialogueState()
    {
        var data = SaveLoadSystem.Load<GlobalData>(GLOBAL_SAVE, GAME_DIRECTORY);
        if (!string.IsNullOrEmpty(data.dialogueData))
        {
            var savedGameData = SaveSystem.Deserialize<SavedGameData>(data.dialogueData);
            SaveSystem.ApplySavedGameData(savedGameData);
        }
    }


    public void SavePlayer()
    {
        if (playerInstance == null)
        {
            Debug.LogWarning("[GlobalLoader] Не удалось сохранить игрока - playerInstance == null");
            return;
        }

        var data = new PlayerData
        {
            Position = playerInstance.transform.position,
            Rotation = playerInstance.transform.rotation
        };

        string sceneName = SceneManager.GetActiveScene().name;
        SaveLoadSystem.Save(GetPlayerSceneSave(sceneName), data, GAME_DIRECTORY);
        SaveLoadSystem.Save(PLAYER_DATA, playerInstance.playerSaver, GAME_DIRECTORY);

        // Сохранение команды
        var team = playerInstance.GetComponent<Team>();
        var teamData = team.CreateSaveData();
        SaveLoadSystem.Save(TEAM_DATA, teamData, GAME_DIRECTORY);

        mainUI.inventoryManager.SaveInventory();
    }

    private void LoadPlayerData()
    {
        if (SaveLoadSystem.Exists(PLAYER_DATA, GAME_DIRECTORY))
        {
            playerInstance.playerSaver = SaveLoadSystem.Load<Player.PlayerSaver>(PLAYER_DATA, GAME_DIRECTORY);
        }
        else
        {
            var characterData = Resources.Load<CharacterData>("CharacterData/Human");
            playerInstance.playerSaver = new Player.PlayerSaver();
            playerInstance.playerSaver.LoadFrom(characterData);
            SaveLoadSystem.Save(PLAYER_DATA, playerInstance.playerSaver, GAME_DIRECTORY);
        }

        // Загрузка команды
        var team = playerInstance.GetComponent<Team>();
        if (team != null && SaveLoadSystem.Exists(TEAM_DATA, GAME_DIRECTORY))
        {
            var teamData = SaveLoadSystem.Load<TeamSaveData>(TEAM_DATA, GAME_DIRECTORY);
            team.LoadFromSaveData(teamData);

            // Обновляем UI команды после загрузки данных
            if (mainUI != null && mainUI.teamManager != null)
            {
                mainUI.teamManager.UpdateTeamUI();
            }
        }
    }

    private void LoadPlayer()
    {
        if (playerInstance == null)
        {
            return;
        }

        if (SceneTransitionData.NextPosition.HasValue)
        {
            playerInstance.transform.position = SceneTransitionData.NextPosition.Value;
            playerInstance.transform.rotation = Quaternion.identity;
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        string fileName = GetPlayerSceneSave(sceneName);

        // Приоритет 2: Сохраненная позиция для текущей сцены
        if (SaveLoadSystem.Exists(fileName, GAME_DIRECTORY))
        {
            var data = SaveLoadSystem.Load<PlayerData>(fileName, GAME_DIRECTORY);

            if (data != null && !isStart)
            {
                playerInstance.transform.SetPositionAndRotation(data.Position, data.Rotation);
                return;
            }
        }

        // Приоритет 3: Стартовая позиция
        ResetPlayerTransform();
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
            sceneIndex = SceneManager.GetActiveScene().buildIndex,
            dialogueData = SaveSystem.Serialize(SaveSystem.RecordSavedGameData()),
            isStart = false,
            gameTime = GameTimer.GameTime
        };

        if (data.sceneIndex != 0 && data.sceneIndex != 1)
            SaveLoadSystem.Save(GLOBAL_SAVE, data, GAME_DIRECTORY);

        // Сохраняем состояния всех PersistentObject на сцене (NPC, объекты и т.д.)
        PersistentObject.SaveAll();

        mainUI.inventoryManager.SaveInventory();
    }

    private void LoadGlobal()
    {
        var data = SaveLoadSystem.Load<GlobalData>(GLOBAL_SAVE, GAME_DIRECTORY);
        isStart = data.isStart;
        var savedGameData = SaveSystem.Deserialize<SavedGameData>(data.dialogueData);
        SaveSystem.ApplySavedGameData(savedGameData);

        GameTimer.SetTime(data.gameTime);

        // Принудительно возобновляем игру при загрузке, сбрасывая все состояния паузы
        GameTimer.ForceResumeGame();
    }

    public void LoadToScene(string sceneToLoad, Vector3 positionToLoad)
    {
        //overridePosition = positionToLoad;
        SceneManager.LoadScene(sceneToLoad);
    }

    public void LoadToScene()
    {
        var globalData = SaveLoadSystem.Load<GlobalData>(GLOBAL_SAVE, GAME_DIRECTORY);
        fightSceneLoader._onSceneActivated.AddListener(OnReturnFromBattle);
        fightSceneLoader.LoadAsync(globalData.SceneIndex);
    }

    private void OnReturnFromBattle()
    {
        fightSceneLoader._onSceneActivated.RemoveListener(OnReturnFromBattle);
        Show();
    }

    public void LoadToScene(string sceneToLoad)
    {
        //overridePosition = positionToLoad;
        fightSceneLoader.LoadAsync(sceneToLoad);
    }

    public void SaveInventory()
    {
        mainUI.inventoryManager.SaveInventory();
    }

    /// <summary>
    /// Вызывается после PointsManager.AddPointsToPlayer() — обновляет playerSaver
    /// и сбрасывает RuntimeData команды чтобы они пересоздались из обновлённого ScriptableObject.
    /// </summary>
    public void RefreshPlayerDataFromCharacterData()
    {
        var characterData = Resources.Load<FightSystem.Data.CharacterData>("CharacterData/Human");
        if (characterData == null) return;

        playerInstance.playerSaver.LoadFrom(characterData);
        SaveLoadSystem.Save(PLAYER_DATA, playerInstance.playerSaver, GAME_DIRECTORY);

        // Сбрасываем RuntimeData у всех членов команды чтобы они пересоздались
        var team = playerInstance.GetComponent<Team>();
        if (team != null)
        {
            foreach (var character in team.characters)
                character.ResetRuntimeData();
        }
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
        public int sceneIndex;
        public string SceneIndex
        {
            get => SceneUtility.GetScenePathByBuildIndex(sceneIndex);
            set
            {
                sceneIndex = SceneUtility.GetBuildIndexByScenePath(value);
            }
        }

        public bool HasGameProgress => sceneIndex > 1;
        public string dialogueData;
        public bool isStart;
        public float gameTime;
    }
}
