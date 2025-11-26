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
    }

    private void OnEnable()
    {
        if (isShow)
            sceneLoader._onSceneActivated.AddListener(ShowOnHandleSceneActivated);
        else if (isHide)
            sceneLoader._onSceneActivated.AddListener(HideOnHandleSceneActivated);

        if (isCameraInit)
        {
            Debug.LogWarning("Trying to Camera init");
            sceneLoader._onLoadingSceneLoad.AddListener(GlobalLoader.Instance.CameraSettingsInitialize);
        }

        sceneLoader._onLoadingSceneLoad.AddListener(GlobalLoader.Instance.mainUI.screenFader.FadeOut);
        sceneLoader._onSceneLoaded.AddListener(GlobalLoader.Instance.mainUI.screenFader.FadeOut);
    }

    private void OnDisable()
    {
        if (isShow)
            sceneLoader._onSceneActivated.RemoveListener(ShowOnHandleSceneActivated);
        else if (isHide)
            sceneLoader._onSceneActivated.RemoveListener(HideOnHandleSceneActivated);

        if (isCameraInit)
            sceneLoader._onLoadingSceneLoad.RemoveListener(GlobalLoader.Instance.CameraSettingsInitialize);

        sceneLoader._onLoadingSceneLoad.RemoveListener(GlobalLoader.Instance.mainUI.screenFader.FadeOut);
        sceneLoader._onSceneLoaded.RemoveListener(GlobalLoader.Instance.mainUI.screenFader.FadeOut);
    }

    private void ShowOnHandleSceneActivated()
    {
        GlobalLoader.Instance.Show();
    }

    private void HideOnHandleSceneActivated()
    {
        GlobalLoader.Instance.Hide();
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
