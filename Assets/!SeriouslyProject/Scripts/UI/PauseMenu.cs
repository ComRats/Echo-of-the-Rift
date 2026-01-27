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

    private Animator settingsAnimator;
    private AnimatorStateInfo stateInfo;

    private void Awake()
    {
        GameTimer.OnGamePaused += OnGamePaused;
        GameTimer.OnGameResumed += OnGameResumed;

        settingsAnimator = settingsPanel.GetComponent<Animator>();

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
            stateInfo = settingsAnimator.GetCurrentAnimatorStateInfo(0);

            if (stateInfo.IsName("ShowSettings") && _mainUIInstance.canOpenUI)
            {
                settingsAnimator.SetTrigger("HideSettings");
                pauseMenuBackGround.SetActive(true);
                return;
            }

            if (pauseMenu.activeSelf && _mainUIInstance.canOpenUI)
            {
                GameTimer.ResumeGame();
                pauseMenu.SetActive(false);
            }
            else if (!pauseMenu.activeSelf && _mainUIInstance.canOpenUI)
            {
                GameTimer.PauseGame();
                pauseMenu.SetActive(true);
                pauseMenuBackGround.SetActive(true);

                if (stateInfo.IsName("ShowSettings"))
                {
                    settingsAnimator.SetTrigger("HideSettings");
                }
            }
        }
    }

    private void ButtonInitialize()
    {
        buttons[0]._button.onClick.AddListener(() =>
        {
            GameTimer.ResumeGame();
            pauseMenu.SetActive(false);
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
    }

    private void OnGameResumed()
    {

    }

    [System.Serializable]
    public class ButtonSettings
    {
        public Button _button;
        public TextMeshProUGUI _buttonText;
    }
}
