using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] private string sceneToLoadWhenExit;
    [SerializeField] private TextMeshProUGUI title;
    [SerializeField] private GameObject settingsPanel;
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private ButtonSettings[] buttons;

    [Inject] private GameSettings gameSettings;
    [Inject] private Player _playerInstance;
    [Inject] private MainUI _mainUIInstance;

    private void Awake()
    {
        GameTimer.OnGamePaused += OnGamePaused;
        GameTimer.OnGameResumed += OnGameResumed;

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
            if (settingsPanel.activeSelf && _mainUIInstance.canOpenUI)
            {
                settingsPanel.SetActive(false);
                pauseMenu.transform.GetChild(0).gameObject.SetActive(true);
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
                settingsPanel.SetActive(false);
                pauseMenu.transform.GetChild(0).gameObject.SetActive(true);
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
        });

        buttons[3]._button.onClick.AddListener(() =>
        {
            GlobalLoader.Instance.SavePlayer();
            GlobalLoader.Instance.SaveGlobal();

            ResetValues();

            GlobalLoader.Instance.LoadToScene(sceneToLoadWhenExit, new Vector3(255f, 255f, 0f));
        });
    }

    private void ResetValues()
    {
        GameTimer.PauseGame();
        pauseMenu.SetActive(false);
        _playerInstance.movement.canMove = false;
        _mainUIInstance.canOpenUI = false;
        _mainUIInstance.gameObject.SetActive(false);
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
