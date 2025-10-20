using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(Collider2D))]
public class SceneLoaderTrigger : MonoBehaviour, IColliderDebugDrawable2D
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

    private void OnDrawGizmos()
    {
        (this as IColliderDebugDrawable2D).OnDrawColliderGizmos2D();
    }
}
