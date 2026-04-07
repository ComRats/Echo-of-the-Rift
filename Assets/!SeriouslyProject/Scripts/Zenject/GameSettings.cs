using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Installers/GameSettings")]
public class GameSettings : ScriptableObjectInstaller<GameSettings>
{
    [Header("UI Settings")]
    public KeyCode openInvenoryKey = KeyCode.E;
    public KeyCode openPauseMenuKey = KeyCode.Escape;
    public KeyCode useButton = KeyCode.F;
    public KeyCode questWindowKey = KeyCode.J;

    [Header("Key Sprites")]
    [Tooltip("Маппинг KeyCode → индекс спрайта в SpriteCollection")]
    public List<KeySpriteEntry> keySpriteMap = new List<KeySpriteEntry>();

    [Serializable]
    public class KeySpriteEntry
    {
        public KeyCode key;
        public int spriteIndex;
    }

    // Допустимые клавиши для кнопки взаимодействия (useButton)
    public static readonly KeyCode[] AllowedUseKeys =
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z, KeyCode.Escape, KeyCode.Tab,
    };

    // Все текущие занятые клавиши (для проверки дубликатов)
    public KeyCode[] AllBoundKeys => new[]
    {
        openInvenoryKey, openPauseMenuKey, useButton, questWindowKey
    };

    /// <summary>
    /// Возвращает индекс спрайта для указанной клавиши.
    /// Если маппинг не найден — возвращает fallback (по умолчанию 14).
    /// </summary>
    public int GetSpriteIndex(KeyCode key, int fallback = 14)
    {
        foreach (var entry in keySpriteMap)
            if (entry.key == key) return entry.spriteIndex;
        return fallback;
    }

    [Header("Combat Settings")]
    [Tooltip("Задержка перед ходом противника (в секундах)")]
    [Range(0.5f, 5f)]
    public float enemyTurnDelay = 1.5f;
    
    [Tooltip("Скорость анимации хода противника")]
    [Range(0.5f, 3f)]
    public float enemyTurnSpeed = 1f;

    public float loadingSceneSpeed = 10f;

    public override void InstallBindings()
    {
        Container.Bind<GameSettings>().FromInstance(this).AsSingle();
        //Debug.Log("Succeful Binding GameSettings");
    }
}