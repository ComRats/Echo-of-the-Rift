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
        // Если ты уверен, что объект один, DontDestroyOnLoad достаточно
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

        // ИСПРАВЛЕНИЕ 1: Если для сцены (например, LoadingScene) музыка не прописана, 
        // мы просто выходим и оставляем играть то, что играло.
        if (string.IsNullOrEmpty(targetMusic))
        {
            return;
        }

        // ИСПРАВЛЕНИЕ 2: Если музыка та же самая
        if (targetMusic == _currentMusicName)
        {
            // Проверяем, играет ли она физически (через Wrapper источника)
            if (_am.TryGetSource(_currentMusicName, out var wrapper) == AudioError.OK)
            {
                if (wrapper.Source.isPlaying)
                {
                    // Если уже играет — просто плавно возвращаем громкость, если она была занижена
                    _am.LerpVolume(_currentMusicName, normalVolume, fadeDuration);
                    return; // ВАЖНО: не идем дальше, чтобы не вызвать Play()
                }
            }
        }

        // Если мы здесь, значит музыка ДЕЙСТВИТЕЛЬНО сменилась

        // 1. Затухание старой музыки
        if (!string.IsNullOrEmpty(_currentMusicName))
        {
            _am.LerpVolume(_currentMusicName, 0f, fadeDuration);
        }

        // 2. Запуск новой
        _currentMusicName = targetMusic;
        _am.Play(_currentMusicName);

        // Устанавливаем громкость в 0 перед началом затухания (Fade In)
        if (_am.TryGetSource(_currentMusicName, out var newWrapper) == AudioError.OK)
        {
            newWrapper.Source.volume = 0f;
        }

        _am.LerpVolume(_currentMusicName, normalVolume, fadeDuration);
    }

    private string GetMusicForScene(string sceneName)
    {
        // Ищем конфиг для конкретной сцены
        var config = sceneMusicSettings.FirstOrDefault(s => s.scene != null && s.scene.SceneName == sceneName);
        return config?.musicName;
    }
}