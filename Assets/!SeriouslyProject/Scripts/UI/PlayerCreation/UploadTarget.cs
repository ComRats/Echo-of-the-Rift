using UnityEngine;
using UnityEngine.SceneManagement;

public class UploadTarget : MonoBehaviour
{
    [SerializeField] private string sceneName = "PlayerCreating";
    [SerializeField] private string nextScene = "PlayerScene";

    private Player player;

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
        player = FindObjectOfType<Player>();
        if (player != null && SceneManager.GetActiveScene().name != sceneName)
        {
            Debug.Log("Всё получилось");
            transform.SetParent(player.transform);
        }
    }

    public void NextScene()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(nextScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        SceneManager.MoveGameObjectToScene(gameObject, scene);

        TargetToPlayer();

        if (player != null)
        {
            player.GetComponent<TestMovement>().canMove = true;
            gameObject.transform.position = new(0f, 0.59f, 0f);
            gameObject.transform.localScale = new(0.0035f, 0.0035f, 0f);
        }

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
