using EchoRift;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject pauseMenuBackGround;
    [SerializeField] private ButtonSettings[] buttons;

    [Inject] private GameSettings gameSettings;
    [Inject] private Player _playerInstance;
    [Inject] private MainUI _mainUIInstance;

    private MusicTransitionManager _musicManager;
    private Animator settingsAnimator;
    private AnimatorStateInfo stateInfo;

    public bool isActive = false;

    private void Awake()
    {
        GameTimer.OnGamePaused += OnGamePaused;
        GameTimer.OnGameResumed += OnGameResumed;

        settingsAnimator = settingsPanel.GetComponent<Animator>();
        _musicManager = FindObjectOfType<MusicTransitionManager>();

        ButtonInitialize();
    }

    private void OnDestroy()
    {
        GameTimer.OnGamePaused -= OnGamePaused;
        GameTimer.OnGameResumed -= OnGameResumed;
    }

    private void Update()
    {
        if (Input.GetKeyDown(gameSettings.openPauseMenuKey))
        {
            if (!_mainUIInstance.canOpenUI) return;

            if (_mainUIInstance.shopUI != null && _mainUIInstance.shopUI.IsShopMode)
            {
                _mainUIInstance.shopUI.CloseShop();
                return;
            }

            if (_mainUIInstance.isOpenUI)
            {
                _mainUIInstance.CloseInventory();
                return;
            }

            if (pauseMenu.activeSelf)
            {
                isActive = false;
                ClosePauseMenu();
            }
            else
            {
                isActive = true;
                OpenPauseMenu();
            }
        }
    }

    public void OpenPauseMenu()
    {
        if (!_mainUIInstance.canOpenUI) return;

        _mainUIInstance.ShowCursor();
        GameTimer.PauseGame();
        pauseMenu.SetActive(true);
        pauseMenuBackGround.SetActive(true);

        if (settingsAnimator.isActiveAndEnabled)
        {
            stateInfo = settingsAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("ShowSettings"))
                settingsAnimator.SetTrigger("HideSettings");
        }
    }

    public void ClosePauseMenu()
    {
        _mainUIInstance.HideCursor();

        if (settingsAnimator.isActiveAndEnabled)
        {
            stateInfo = settingsAnimator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("ShowSettings"))
            {
                settingsAnimator.SetTrigger("HideSettings");
                pauseMenuBackGround.SetActive(true);
                return;
            }
        }

        GameTimer.ResumeGame();
        pauseMenu.SetActive(false);
    }

    private void ButtonInitialize()
    {
        buttons[0]._button.onClick.AddListener(() =>
        {
            ClosePauseMenu();
        });

        buttons[2]._button.onClick.AddListener(() =>
        {
            GlobalLoader.Instance.SavePlayer();
            GlobalLoader.Instance.SaveGlobal();
            GlobalLoader.Instance.SaveInventory();
        });

        buttons[3]._button.onClick.AddListener(() =>
        {
            GlobalLoader.Instance.SavePlayer();
            GlobalLoader.Instance.SaveGlobal();
            GlobalLoader.Instance.SaveInventory();

            ResetValues();

            sceneLoader._onLoadingSceneLoad.AddListener(() => GlobalLoader.Instance.Hide());
            sceneLoader.LoadAsync();
        });
    }

    private void ResetValues()
    {
        PauseGame();
        pauseMenu.SetActive(false);
    }

    public void PauseGame()
    {
        GameTimer.PauseGame();
    }

    public void ResumeGame()
    {
        GameTimer.ResumeGame();
    }

    private void OnGamePaused()
    {
        if (_musicManager != null)
        {
            _musicManager.DuckMusic();
        }
    }

    private void OnGameResumed()
    {
        if (_musicManager != null)
        {
            _musicManager.RestoreMusic();
        }
    }

    [System.Serializable]
    public class ButtonSettings
    {
        public Button _button;
        public TextMeshProUGUI _buttonText;
    }
}