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
        if (!SaveLoadSystem.Exists("globalSave") || SaveLoadSystem.Load<GlobalLoader.GlobalData>("globalSave").sceneIndex <= 1)
        {
            LoadButton.SetActive(false);
        }

        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("Show");
    }

    public void TryPlay()
    {
        if (SaveLoadSystem.Exists("globalSave"))
            GameMassage.GameAlert(gameAlertPrefab, "Начать новую игру?", "Да", Play, "Нет", GameMassage.CloseAlert, 1f, Color.black);
        else
            Play();
    }

    private void Play()
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
    }

    public void Quit()
    {
        GameMassage.GameAlert(gameAlertPrefab, "Выйти из игры?", "Да", Application.Quit, "Нет", GameMassage.CloseAlert, 1f, Color.black);
    }

   
}
