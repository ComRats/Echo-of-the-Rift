using static EchoRift.EchoRiftSaveLoadSystem.SaveFileNames;
using EchoRift.EchoRiftSaveLoadSystem;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using AudioManager.Provider;
using EchoRift.Dialogue;
using FightSystem.Data;
using UnityEngine;
using EchoRift;
using Zenject;
using System;

[DisallowMultipleComponent]
public class GlobalLoader : MonoBehaviour
{
    public TimeManager timeManager;
    public SceneLoader fightSceneLoader;

    [SerializeField] private AudioManagerSettings settings;
    [SerializeField] private List<SerializableScene> notShowScene;

    public static GlobalLoader Instance => instance;
    private static GlobalLoader instance;

    [Inject, HideInInspector] public Player playerInstance;
    [Inject, HideInInspector] public MainUI mainUI;
    [Inject] private GameSettings gameSettings;

    private bool isStart;
    private bool isIsolatedSceneActive;
    private bool shouldRestoreAfterIsolatedSceneLoad;
    private bool wasPlayerActiveBeforeIsolation;
    private bool wasMainUIActiveBeforeIsolation;
    private bool wasCursorVisibleBeforeIsolation;
    private string isolatedSceneName;

    private void LateUpdate()
    {
        if (!isIsolatedSceneActive || SceneManager.GetActiveScene().name != isolatedSceneName)
            return;

        HealIsolatedSceneState();
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

    public void EnterIsolatedScene()
    {
        if (isIsolatedSceneActive)
            return;

        wasPlayerActiveBeforeIsolation = playerInstance != null && playerInstance.gameObject.activeSelf;
        wasMainUIActiveBeforeIsolation = mainUI != null && mainUI.gameObject.activeSelf;
        wasCursorVisibleBeforeIsolation = mainUI != null && mainUI.isCursorVisible;

        if (playerInstance != null)
            playerInstance.gameObject.SetActive(false);

        if (mainUI != null)
            mainUI.gameObject.SetActive(false);

        isolatedSceneName = "Dice";
        isIsolatedSceneActive = true;
        HealIsolatedSceneState();
    }

    public void MarkIsolatedSceneLoaded(string sceneName)
    {
        isolatedSceneName = sceneName ?? string.Empty;

        if (isIsolatedSceneActive)
            HealIsolatedSceneState();
    }

    public void ExitIsolatedScene()
    {
        if (!isIsolatedSceneActive)
            return;

        if (playerInstance != null)
            playerInstance.gameObject.SetActive(wasPlayerActiveBeforeIsolation);

        if (mainUI != null)
            mainUI.gameObject.SetActive(wasMainUIActiveBeforeIsolation);

        if (mainUI != null)
        {
            if (wasCursorVisibleBeforeIsolation)
                mainUI.ShowCursor();
            else
                mainUI.HideCursor();
        }
        else
        {
            CursorManager.Hide();
        }

        RestoreGameplayAudioListener();

        isIsolatedSceneActive = false;
        isolatedSceneName = string.Empty;
    }

    public void PrepareReturnFromIsolatedScene()
    {
        shouldRestoreAfterIsolatedSceneLoad = true;
    }

    private void HealIsolatedSceneState()
    {
        if (playerInstance != null && playerInstance.gameObject.activeSelf)
            playerInstance.gameObject.SetActive(false);

        if (mainUI != null && mainUI.gameObject.activeSelf)
            mainUI.gameObject.SetActive(false);

        CursorManager.Show();
        RestoreIsolatedSceneAudioListener();
    }

    private void RestoreIsolatedSceneAudioListener()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>(true);
        if (listeners.Length == 0)
            return;

        AudioListener isolatedListener = Camera.main != null
            ? Camera.main.GetComponent<AudioListener>()
            : null;

        if (isolatedListener == null)
        {
            foreach (var listener in listeners)
                listener.enabled = false;

            listeners[0].enabled = true;
            return;
        }

        foreach (var listener in listeners)
            listener.enabled = listener == isolatedListener;
    }

    private void RestoreGameplayAudioListener()
    {
        AudioListener gameplayListener = playerInstance != null
            ? playerInstance.GetComponentInChildren<AudioListener>(true)
            : null;

        AudioListener[] listeners = FindObjectsOfType<AudioListener>(true);

        if (gameplayListener == null)
        {
            if (listeners.Length > 0)
                listeners[0].enabled = true;
            return;
        }

        foreach (var listener in listeners)
            listener.enabled = listener == gameplayListener;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RestoreDialogueState();

        if (scene.name != "Dice")
            PersistentObject.LoadAll();

        LoadPlayer();

        if (mainUI != null)
        {
            mainUI.ResetUIState();

            bool isMenuScene = notShowScene.Exists(s => (string)s == scene.name);
            mainUI.canOpenUI = !isMenuScene;
        }

        if (mainUI != null && mainUI.teamManager != null)
        {
            mainUI.teamManager.UpdateTeamUI();
        }

        if (shouldRestoreAfterIsolatedSceneLoad)
        {
            ExitIsolatedScene();
            playerInstance?.cameraSettings.Initialize();

            if (playerInstance != null)
                playerInstance.movement.canMove = true;

            shouldRestoreAfterIsolatedSceneLoad = false;
        }

        RestoreUnlockedAbilities();

        if (scene.name != "Dice" && !isIsolatedSceneActive && EchoRift.DiceSessionState.HasActiveSession)
            EchoRift.DiceSessionState.Clear();
    }

    private void RestoreDialogueState()
    {
        DialogueSaveManager.Load();
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

        var team = playerInstance.GetComponent<Team>();
        if (team != null && SaveLoadSystem.Exists(TEAM_DATA, GAME_DIRECTORY))
        {
            var teamData = SaveLoadSystem.Load<TeamSaveData>(TEAM_DATA, GAME_DIRECTORY);
            team.LoadFromSaveData(teamData);

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

        if (SaveLoadSystem.Exists(fileName, GAME_DIRECTORY))
        {
            var data = SaveLoadSystem.Load<PlayerData>(fileName, GAME_DIRECTORY);

            if (data != null && !isStart)
            {
                playerInstance.transform.SetPositionAndRotation(data.Position, data.Rotation);
                return;
            }
        }

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
            isStart = false,
            gameTime = GameTimer.GameTime
        };

        if (data.sceneIndex != 0 && data.sceneIndex != 1)
        {
            SaveLoadSystem.Save(GLOBAL_SAVE, data, GAME_DIRECTORY);
            DialogueSaveManager.Save();
        }

        PersistentObject.SaveAll();

        mainUI.inventoryManager.SaveInventory();
    }

    private void LoadGlobal()
    {
        var data = SaveLoadSystem.Load<GlobalData>(GLOBAL_SAVE, GAME_DIRECTORY);
        isStart = data.isStart;

        DialogueSaveManager.Load();

        GameTimer.SetTime(data.gameTime);

        GameTimer.ForceResumeGame();
    }

    public void LoadToScene(string sceneToLoad, Vector3 positionToLoad)
    {
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
        if (fightSceneLoader != null)
        {
            fightSceneLoader.LoadAsync(sceneToLoad);
            return;
        }

        SceneManager.LoadScene(sceneToLoad);
    }

    public void SaveInventory()
    {
        mainUI.inventoryManager.SaveInventory();
    }

    private void RestoreUnlockedAbilities() { }

    public void RefreshPlayerDataFromCharacterData()
    {
        var characterData = Resources.Load<FightSystem.Data.CharacterData>("CharacterData/Human");
        if (characterData == null) return;

        playerInstance.playerSaver.LoadFrom(characterData);
        SaveLoadSystem.Save(PLAYER_DATA, playerInstance.playerSaver, GAME_DIRECTORY);

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
        public bool isStart;
        public float gameTime;
    }
}
