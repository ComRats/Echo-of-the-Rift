using EchoRift.SaveLoadSystem;
using static EchoRift.SaveLoadSystem.SaveFileNames;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject LoadButton;
    [SerializeField] private GameAlert gameAlertPrefab;
    [SerializeField] private SceneLoader startSceneLoader;
    [SerializeField] private SceneLoader loadSceneLoader;
    [SerializeField] private InventoryData inventoryData;

    private GlobalLoader.GlobalData globalData;

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
                GameMassage.GameAlert(gameAlertPrefab, "Вы уверенный? Все ваши сохранения удалятся.", "Да", Play, "Нет", GameMassage.CloseAlert, 1f);
                return;
            }
        }
        Play();
    }

    private void Play()
    {
        SaveLoadSystem.ClearAllSaves(GAME_DIRECTORY);
        
        inventoryData.Clear();
        
        var data = new GlobalLoader.GlobalData
        {
            isStart = true
        };
        SaveLoadSystem.Save(GLOBAL_SAVE, data, GAME_DIRECTORY);

        DialogueManager.ResetDatabase(DatabaseResetOptions.RevertToDefault);
        SaveSystem.ResetGameState();
        
        // Установка времени на 12:00 (12 часов * 60 минут * 60 секунд = 43200 секунд)
        GameTimer.SetTime(12f * 60f * 60f);
        //loadSceneLoader._onSceneActivated.AddListener(() => 
        //{
        //    FindObjectOfType<NeedToEnable>().EnableComponent();
        //    Debug.LogWarning(FindObjectOfType<NeedToEnable>().name);
        //}
        //);

        startSceneLoader.LoadAsync();
    }

    public void Load()
    {
        globalData = SaveLoadSystem.Load<GlobalLoader.GlobalData>(GLOBAL_SAVE, GAME_DIRECTORY);
        SceneTransitionData.NextSceneName = globalData.SceneIndex;

        string filePath = SaveLoadSystem.GetPath(PLAYER_NAME, GAME_DIRECTORY);
        var playerName = SaveLoadSystem.Load<ChangeNameDialogueActor.PLayerNameData>(PLAYER_NAME, GAME_DIRECTORY);
        var playerActor = GlobalLoader.Instance.playerInstance.dialogActor;

        var savedGameData = SaveSystem.Deserialize<SavedGameData>(globalData.dialogueData);
        SaveSystem.ApplySavedGameData(savedGameData);

        loadSceneLoader._onSceneActivated.AddListener(() => GameTimer.ResumeGame());
        playerActor.SaveNameForDialogueActor(playerName.playerDialogueName, true);
        loadSceneLoader.LoadAsync(globalData.SceneIndex);
    }

    public void Quit()
    {
        GameMassage.GameAlert(gameAlertPrefab, "Выйти из игры?", "Да", Application.Quit, "Нет", GameMassage.CloseAlert, 1f);
    }
}
