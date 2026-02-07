using UnityEngine;
using UnityEngine.SceneManagement;
using AudioManager.Locator;
using AudioManager.Core;
using System.Collections.Generic;
using System.Linq;
using System;

[Serializable]
public class SceneMusicConfig
{
    public SerializableScene scene;
    public string musicName;
}

public class MusicTransitionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<SceneMusicConfig> sceneMusicSettings = new List<SceneMusicConfig>();
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float normalVolume = 1.0f;

    private IAudioManager _am;
    private string _currentMusicName;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
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

        string targetMusic = GetMusicForScene(sceneName);

        if (string.IsNullOrEmpty(targetMusic))
        {
            if (!string.IsNullOrEmpty(_currentMusicName))
            {
                _am.LerpVolume(_currentMusicName, 0f, fadeDuration);
                _currentMusicName = null;
            }
            return;
        }

        if (targetMusic == _currentMusicName)
        {
            _am.LerpVolume(_currentMusicName, normalVolume, fadeDuration);
        }
        else
        {
            if (!string.IsNullOrEmpty(_currentMusicName))
            {
                _am.LerpVolume(_currentMusicName, 0f, fadeDuration);
            }

            _currentMusicName = targetMusic;
            _am.Play(_currentMusicName);

            if (_am.TryGetSource(_currentMusicName, out var wrapper) == AudioError.OK)
            {
                wrapper.Source.volume = 0f;
            }

            _am.LerpVolume(_currentMusicName, normalVolume, fadeDuration);
        }
    }

    private string GetMusicForScene(string sceneName)
    {
        var config = sceneMusicSettings.FirstOrDefault(s => s.scene != null && s.scene.SceneName == sceneName);
        return config?.musicName;
    }
}