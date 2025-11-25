using EchoRift;
using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(Collider2D))]
public class SceneLoaderTrigger : MonoBehaviour, IColliderDebugDrawable2D
{
    [SerializeField] private Vector3 nextScenePosition;
    [SerializeField] private SceneLoader sceneLoader;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            sceneLoader._onLoadingSceneLoad.AddListener(() => GlobalLoader.Instance.Hide());
            sceneLoader._onSceneActivated.AddListener(() => player.movement.SetPlayerPosition(nextScenePosition));
            sceneLoader.LoadAsync();
        }
    }

    private void OnDrawGizmos()
    {
        (this as IColliderDebugDrawable2D).OnDrawColliderGizmos2D();
    }
}
