#if UNITY_EDITOR
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEditor;

/// <summary>
/// Только в редакторе. Если игра запущена не с entry-сцены (index 0),
/// перенаправляет на неё, ждёт инициализации GlobalLoader и возвращается на целевую сцену.
/// Включить/выключить: меню Tools → EditorBootstrap → Enable/Disable
/// </summary>
public class EditorBootstrap : MonoBehaviour
{
    private const int    ENTRY_SCENE_INDEX = 0;
    private const string TARGET_SCENE_KEY  = "EditorBootstrap_TargetScene";
    private const string ENABLED_KEY       = "EditorBootstrap_Enabled";

    public static bool IsEnabled
    {
        get => EditorPrefs.GetBool(ENABLED_KEY, false);
        set => EditorPrefs.SetBool(ENABLED_KEY, value);
    }

    [MenuItem("Tools/EditorBootstrap/Enable")]
    private static void Enable()  { IsEnabled = true;  Debug.Log("[EditorBootstrap] Включён"); }

    [MenuItem("Tools/EditorBootstrap/Disable")]
    private static void Disable() { IsEnabled = false; Debug.Log("[EditorBootstrap] Отключён"); }

    [MenuItem("Tools/EditorBootstrap/Enable",  validate = true)]
    private static bool ValidateEnable()  => !IsEnabled;

    [MenuItem("Tools/EditorBootstrap/Disable", validate = true)]
    private static bool ValidateDisable() => IsEnabled;

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void Init()
    {
        if (!IsEnabled) return;
        if (SceneManager.GetActiveScene().buildIndex == ENTRY_SCENE_INDEX) return;
        if (GlobalLoader.Instance != null) return;

        string targetScene = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetString(TARGET_SCENE_KEY, targetScene);
        PlayerPrefs.Save();

        Debug.Log($"[EditorBootstrap] Редирект → entry. Цель: '{targetScene}'");

        var go = new GameObject("[EditorBootstrap]");
        Object.DontDestroyOnLoad(go);
        go.AddComponent<EditorBootstrap>();

        SceneManager.LoadScene(ENTRY_SCENE_INDEX);
    }

    private void Start()
    {
        string target = PlayerPrefs.GetString(TARGET_SCENE_KEY, "");
        if (string.IsNullOrEmpty(target)) { Destroy(gameObject); return; }

        PlayerPrefs.DeleteKey(TARGET_SCENE_KEY);
        PlayerPrefs.Save();

        StartCoroutine(WaitAndLoad(target));
    }

    private IEnumerator WaitAndLoad(string targetScene)
    {
        yield return new WaitUntil(() => GlobalLoader.Instance != null);
        yield return null;

        Debug.Log($"[EditorBootstrap] GlobalLoader готов → загружаем '{targetScene}'");

        GlobalLoader.Instance.fightSceneLoader._onSceneActivated.AddListener(OnSceneReady);
        GlobalLoader.Instance.LoadToScene(targetScene);
    }

    private void OnSceneReady()
    {
        GlobalLoader.Instance.fightSceneLoader._onSceneActivated.RemoveListener(OnSceneReady);
        GlobalLoader.Instance.Show();
        Destroy(gameObject);
    }
}
#endif
