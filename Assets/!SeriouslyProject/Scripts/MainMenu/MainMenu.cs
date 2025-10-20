using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameMassage;
    [SerializeField] private GameObject LoadButton;
    [SerializeField] private GameAlert gameAlertPrefab;
    private GlobalLoader.GlobalData globalData;

    private void Start()
    {
        if (!SaveLoadSystem.Exists("globalSave"))
        {
            LoadButton.SetActive(false);
        }
    }

    public void Play()
    {
        GameMassage.GameAlert(gameAlertPrefab, "Начать новую игру?", "Да", TryPlay, "Нет", GameMassage.CloseAlert, 1f, Color.black);
    }

    private void TryPlay()
    {
        var data = new GlobalLoader.GlobalData
        {
            isStart = true
        };
        SaveLoadSystem.Save("globalSave", data);

        SceneManager.LoadScene(1);
    }

    public void Load()
    {
        globalData = SaveLoadSystem.Load<GlobalLoader.GlobalData>("globalSave");
        SceneManager.LoadScene(globalData.sceneIndex);
    }

    public void Quit()
    {
        GameMassage.GameAlert(gameAlertPrefab, "Выйти из игры?", "Да", Application.Quit, "Нет", GameMassage.CloseAlert, 1f, Color.black);
    }
}
