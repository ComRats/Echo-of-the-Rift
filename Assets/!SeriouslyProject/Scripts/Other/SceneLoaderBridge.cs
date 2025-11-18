using UnityEngine;

[RequireComponent(typeof(SceneLoader))]
public class SceneLoaderBridge : MonoBehaviour
{
    private SceneLoader sceneLoader;
    private GlobalLoader globalLoader;

    private void Awake()
    {
        if (sceneLoader == null)
            sceneLoader = GetComponent<SceneLoader>();
    }

    private void OnEnable()
    {
        sceneLoader._onSceneActivated.AddListener(HandleSceneActivated);
    }

    private void OnDisable()
    {
        sceneLoader._onSceneActivated.RemoveListener(HandleSceneActivated);
    }

    private void HandleSceneActivated()
    {
        FindObjectOfType<GlobalLoader>().Show();
    }
}
