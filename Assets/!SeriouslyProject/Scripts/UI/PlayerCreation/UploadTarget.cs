using UnityEngine;
using UnityEngine.SceneManagement;

public class UploadTarget : MonoBehaviour
{
    [SerializeField] private string sceneName = "PlayerCreating";
    [SerializeField] private string nextScene = "PlayerScene";

    private Player player;

    private void Awake()
    {
        // не уничтожать при смене сцены
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
        // убираем и подписываемся заново (чтобы не было дубликатов)
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;

        SceneManager.LoadScene(nextScene);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // теперь переносим объект в новую сцену
        SceneManager.MoveGameObjectToScene(gameObject, scene);

        // снова делаем дочерним Player
        TargetToPlayer();

        if (player != null)
        {
            player.GetComponent<TestMovement>().canMove = true;
            gameObject.transform.position = new(0f, 0.59f, 0f);
            gameObject.transform.localScale = new(0.0035f, 0.0035f, 0f);
        }

        // отписка от события
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}
