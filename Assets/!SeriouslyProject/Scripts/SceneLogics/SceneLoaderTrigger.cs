using EchoRift;
using UnityEngine;

public class SceneLoaderTrigger : MonoBehaviour
{
    [SerializeField] private Vector3 nextScenePosition;
    [SerializeField] private SceneLoader sceneLoader;
    private bool _isTriggered = false;

    private async void OnTriggerEnter2D(Collider2D collision)
    {
        if (_isTriggered) return;

        if (collision.TryGetComponent<Player>(out var player))
        {
            _isTriggered = true;
            player.movement.canMove = false;

            await GlobalLoader.Instance.mainUI.screenFader.FadeInAsync();

            SceneTransitionData.NextPosition = nextScenePosition;
            sceneLoader.LoadAsync();
        }
    }
}

public static class SceneTransitionData
{
    public static Vector3? NextPosition = null;
}