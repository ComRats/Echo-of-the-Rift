using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoaderTrigger : MonoBehaviour
{
    [SerializeField] string nextSceneToLoad;
    [SerializeField] Vector3 nextPositionToLoad;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var sceneLoader))
        {
            GlobalLoader.Instance.LoadToScene(nextSceneToLoad, nextPositionToLoad);
        }
    }
}
