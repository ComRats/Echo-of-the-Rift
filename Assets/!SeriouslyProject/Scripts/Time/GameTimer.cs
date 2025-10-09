using UnityEngine;
using System;

public static class GameTimer
{
    /// <summary>
    /// Событие, вызываемое при изменении скорости времени.
    /// </summary>
    public static event Action<float> OnTimeScaleChanged;

    /// <summary>
    /// Событие, вызываемое при постановке игры на паузу.
    /// </summary>
    public static event Action OnGamePaused;

    /// <summary>
    /// Событие, вызываемое при возобновлении игры после паузы.
    /// </summary>
    public static event Action OnGameResumed;

    /// <summary>
    /// Текущее игровое время в секундах.
    /// </summary>
    public static float GameTime => gameTime;

    /// <summary>
    /// Скорость течения времени. Значения больше 1 ускоряют время, меньше 1 замедляют, 0 останавливает.
    /// </summary>
    public static float TimeScale
    {
        get => timeScale;
        set
        {
            timeScale = Mathf.Max(0f, value); // Не допускаем отрицательной скорости времени
            OnTimeScaleChanged?.Invoke(timeScale);
        }
    }

    /// <summary>
    /// Указывает, находится ли игра на паузе.
    /// </summary>
    public static bool IsPaused => isPaused;

    private static float gameTime = 0f;
    private static float timeScale = 1f;
    private static bool isPaused = false;
    private static int lastUpdateFrame = -1;
    private static float previousTimeScale = 1f;

    /// <summary>
    /// Обновляет игровое время. Должно вызываться один раз за кадр из MonoBehaviour.
    /// </summary>
    public static void Update()
    {
        // Проверяем, что Update не вызывается повторно в том же кадре
        if (Time.frameCount == lastUpdateFrame)
        {
            return;
        }

        lastUpdateFrame = Time.frameCount;

        if (!isPaused)
        {
            gameTime += Time.deltaTime * timeScale;
        }
    }

    /// <summary>
    /// Ставит игру на паузу, останавливая обновление игрового времени.
    /// </summary>
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

    /// <summary>
    /// Возобновляет игру после паузы, позволяя времени обновляться.
    /// </summary>
    public static void ResumeGame()
    {
        if (isPaused)
        {
            isPaused = false;
            OnGameResumed?.Invoke();
            TimeScale = previousTimeScale;
        }
    }

    /// <summary>
    /// Сбрасывает игровое время до 0.
    /// </summary>
    public static void ResetTime()
    {
        gameTime = 0f;
    }

    /// <summary>
    /// Возвращает игровое время в формате mm:ss.
    /// </summary>
    /// <returns>Строка в формате "mm:ss".</returns>
    public static string GetFormattedTime()
    {
        int minutes = Mathf.FloorToInt(gameTime / 60f);
        int seconds = Mathf.FloorToInt(gameTime % 60f);
        return string.Format("{0:00}:{1:00}", minutes, seconds);
    }
}