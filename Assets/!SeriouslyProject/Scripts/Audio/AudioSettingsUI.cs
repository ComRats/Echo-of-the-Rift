using UnityEngine;
using UnityEngine.UI;
using AudioManager.Locator;
using EchoRift.SaveLoadSystem;

public class AudioSettingsUI : MonoBehaviour
{
    [Header("Настройки")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Slider sfxSlider;

    private const int SFX_INDEX = 0;
    private const int MUSIC_INDEX = 1;
    private const string AudioSaveKey = "audio_settings";

    private void Start()
    {
        var am = ServiceLocator.GetService();
        if (am == null) return;

        var savedData = SaveLoadSystem.Load<AudioSaveData>(AudioSaveKey);

        sfxSlider.value = savedData.sfxVolume;
        am.SetTypeVolume(SFX_INDEX, sfxSlider.value);
        sfxSlider.onValueChanged.AddListener(val => am.SetTypeVolume(SFX_INDEX, val));

        musicSlider.value = savedData.musicVolume;
        am.SetTypeVolume(MUSIC_INDEX, musicSlider.value);
        musicSlider.onValueChanged.AddListener(val => am.SetTypeVolume(MUSIC_INDEX, val));
    }

    public void CloseSettingsMenu()
    {
        AudioSaveData data = new AudioSaveData();
        data.musicVolume = musicSlider.value;
        data.sfxVolume = sfxSlider.value;

        SaveLoadSystem.Save(AudioSaveKey, data);
    }

    [System.Serializable]
    public class AudioSaveData
    {
        public float musicVolume = 1f;
        public float sfxVolume = 1f;
    }
}