using EchoRift;
using UnityEngine;

[ExecuteAlways, RequireComponent(typeof(Collider2D))]
public class SceneLoaderTrigger : MonoBehaviour, IColliderDebugDrawable2D
{
    [SerializeField] private Vector3 nextScenePosition;
    [SerializeField] private SceneLoader sceneLoader;

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            player.movement.canMove = false;
            await GlobalLoader.Instance.mainUI.screenFader.FadeInAsync();

            sceneLoader._onLoadingSceneLoad.AddListener(() => GlobalLoader.Instance.Hide());
            sceneLoader._onSceneActivated.AddListener(() => player.movement.SetPlayerPosition(nextScenePosition));
            sceneLoader.LoadAsync();
            player.movement.canMove = true;
        }
    }

    private void OnDrawGizmos()
    {
        (this as IColliderDebugDrawable2D).OnDrawColliderGizmos2D();
    }
}
