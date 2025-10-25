using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameMassage;
    [SerializeField] private GameObject LoadButton;
    [SerializeField] private GameAlert gameAlertPrefab;
    [SerializeField] private SceneLoader startSceneLoader;
    [SerializeField] private SceneLoader loadSceneLoader;
    private GlobalLoader.GlobalData globalData;

    //[Inject] private GameSettings gameSettings;
    //[Inject] private Player _playerInstance;
    //[Inject] private MainUI _mainUIInstance;

    private void Start()
    {
        if (!SaveLoadSystem.Exists("globalSave"))
        {
            LoadButton.SetActive(false);
        }
    }

    public void Play()
    {
        if (SaveLoadSystem.Exists("globalSave"))
            GameMassage.GameAlert(gameAlertPrefab, "Начать новую игру?", "Да", TryPlay, "Нет", GameMassage.CloseAlert, 1f, Color.black);
        else
            TryPlay();
    }

    private void TryPlay()
    {
        SaveLoadSystem.ClearAllSaves();
        var data = new GlobalLoader.GlobalData
        {
            isStart = true
        };
        SaveLoadSystem.Save("globalSave", data);

        startSceneLoader.LoadAsync();
    }

    public void Load()
    {
        globalData = SaveLoadSystem.Load<GlobalLoader.GlobalData>("globalSave");
        loadSceneLoader.LoadAsync(globalData.SceneIndex);

        //if (_playerInstance != null)
        //{
        //    GameTimer.ResumeGame();
        //    _playerInstance.movement.canMove = true;
        //    _mainUIInstance.canOpenUI = true;
        //    _mainUIInstance.gameObject.SetActive(true);
        //}
    }

    public void Quit()
    {
        GameMassage.GameAlert(gameAlertPrefab, "Выйти из игры?", "Да", Application.Quit, "Нет", GameMassage.CloseAlert, 1f, Color.black);
    }
}
