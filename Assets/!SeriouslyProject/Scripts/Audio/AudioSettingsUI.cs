using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using AudioManager.Locator;
using EchoRift.SaveLoadSystem;

public class AudioSettingsUI : MonoBehaviour
{
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const string AudioSaveKey = "audio_settings";
    private const int SFX_INDEX = 0;
    private const int MUSIC_INDEX = 1;

    private void Awake()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        RefreshAllSettings();
    }

    public void RefreshAllSettings()
    {
        var am = ServiceLocator.GetService();
        if (am == null) return;

        var savedData = SaveLoadSystem.Load<AudioSaveData>(AudioSaveKey) ?? new AudioSaveData();

        Configure(sfxSlider, savedData.sfxVolume, SFX_INDEX);
        Configure(musicSlider, savedData.musicVolume, MUSIC_INDEX);
    }

    private void Configure(Slider slider, float volume, int typeIndex)
    {
        slider.onValueChanged.RemoveAllListeners();
        slider.value = volume;

        var am = ServiceLocator.GetService();
        am?.SetTypeVolume(typeIndex, volume);

        slider.onValueChanged.AddListener(val =>
        {
            ServiceLocator.GetService()?.SetTypeVolume(typeIndex, val);
            SaveCurrentState();
        });
    }

    private void SaveCurrentState()
    {
        AudioSaveData data = new AudioSaveData
        {
            musicVolume = musicSlider.value,
            sfxVolume = sfxSlider.value
        };
        SaveLoadSystem.Save(AudioSaveKey, data);
    }

    [System.Serializable]
    public class AudioSaveData
    {
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
    }
}