using UnityEngine;
using System;

public static class GameTimer
{
    /// <summary>
    /// �������, ���������� ��� ��������� �������� �������.
    /// </summary>
    public static event Action<float> OnTimeScaleChanged;

    /// <summary>
    /// �������, ���������� ��� ���������� ���� �� �����.
    /// </summary>
    public static event Action OnGamePaused;

    /// <summary>
    /// �������, ���������� ��� ������������� ���� ����� �����.
    /// </summary>
    public static event Action OnGameResumed;

    /// <summary>
    /// ������� ������� ����� � ��������.
    /// </summary>
    public static float GameTime => gameTime;

    /// <summary>
    /// �������� ������� �������. �������� ������ 1 �������� �����, ������ 1 ���������, 0 �������������.
    /// </summary>
    public static float TimeScale
    {
        get => timeScale;
        set
        {
            timeScale = Mathf.Max(0f, value); // �� ��������� ������������� �������� �������
            OnTimeScaleChanged?.Invoke(timeScale);
        }
    }

    /// <summary>
    /// ���������, ��������� �� ���� �� �����.
    /// </summary>
    public static bool IsPaused => isPaused;

    private static float gameTime = 0f;
    private static float timeScale = 1f;
    private static bool isPaused = false;
    private static int lastUpdateFrame = -1;
    private static float previousTimeScale = 1f;

    /// <summary>
    /// Обновляет игровое время. Должен вызываться один раз за кадр из MonoBehaviour.
    /// </summary>
    public static void Update()
    {
        // Проверяем, что Update не вызывается несколько в один и тот же кадр
        if (Time.frameCount == lastUpdateFrame)
        {
            return;
        }

        lastUpdateFrame = Time.frameCount;

        if (!isPaused)
        {
            // Множитель скорости времени: реальные секунды на 1 игровую минуту = 60 / множитель
            // 20 = 3 сек/мин, 12 = 5 сек/мин, 6 = 10 сек/мин, 1 = 1 мин/мин (реальное время)
            gameTime += Time.deltaTime * timeScale * 60f;
        }
    }

    /// <summary>
    /// ������ ���� �� �����, ������������ ���������� �������� �������.
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
    /// ������������ ���� ����� �����, �������� ������� �����������.
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
    /// Принудительно возобновляет игру, сбрасывая все состояния паузы.
    /// Используется при загрузке игры для гарантированного возобновления.
    /// </summary>
    public static void ForceResumeGame()
    {
        isPaused = false;
        previousTimeScale = 1f;
        TimeScale = 1f;
        OnGameResumed?.Invoke();
    }

    /// <summary>
    /// ���������� ������� ����� �� 0.
    /// </summary>
    public static void ResetTime()
    {
        gameTime = 0f;
    }

    /// <summary>
    /// Устанавливает игровое время на указанное значение.
    /// </summary>
    /// <param name="time">Новое значение времени в секундах.</param>
    public static void SetTime(float time)
    {
        gameTime = Mathf.Max(0f, time);
    }


    /// <summary>
    /// Возвращает текущее время в формате HH:MM (часы:минуты).
    /// </summary>
    /// <returns>Строка в формате "HH:MM".</returns>
    public static string GetFormattedTime()
    {
        int totalMinutes = Mathf.FloorToInt(gameTime / 60f);
        int hours = (totalMinutes / 60) % 24; // Часы в 24-часовом формате
        int minutes = totalMinutes % 60;
        return string.Format("{0:00}:{1:00}", hours, minutes);
    }
}