using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Zenject;

public class SceneLoader : MonoBehaviour
{
    [SerializeField] SerializableScene _serializableScene;
    [SerializeField] float _loadingSpeed = 0.5f;
    [SerializeField] bool _preloadOnStart;
    [SerializeField] LoadSceneMode _loadSceneMode = LoadSceneMode.Single;
    [SerializeField] bool _useSceneManager = false;
    [SerializeField] bool _destroyOnLoad = true;

    [Header("Events")]
    public UnityEvent _onSceneLoaded;
    public UnityEvent _onSceneActivated;
    public UnityEvent _onSceneUnloaded;
    public UnityEvent _onLoadingSceneLoad;

    private const string LoadingScene = "LoadingScene";
    private AsyncOperation _asyncLoadOperation;
    private AsyncOperation _asyncUnloadOperation;

    private bool _isListeningForLoadCompletedEvent = false;
    private bool _isListeningForUnloadCompletedEvent = false;
    private WaitForEndOfFrame _waitForEndOfFrame = new WaitForEndOfFrame();
    private Slider loadingSlider;

    IEnumerator Start()
    {
        yield return _waitForEndOfFrame;
        if (_preloadOnStart)
        {
            PreLoadAsync();
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneUnloaded += OnSceneUnloaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += OnActiveStateChanged;
    }

    private void OnDisable()
    {
        SceneManager.sceneUnloaded -= OnSceneUnloaded;
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.activeSceneChanged -= OnActiveStateChanged;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == _serializableScene.SceneName)
            _onSceneLoaded?.Invoke();
    }

    private void OnSceneUnloaded(Scene scene)
    {
        if (scene.name == _serializableScene.SceneName)
            _onSceneUnloaded?.Invoke();
    }

    private void OnActiveStateChanged(Scene oldScene, Scene newScene)
    {
        // можно отследить смену активной сцены
    }

    public async void LoadAsync()
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        if (_serializableScene == null || !_serializableScene.IsValid())
        {
            Debug.LogError("Scene not set or invalid!");
            return;
        }

        string targetScene = _serializableScene.SceneName;

        if (_useSceneManager)
        {
            await SceneLoaderManager.Instance.LoadAsync(targetScene);
            return;
        }

        StartCoroutine(LoadThroughIntermediateScene(targetScene));
    }

    public async void LoadAsync(string _targetScene)
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        if (_targetScene == null)
        {
            Debug.LogError("Scene not set or invalid!");
            return;
        }

        string targetScene = _targetScene;

        if (_useSceneManager)
        {
            await SceneLoaderManager.Instance.LoadAsync(targetScene);
            return;
        }

        StartCoroutine(LoadThroughIntermediateScene(targetScene));
    }

    /// <summary>
    /// Загружает сначала сцену "Loading", затем целевую сцену.
    /// </summary>
    private IEnumerator LoadThroughIntermediateScene(string targetScene)
    {
        transform.parent = null;
        DontDestroyOnLoad(gameObject);

        AsyncOperation loadingOp = SceneManager.LoadSceneAsync(LoadingScene, LoadSceneMode.Single);
        while (!loadingOp.isDone)
            yield return null;

        _onLoadingSceneLoad?.Invoke();

        if (loadingSlider == null)
            loadingSlider = FindObjectOfType<Slider>();

        yield return new WaitForSeconds(0.5f);

        _asyncLoadOperation = SceneManager.LoadSceneAsync(targetScene, LoadSceneMode.Single);
        _asyncLoadOperation.allowSceneActivation = false;

        float fakeProgress = 0f;

        while (_asyncLoadOperation.progress < 0.9f)
        {
            fakeProgress = Mathf.MoveTowards(fakeProgress, _asyncLoadOperation.progress, _loadingSpeed * Time.unscaledDeltaTime);

            loadingSlider.value = fakeProgress;

            yield return null;
        }

        while (fakeProgress < 1f)
        {
            fakeProgress = Mathf.MoveTowards(fakeProgress, 1f, _loadingSpeed * Time.unscaledDeltaTime);

            if (loadingSlider != null)
                loadingSlider.value = fakeProgress;

            yield return null;
        }

        _asyncLoadOperation.allowSceneActivation = true;

        while (!_asyncLoadOperation.isDone)
            yield return null;

        _onSceneActivated?.Invoke();

        if (SceneManager.GetSceneByName(LoadingScene).isLoaded)
        {
            SceneManager.UnloadSceneAsync(LoadingScene);
        }

        _onSceneLoaded?.Invoke();
        _asyncLoadOperation = null;

        if (_destroyOnLoad)
            Destroy(gameObject);
    }

    public async void PreLoadAsync()
    {
        if (_asyncLoadOperation == null)
            await LoadSceneAsync(_serializableScene.SceneName, _loadSceneMode, false);
    }

    public async Task<AsyncOperation> LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool allowSceneActivation = true)
    {
        DisableOnAsyncLoadCompletedListener();

        _asyncLoadOperation = SceneManager.LoadSceneAsync(sceneName, loadSceneMode);
        _asyncLoadOperation.allowSceneActivation = allowSceneActivation;
        _asyncLoadOperation.completed += OnLoadCompleted;
        _isListeningForLoadCompletedEvent = true;

        return _asyncLoadOperation;
    }

    private void OnLoadCompleted(AsyncOperation asyncOperation)
    {
        _asyncLoadOperation = null;
        _onSceneLoaded?.Invoke();
    }

    private void DisableOnAsyncLoadCompletedListener()
    {
        if (_isListeningForLoadCompletedEvent)
        {
            if (_asyncLoadOperation != null)
            {
                _asyncLoadOperation.completed -= OnLoadCompleted;
                _asyncLoadOperation = null;
            }
            _isListeningForLoadCompletedEvent = false;
        }
    }

    private void OnDestroy()
    {
        DisableOnAsyncLoadCompletedListener();
        DisableOnAsyncUnloadCompletedListener();
    }

    private void DisableOnAsyncUnloadCompletedListener()
    {
        if (_isListeningForUnloadCompletedEvent)
        {
            if (_asyncUnloadOperation != null)
            {
                _asyncUnloadOperation.completed -= OnAsyncUnloadCompleted;
                _asyncUnloadOperation = null;
            }
            _isListeningForUnloadCompletedEvent = false;
        }
    }

    private void OnAsyncUnloadCompleted(AsyncOperation asyncOperation)
    {
        _asyncUnloadOperation = null;
        _onSceneUnloaded?.Invoke();
    }
}
