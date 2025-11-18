using EchoRift;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class UploadTarget : MonoBehaviour
{
    [SerializeField] private SceneLoader nextSceneLoader;
    [SerializeField] private PointsManager points;
    [SerializeField] private TextMeshProUGUI descriptionStats;
    [SerializeField] private string sceneName;
    [SerializeField] private string nextScene;

    [Inject] private Player playerInstance;
    [Inject] private MainUI mainUiInstance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        TargetToPlayer();
    }

    public void TargetToPlayer()
    {
        if (playerInstance != null && SceneManager.GetActiveScene().name != sceneName)
        {
            transform.SetParent(playerInstance.transform);
        }
    }

    public void NextScene()
    {
        if (points.usedPoints >= points.maxPoints)
        {
            points.AddPointsToPlayer();
            descriptionStats.text = "Загрузка...";

            transform.SetParent(null);

            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

            RestoreValues();

            TargetToPlayer();

            nextSceneLoader.LoadAsync();            
        }
        else
        {
            descriptionStats.text = $"Распределите оставшиеся очки: ({points.maxPoints - points.usedPoints})";
        }
    }

    private void RestoreValues()
    {
        playerInstance ??= Object.FindObjectOfType<Player>(true);
        mainUiInstance ??= Object.FindObjectOfType<MainUI>(true);

        if (playerInstance != null)
            playerInstance.movement.canMove = true;

        if (mainUiInstance != null)
        {
            mainUiInstance.canOpenUI = true;
            mainUiInstance.gameObject.SetActive(true);
        }

        GameTimer.ResumeGame();

        transform.position = new Vector3(0f, 0.59f, 0f);
        transform.localScale = Vector3.one * 0.0035f;
    }
}
