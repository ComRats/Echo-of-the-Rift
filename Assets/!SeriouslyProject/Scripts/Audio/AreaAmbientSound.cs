using UnityEngine;
using AudioManager.Locator;
using AudioManager.Core;
using EchoRift;
using System.Collections;

[RequireComponent(typeof(Collider2D))]
public class AreaAmbientSound : MonoBehaviour
{
    [Header("Sound")]
    [SerializeField] private string soundName;
    [SerializeField] private float fadeDuration = 1f;
    [SerializeField] private float targetVolume = 1f;

    [Header("Settings")]
    [SerializeField] private bool playOnEnter = true;
    [SerializeField] private bool stopOnExit = true;
    [SerializeField] private bool loop = true;

    private IAudioManager _am;
    private MusicTransitionManager _musicManager;
    private bool _isPlaying = false;

    private void Start()
    {
        _am = ServiceLocator.GetService();
        _musicManager = FindObjectOfType<MusicTransitionManager>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!playOnEnter) return;
        if (!other.CompareTag("Player")) return;

        PlaySound();
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!stopOnExit) return;
        if (!other.CompareTag("Player")) return;

        StopSound();
    }

    private void PlaySound()
    {
        if (_am == null || _isPlaying) return;

        _isPlaying = true;

        _am.Play(soundName);
        _musicManager?.RegisterAmbient(soundName);

        if (_am.TryGetSource(soundName, out var wrapper) == AudioError.OK)
        {
            wrapper.Source.loop = loop;
            wrapper.Source.volume = 0f;
        }

        _am.LerpVolume(soundName, targetVolume, fadeDuration);
    }

    private void StopSound()
    {
        if (_am == null || !_isPlaying) return;

        _isPlaying = false;

        _am.LerpVolume(soundName, 0f, fadeDuration);
        StartCoroutine(StopAfterFade());

        _musicManager?.UnregisterAmbient(soundName);
    }

    private IEnumerator StopAfterFade()
    {
        yield return new WaitForSeconds(fadeDuration);
        _am.Stop(soundName);
    }
}