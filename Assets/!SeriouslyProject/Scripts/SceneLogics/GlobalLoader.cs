using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

[DisallowMultipleComponent]
public class GlobalLoader : MonoBehaviour
{
    [SerializeField] private GameObject player;

    public static GlobalLoader Instance => instance;

    private static GlobalLoader instance;
    private Vector3? overridePosition = null;
    /// <summary>
    /// сохранение всего зависит от сцены тоесть к примеру сохранение язычка в одной сцене будет отличаться от сохранения в другой сцене это нужно изменить
    /// </summary>
    private string SavePath => Path.Combine(Application.persistentDataPath, $"playerSave_{SceneManager.GetActiveScene().name}.json");

    private int selectedTongueIndex = 0;
    public int SelectedTongueIndex
    {
        get => selectedTongueIndex;
        set => selectedTongueIndex = value;
    }

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;

        player = Instantiate(player);
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Save();
    }

    private void OnApplicationQuit()
    {
        Save();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        DontDestroyOnLoad(player);
        Load();
    }

    private void Save()
    {
        if (player == null) return;

        var data = new PlayerData
        {
            Position = player.transform.position,
            Rotation = player.transform.rotation,
            SelectedTongueIndex = selectedTongueIndex
        };

        File.WriteAllText(SavePath, JsonUtility.ToJson(data, true));
    }

    private void Load()
    {
        if (player == null) return;

        if (overridePosition != null)
        {
            player.transform.position = overridePosition.Value;
            overridePosition = null;
            return;
        }

        if (!File.Exists(SavePath)) return;

        var json = File.ReadAllText(SavePath);
        var data = JsonUtility.FromJson<PlayerData>(json);

        player.transform.SetPositionAndRotation(data.Position, data.Rotation);
        selectedTongueIndex = data.SelectedTongueIndex;
    }

    public void LoadToScene(string sceneToLoad, GameObject objectToLoad, Vector3 positionToLoad)
    {
        overridePosition = positionToLoad;
        SceneManager.LoadScene(sceneToLoad);
    }

    [System.Serializable]
    private class PlayerData
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public int SelectedTongueIndex;
    }
}
