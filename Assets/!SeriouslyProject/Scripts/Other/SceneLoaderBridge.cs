using UnityEngine;
using System.Threading.Tasks;

public class SceneLoaderBridge : MonoBehaviour
{
    [SerializeField] private bool isShow = true;
    private SceneLoader _sceneLoader;

    private void Awake()
    {
        _sceneLoader = GetComponent<SceneLoader>();
        _sceneLoader._onLoadingSceneLoad.AddListener(OnPreloadLogic);
        _sceneLoader._onSceneActivated.AddListener(OnActivatedLogic);
    }

    private void OnPreloadLogic()
    {
        var player = GlobalLoader.Instance.playerInstance;
        if (player != null && SceneTransitionData.NextPosition.HasValue)
        {
            player.movement.SetPlayerPosition(SceneTransitionData.NextPosition.Value);
            player.cameraSettings.Initialize();
        }
    }

    private async void OnActivatedLogic()
    {
        if (this == null) return;

        if (isShow) GlobalLoader.Instance.Show();

        await Task.Yield();

        GlobalLoader.Instance.playerInstance?.cameraSettings.Initialize();

        await GlobalLoader.Instance.mainUI.screenFader.FadeOutAsync();

        if (GlobalLoader.Instance.playerInstance != null)
            GlobalLoader.Instance.playerInstance.movement.canMove = true;
    }
}