using UnityEngine;
using UnityEngine.SceneManagement;
using AudioManager.Locator;
using AudioManager.Core;
using System.Collections.Generic;
using System.Linq;
using System;
using PixelCrushers.DialogueSystem;

[Serializable]
public class SceneMusicConfig
{
    public SerializableScene scene;
    public List<string> musicNames = new List<string>();
}

public class MusicTransitionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<SceneMusicConfig> sceneMusicSettings = new List<SceneMusicConfig>();
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float normalVolume = 1.0f;
    [SerializeField] private float pausedVolume = 0.3f;

    private IAudioManager _am;
    private List<string> _currentMusicNames = new List<string>();

    private List<string> _ambientSounds = new List<string>();

    private bool _isPaused = false;
    private bool _isDialogueActive = false;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.conversationStarted += OnConversationStarted;
            DialogueManager.instance.conversationEnded += OnConversationEnded;
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (DialogueManager.instance != null)
        {
            DialogueManager.instance.conversationStarted -= OnConversationStarted;
            DialogueManager.instance.conversationEnded -= OnConversationEnded;
        }
    }

    private void Start()
    {
        _am = ServiceLocator.GetService();
        HandleMusicChange(SceneManager.GetActiveScene().name);
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        HandleMusicChange(scene.name);
    }

    private void HandleMusicChange(string sceneName)
    {
        if (_am == null) _am = ServiceLocator.GetService();
        if (_am == null) return;

        List<string> targetMusicList = GetMusicForScene(sceneName);

        if (targetMusicList == null || targetMusicList.Count == 0)
            return;

        if (AreMusicListsEqual(_currentMusicNames, targetMusicList))
        {
            foreach (var musicName in _currentMusicNames)
            {
                if (_am.TryGetSource(musicName, out var wrapper) == AudioError.OK)
                {
                    if (wrapper.Source.isPlaying)
                        _am.LerpVolume(musicName, normalVolume, fadeDuration);
                }
            }
            return;
        }

        foreach (var oldMusic in _currentMusicNames)
        {
            _am.LerpVolume(oldMusic, 0f, fadeDuration);
        }

        _currentMusicNames = new List<string>(targetMusicList);

        foreach (var newMusic in _currentMusicNames)
        {
            _am.Play(newMusic);

            if (_am.TryGetSource(newMusic, out var wrapper) == AudioError.OK)
                wrapper.Source.volume = 0f;

            _am.LerpVolume(newMusic, normalVolume, fadeDuration);
        }
    }

    private bool AreMusicListsEqual(List<string> list1, List<string> list2)
    {
        if (list1.Count != list2.Count) return false;
        return list1.OrderBy(x => x).SequenceEqual(list2.OrderBy(x => x));
    }

    private List<string> GetMusicForScene(string sceneName)
    {
        var config = sceneMusicSettings.FirstOrDefault(s => s.scene != null && s.scene.SceneName == sceneName);
        return config?.musicNames ?? new List<string>();
    }

    public void RegisterAmbient(string soundName)
    {
        if (!_ambientSounds.Contains(soundName))
            _ambientSounds.Add(soundName);
    }

    public void UnregisterAmbient(string soundName)
    {
        if (_ambientSounds.Contains(soundName))
            _ambientSounds.Remove(soundName);
    }

    private IEnumerable<string> GetAllSounds()
    {
        return _currentMusicNames.Concat(_ambientSounds);
    }

    public void DuckMusic(float duckDuration = 0.5f)
    {
        if (_am == null || _isPaused) return;

        _isPaused = true;

        foreach (var sound in GetAllSounds())
        {
            _am.LerpVolume(sound, pausedVolume, duckDuration);
        }
    }

    public void RestoreMusic(float restoreDuration = 0.5f)
    {
        if (_am == null || !_isPaused) return;

        _isPaused = false;

        foreach (var sound in GetAllSounds())
        {
            _am.LerpVolume(sound, normalVolume, restoreDuration);
        }
    }

    private void OnConversationStarted(Transform actor)
    {
        if (_am == null || _isDialogueActive) return;

        _isDialogueActive = true;

        foreach (var sound in GetAllSounds())
        {
            _am.LerpVolume(sound, pausedVolume, fadeDuration);
        }
    }

    private void OnConversationEnded(Transform actor)
    {
        if (_am == null || !_isDialogueActive) return;

        _isDialogueActive = false;

        foreach (var sound in GetAllSounds())
        {
            _am.LerpVolume(sound, normalVolume, fadeDuration);
        }
    }
}