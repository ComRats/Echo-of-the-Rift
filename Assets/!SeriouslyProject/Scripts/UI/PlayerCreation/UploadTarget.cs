using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class UploadTarget : MonoBehaviour
{
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
        SceneManager.LoadSceneAsync(nextScene).completed += _ =>
        {
            transform.SetParent(null);

            SceneManager.MoveGameObjectToScene(gameObject, SceneManager.GetActiveScene());

            RestoreValues();

            TargetToPlayer();
        };
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
