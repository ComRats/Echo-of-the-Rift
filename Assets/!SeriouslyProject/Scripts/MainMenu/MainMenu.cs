using EchoRift.SaveLoadSystem;
using PixelCrushers;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

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

        if (!SaveLoadSystem.Exists("globalSave", GlobalLoader.GAME_DIRECTORY) || SaveLoadSystem.Load<GlobalLoader.GlobalData>("globalSave", GlobalLoader.GAME_DIRECTORY).sceneIndex <= 1)
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
        if (SaveLoadSystem.Exists("globalSave", GlobalLoader.GAME_DIRECTORY))
        {
            var data = SaveLoadSystem.Load<GlobalLoader.GlobalData>("globalSave", GlobalLoader.GAME_DIRECTORY);

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
        SaveLoadSystem.ClearAllSaves(GlobalLoader.GAME_DIRECTORY);
        
        inventoryData.Clear();
        
        var data = new GlobalLoader.GlobalData
        {
            isStart = true
        };
        SaveLoadSystem.Save("globalSave", data, GlobalLoader.GAME_DIRECTORY);

        DialogueManager.ResetDatabase(DatabaseResetOptions.RevertToDefault);
        SaveSystem.ResetGameState();

        startSceneLoader.LoadAsync();
    }

    public void Load()
    {
        globalData = SaveLoadSystem.Load<GlobalLoader.GlobalData>("globalSave", GlobalLoader.GAME_DIRECTORY);
        SceneTransitionData.NextSceneName = globalData.SceneIndex;

        string fileName = $"PlayerName";
        string filePath = SaveLoadSystem.GetPath(fileName, GlobalLoader.GAME_DIRECTORY);
        var playerName = SaveLoadSystem.Load<ChangeNameDialogueActor.PLayerNameData>(fileName, GlobalLoader.GAME_DIRECTORY);
        var playerActor = GlobalLoader.Instance.playerInstance.dialogActor;

        var savedGameData = SaveSystem.Deserialize<SavedGameData>(globalData.dialogueData);
        SaveSystem.ApplySavedGameData(savedGameData);

        loadSceneLoader._onSceneActivated.AddListener(() => playerActor.SaveNameForDialogueActor(playerName.playerDialogueName, true));
        loadSceneLoader.LoadAsync(globalData.SceneIndex);
    }

    public void Quit()
    {
        GameMassage.GameAlert(gameAlertPrefab, "Выйти из игры?", "Да", Application.Quit, "Нет", GameMassage.CloseAlert, 1f);
    }
}
