using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject gameMassage;
    [SerializeField] private GameObject LoadButton;
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
        Application.Quit();
    }
}
