using UnityEngine;
using System;

public static class GameTimer
{
    public static event Action<float> OnTimeScaleChanged;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;

    /// <summary>
    /// Сколько игровых минут проходит за 1 реальную секунду.
    /// По умолчанию 1 = 1 мин/сек (1 реальный час = 1 игровой день).
    /// Уменьши до 0.1–0.5 для более медленного хода времени.
    /// </summary>
    public static float GameMinutesPerRealSecond = 1f;

    public static float GameTime => gameTime;

    public static float TimeScale
    {
        get => timeScale;
        set
        {
            timeScale = Mathf.Max(0f, value);
            OnTimeScaleChanged?.Invoke(timeScale);
        }
    }

    public static bool IsPaused => isPaused;

    private static float gameTime = 0f;
    private static float timeScale = 1f;
    private static bool isPaused = false;
    private static int lastUpdateFrame = -1;
    private static float previousTimeScale = 1f;

    public static void Update()
    {
        if (Time.frameCount == lastUpdateFrame)
        {
            return;
        }

        lastUpdateFrame = Time.frameCount;

        if (!isPaused)
        {
            gameTime += Time.deltaTime * timeScale * 60f;
        }
    }

    public static void PauseGame()
    {
        if (!isPaused)
        {
            isPaused = true;
            OnGamePaused?.Invoke();
            previousTimeScale = timeScale;
            TimeScale = 0f;
        }
    }

    public static void ResumeGame()
    {
        if (isPaused)
        {
            isPaused = false;
            OnGameResumed?.Invoke();
            TimeScale = previousTimeScale;
        }
    }

    public static void ForceResumeGame()
    {
        isPaused = false;
        previousTimeScale = 1f;
        TimeScale = 1f;
        OnGameResumed?.Invoke();
    }

    public static void ResetTime()
    {
        gameTime = 0f;
    }

    public static void SetTime(float time)
    {
        gameTime = Mathf.Max(0f, time);
    }

    public static string GetFormattedTime()
    {
        int totalMinutes = Mathf.FloorToInt(gameTime / 60f);
        int hours = (totalMinutes / 60) % 24;
        int minutes = totalMinutes % 60;
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }
}