using Sirenix.OdinInspector;
using UnityEngine;

[RequireComponent(typeof(SceneLoader))]
public class SceneLoaderBridge : MonoBehaviour
{
    [Header("ShowPlayer&UI")]
    [HorizontalGroup("Toggle")]
    [ToggleLeft]
    [OnValueChanged("SetShow")]
    [SerializeField] private bool isShow = true;

    [Header("HidePlayer&UI")]
    [HorizontalGroup("Toggle")]
    [ToggleLeft]
    [OnValueChanged("SetHide")]
    [SerializeField] private bool isHide = false;

    [Header("CameraSettingsInitialize")]
    [SerializeField] private bool isCameraInit = false;

    private SceneLoader sceneLoader;

    private void Awake()
    {
        if (sceneLoader == null)
            sceneLoader = GetComponent<SceneLoader>();

        if (isShow)
            sceneLoader._onSceneActivated.AddListener(() => GlobalLoader.Instance.Show());
        else if (isHide)
            sceneLoader._onSceneActivated.AddListener(() => GlobalLoader.Instance.Hide());

        if (isCameraInit)
        {
            //sceneLoader._onSceneActivated.AddListener(GlobalLoader.Instance.CameraSettingsInitialize);
        }

        sceneLoader._onSceneActivated.AddListener(async () =>
        {
            await GlobalLoader.Instance.mainUI.screenFader.FadeOutAsync();
        });
    }

    private void OnDestroy()
    {
        if (isShow)
            sceneLoader._onSceneActivated.RemoveListener(() => GlobalLoader.Instance.Show());
        else if (isHide)
            sceneLoader._onSceneActivated.RemoveListener(() => GlobalLoader.Instance.Hide());

        if (isCameraInit)
            //sceneLoader._onLoadingSceneLoad.RemoveListener(GlobalLoader.Instance.CameraSettingsInitialize);

        sceneLoader._onSceneActivated.RemoveListener(async () =>
        {
            await GlobalLoader.Instance.mainUI.screenFader.FadeOutAsync();
        });
    }

    private void SetShow()
    {
        if (isShow)
            isHide = false;
    }

    private void SetHide()
    {
        if (isHide)
            isShow = false;
    }
}
