using EchoRift.SaveLoadSystem;
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
                GameMassage.GameAlert(gameAlertPrefab, "������ ����� ����? �������� ����� ������.", "��", Play, "���", GameMassage.CloseAlert, 1f);
                return;
            }
        }
        Play();
    }

    private void Play()
    {
        SaveLoadSystem.ClearAllSaves(GlobalLoader.GAME_DIRECTORY);
        
        // Очищаем ScriptableObject инвентаря при новой игре
        if (inventoryData != null)
        {
            inventoryData.Clear();
            Debug.Log("Инвентарь очищен для новой игры");
        }
        else
        {
            Debug.LogWarning("InventoryData не назначен в MainMenu!");
        }
        
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
        SceneTransitionData.NextSceneName = globalData.SceneIndex;
        loadSceneLoader.LoadAsync(globalData.SceneIndex);
    }

    public void Quit()
    {
        GameMassage.GameAlert(gameAlertPrefab, "����� �� ����?", "��", Application.Quit, "���", GameMassage.CloseAlert, 1f);
    }
}
