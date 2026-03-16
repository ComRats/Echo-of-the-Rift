using UnityEngine;

/// <summary>
/// Единственный источник истины для состояния курсора.
/// LateUpdate гарантирует что никакой другой скрипт не перебьёт состояние в этом кадре.
/// </summary>
public class CursorManager : MonoBehaviour
{
    public static CursorManager Instance { get; private set; }

    private bool _wantVisible = true;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        StartCoroutine(EndOfFrameEnforce());
    }

    private System.Collections.IEnumerator EndOfFrameEnforce()
    {
        var eof = new WaitForEndOfFrame();
        while (true)
        {
            yield return eof;
            EnforceState();
        }
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void AutoCreate()
    {
        if (Instance != null) return;
        var go = new GameObject("CursorManager");
        go.AddComponent<CursorManager>();
    }

    private void Update()
    {
        EnforceState();
    }

    private void LateUpdate()
    {
        EnforceState();
    }

    private void EnforceState()
    {
        if (Cursor.visible != _wantVisible)
            Cursor.visible = _wantVisible;

        var wantedLock = _wantVisible ? CursorLockMode.None : CursorLockMode.Confined;
        if (Cursor.lockState != wantedLock)
            Cursor.lockState = wantedLock;
    }

    public static void Show()
    {
        //Debug.LogWarning($"[CursorManager] Show called\n{System.Environment.StackTrace}");
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        if (Instance != null) Instance._wantVisible = true;
    }

    public static void Hide()
    {
        //Debug.LogWarning($"[CursorManager] Hide called, Instance={(Instance != null ? "ok" : "NULL")}");
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        if (Instance != null) Instance._wantVisible = false;
    }
}
