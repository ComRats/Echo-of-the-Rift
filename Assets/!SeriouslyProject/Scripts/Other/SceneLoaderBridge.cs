using UnityEngine;
using System.Threading.Tasks;
using EchoRift;

public class SceneLoaderBridge : MonoBehaviour
{
    [SerializeField] private bool isShow = true;
    private SceneLoader _sceneLoader;

    private Player player => GlobalLoader.Instance.playerInstance;

    private void Awake()
    {
        _sceneLoader = GetComponent<SceneLoader>();
        _sceneLoader._onLoadingSceneLoad.AddListener(OnPreloadLogic);
        _sceneLoader._onSceneActivated.AddListener(OnActivatedLogic);
    }

    private void OnPreloadLogic()
    {
    }

    private async void OnActivatedLogic()
    {
        if (this == null) return;

        if (isShow) GlobalLoader.Instance.Show();

        await Task.Yield();

        if (SceneTransitionData.NextPosition.HasValue)
        {
            player.movement.SetPlayerPosition(SceneTransitionData.NextPosition.Value);
            SceneTransitionData.NextPosition = null;
        }

        GlobalLoader.Instance.playerInstance?.cameraSettings.Initialize();

        await GlobalLoader.Instance.mainUI.screenFader.FadeOutAsync();

        if (GlobalLoader.Instance.playerInstance != null)
            GlobalLoader.Instance.playerInstance.movement.canMove = true;
    }
}