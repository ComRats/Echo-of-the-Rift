using UnityEngine;
using AudioManager.Locator;
using AudioManager.Core;

public class SmoothMusicController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private string musicName;
    [SerializeField] private float fadeDuration = 2.0f;
    [SerializeField] private float targetVolume = 1.0f;

    private IAudioManager am;

    private void Start()
    {
        am = ServiceLocator.GetService();
        FadeIn();
    }

    public void FadeIn()
    {
        am.Play(musicName);

        if (am.TryGetSource(musicName, out var wrapper) == AudioError.OK)
        {
            wrapper.Source.volume = 0f;
        }

        am.LerpVolume(musicName, targetVolume, fadeDuration);
    }

    public void FadeOut()
    {
        am.LerpVolume(musicName, 0f, fadeDuration);
    }
}