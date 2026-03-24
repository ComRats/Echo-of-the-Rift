using UnityEngine;
using UnityEngine.UI;
using Zenject;
using EchoRift.SaveLoadSystem;
using static EchoRift.SaveLoadSystem.SaveFileNames;

public class GameSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider turnDelaySlider;
    [SerializeField] private Slider turnSpeedSlider;
    [SerializeField] private Slider loadingSpeedSlider;

    [Inject]
    private GameSettings _settings;

    private void Start()
    {
        RefreshAllSettings();
    }

    public void RefreshAllSettings()
    {
        var data = SaveLoadSystem.Load<GlobalSettingsData>(SETTINGS) ?? new GlobalSettingsData();

        _settings.enemyTurnDelay = data.enemyTurnDelay;
        _settings.enemyTurnSpeed = data.enemyTurnSpeed;
        _settings.loadingSceneSpeed = data.loadingSceneSpeed;

        Configure(turnDelaySlider, _settings.enemyTurnDelay, (val) => _settings.enemyTurnDelay = val);
        Configure(turnSpeedSlider, _settings.enemyTurnSpeed, (val) => _settings.enemyTurnSpeed = val);
        Configure(loadingSpeedSlider, _settings.loadingSceneSpeed, (val) => _settings.loadingSceneSpeed = val);
    }

    private void Configure(Slider slider, float value, System.Action<float> applyValue)
    {
        if (slider == null) return;

        slider.onValueChanged.RemoveAllListeners();
        slider.value = value;
        slider.onValueChanged.AddListener(val =>
        {
            applyValue?.Invoke(val);
            SaveCurrentState();
        });
    }

    public void SaveCurrentState()
    {
        var data = SaveLoadSystem.Load<GlobalSettingsData>(SETTINGS) ?? new GlobalSettingsData();

        data.enemyTurnDelay = _settings.enemyTurnDelay;
        data.enemyTurnSpeed = _settings.enemyTurnSpeed;
        data.loadingSceneSpeed = _settings.loadingSceneSpeed;

        SaveLoadSystem.Save(SETTINGS, data);
    }
}