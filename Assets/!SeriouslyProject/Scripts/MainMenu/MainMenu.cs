using EchoRift.EchoRiftSaveLoadSystem;
using static EchoRift.EchoRiftSaveLoadSystem.SaveFileNames;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using EchoRift.Dialogue;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject LoadButton;
    [SerializeField] private GameAlert gameAlertPrefab;
    [SerializeField] private SceneLoader startSceneLoader;
    [SerializeField] private SceneLoader loadSceneLoader;
    [SerializeField] private InventoryData inventoryData;

    private GlobalLoader.GlobalData globalData;

    private void Awake()
    {
        if (!SaveLoadSystem.Exists(GLOBAL_SAVE, GAME_DIRECTORY) || SaveLoadSystem.Load<GlobalLoader.GlobalData>(GLOBAL_SAVE, GAME_DIRECTORY).sceneIndex <= 1)
        {
            LoadButton.SetActive(false);
        }
    }

    private void Start()
    {
        Animator animator = GetComponent<Animator>();

        if (!SaveLoadSystem.Exists(GLOBAL_SAVE, GAME_DIRECTORY) || SaveLoadSystem.Load<GlobalLoader.GlobalData>(GLOBAL_SAVE, GAME_DIRECTORY).sceneIndex <= 1)
        {
            LoadButton.SetActive(false);
            animator.SetTrigger("ButtonsShowLoad");
        }
        else
        {
            animator.SetTrigger("Show");
        }

    }

    public void TryPlay()
    {
        if (SaveLoadSystem.Exists(GLOBAL_SAVE, GAME_DIRECTORY))
        {
            var data = SaveLoadSystem.Load<GlobalLoader.GlobalData>(GLOBAL_SAVE, GAME_DIRECTORY);

            if (data != null && data.HasGameProgress)
            {
                GameMassage.GameAlert(gameAlertPrefab, "Вы уверенный? Все ваши сохранения удалятся.", "Нет", GameMassage.CloseAlert, "Да", Play, 1f);
                return;
            }
        }
        Play();
    }

    private void Play()
    {
        SaveLoadSystem.ClearAllSaves(GAME_DIRECTORY);
        SceneObjectsData.ResetCache();
        DialogueSaveManager.Delete();

        if (GlobalLoader.Instance != null)
            GlobalLoader.Instance.mainUI.inventoryManager.ResetForNewGame();
        else
            inventoryData.Clear();

        var characterData = Resources.Load<FightSystem.Data.CharacterData>("CharacterData/Human");
        characterData?.ResetToDefaults();
        
        var data = new GlobalLoader.GlobalData
        {
            isStart = true
        };
        SaveLoadSystem.Save(GLOBAL_SAVE, data, GAME_DIRECTORY);

        DialogueManager.ResetDatabase(DatabaseResetOptions.RevertToDefault);
        SaveSystem.ResetGameState();
        
        GameTimer.SetTime(12f * 60f * 60f);

        startSceneLoader.LoadAsync();
    }

    public void Load()
    {
        globalData = SaveLoadSystem.Load<GlobalLoader.GlobalData>(GLOBAL_SAVE, GAME_DIRECTORY);
        SceneTransitionData.NextSceneName = globalData.SceneIndex;

        string filePath = SaveLoadSystem.GetPath(PLAYER_NAME, GAME_DIRECTORY);
        var playerName = SaveLoadSystem.Load<ChangeNameDialogueActor.PLayerNameData>(PLAYER_NAME, GAME_DIRECTORY);
        var playerActor = GlobalLoader.Instance.playerInstance.dialogActor;

        DialogueSaveManager.Load();

        loadSceneLoader._onSceneActivated.AddListener(() =>
        {
            GameTimer.ResumeGame();
            GlobalLoader.Instance.mainUI.HideCursor();
        });
        playerActor.SaveNameForDialogueActor(playerName.playerDialogueName, true);
        loadSceneLoader.LoadAsync(globalData.SceneIndex);
    }

    public void Quit()
    {
        GameMassage.GameAlert(gameAlertPrefab, "Выйти из игры?", "Нет", GameMassage.CloseAlert, "Да", Application.Quit, 1f);
    }

    public void Credits()
    {
        Application.OpenURL("https://comrats.github.io");
    }
}
