using EchoRift.SaveLoadSystem;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject LoadButton;
    [SerializeField] private GameAlert gameAlertPrefab;
    [SerializeField] private SceneLoader startSceneLoader;
    [SerializeField] private SceneLoader loadSceneLoader;

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
            GameMassage.GameAlert(gameAlertPrefab, "Начать новую игру?", "Да", Play, "Нет", GameMassage.CloseAlert, 1f);
        else
            Play();
    }

    private void Play()
    {
        SaveLoadSystem.ClearAllSaves(GlobalLoader.GAME_DIRECTORY);
        var data = new GlobalLoader.GlobalData
        {
            isStart = true
        };
        SaveLoadSystem.Save("globalSave", data, GlobalLoader.GAME_DIRECTORY);

        startSceneLoader.LoadAsync();
    }

    public void Load()
    {
        globalData = SaveLoadSystem.Load<GlobalLoader.GlobalData>("globalSave", GlobalLoader.GAME_DIRECTORY);
        loadSceneLoader.LoadAsync(globalData.SceneIndex);
    }

    public void Quit()
    {
        GameMassage.GameAlert(gameAlertPrefab, "Выйти из игры?", "Да", Application.Quit, "Нет", GameMassage.CloseAlert, 1f, Color.black);
    }


}
