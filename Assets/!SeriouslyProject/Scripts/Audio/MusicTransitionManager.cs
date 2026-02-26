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
    public List<string> musicNames = new List<string>();
}

public class MusicTransitionManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private List<SceneMusicConfig> sceneMusicSettings = new List<SceneMusicConfig>();
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float normalVolume = 1.0f;

    private IAudioManager _am;
    private List<string> _currentMusicNames = new List<string>();

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

        List<string> targetMusicList = GetMusicForScene(sceneName);

        // Сценарий 1: Если нет музыки (например, LoadingScene) просто не трогаем
        if (targetMusicList == null || targetMusicList.Count == 0)
        {
            return;
        }

        // Сценарий 2: Если список музыки тот же самый
        if (AreMusicListsEqual(_currentMusicNames, targetMusicList))
        {
            // Проверяем и восстанавливаем громкость для всех треков
            foreach (var musicName in _currentMusicNames)
            {
                if (_am.TryGetSource(musicName, out var wrapper) == AudioError.OK)
                {
                    if (wrapper.Source.isPlaying)
                    {
                        _am.LerpVolume(musicName, normalVolume, fadeDuration);
                    }
                }
            }
            return;
        }

        // Если мы здесь, значит музыка действительно меняется

        // 1. Затухание старой музыки
        foreach (var oldMusic in _currentMusicNames)
        {
            if (!string.IsNullOrEmpty(oldMusic))
            {
                _am.LerpVolume(oldMusic, 0f, fadeDuration);
            }
        }

        // 2. Запуск новой музыки
        _currentMusicNames = new List<string>(targetMusicList);
        
        foreach (var newMusic in _currentMusicNames)
        {
            if (string.IsNullOrEmpty(newMusic)) continue;
            
            _am.Play(newMusic);

            // Устанавливаем громкость в 0 перед плавным нарастанием (Fade In)
            if (_am.TryGetSource(newMusic, out var newWrapper) == AudioError.OK)
            {
                newWrapper.Source.volume = 0f;
            }

            _am.LerpVolume(newMusic, normalVolume, fadeDuration);
        }
    }

    private bool AreMusicListsEqual(List<string> list1, List<string> list2)
    {
        if (list1.Count != list2.Count) return false;
        
        var sorted1 = list1.OrderBy(x => x).ToList();
        var sorted2 = list2.OrderBy(x => x).ToList();
        
        for (int i = 0; i < sorted1.Count; i++)
        {
            if (sorted1[i] != sorted2[i]) return false;
        }
        
        return true;
    }

    private List<string> GetMusicForScene(string sceneName)
    {
        // Ищем музыку для конкретной сцены
        var config = sceneMusicSettings.FirstOrDefault(s => s.scene != null && s.scene.SceneName == sceneName);
        return config?.musicNames ?? new List<string>();
    }
}