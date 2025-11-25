using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Events;

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

    //[SerializeField] private UnityEvent actionBeforeNextScene;

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
    }

    private void OnDisable()
    {
        if (isShow)
            sceneLoader._onSceneActivated.RemoveListener(ShowOnHandleSceneActivated);
        else if (isHide)
            sceneLoader._onSceneActivated.RemoveListener(HideOnHandleSceneActivated);
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
