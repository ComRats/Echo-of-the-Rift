using UnityEngine;
using System;
using Sirenix.OdinInspector;

public class TimeManager : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField] private float normalTimeScale = 1f;
    [SerializeField] private float fastTimeScale = 5f;
    
    [Header("References")]
    [SerializeField] private DayNightCycle dayNightCycle;
    
    [Title("Debug Info", "Текущее состояние времени")]
    [ShowInInspector, ReadOnly, PropertyOrder(-1)]
    private string CurrentTime => $"{GetCurrentHour():00}:{GetCurrentMinute():00}";
    
    [ShowInInspector, ReadOnly, PropertyOrder(-1)]
    private string CurrentDay => $"День {GetCurrentDay()}";
    
    [ShowInInspector, ReadOnly, PropertyOrder(-1)]
    private string CurrentPeriod => GetCurrentPeriod().ToString();
    
    [Title("Time Control", "Установить конкретное время")]
    [Button("Установить время", ButtonSizes.Medium), PropertyOrder(0)]
    [HorizontalGroup("SetTime")]
    private void SetTimeButton()
    {
        SetTime(setHour, setMinute);
    }
    
    [HorizontalGroup("SetTime"), LabelWidth(50)]
    [Range(0, 23)]
    [SerializeField] private int setHour = 12;
    
    [HorizontalGroup("SetTime"), LabelWidth(50)]
    [Range(0, 59)]
    [SerializeField] private int setMinute = 0;
    
    [Title("Quick Actions", "Быстрая установка времени")]
    [Button("Утро (6:00)", ButtonSizes.Medium), HorizontalGroup("QuickTime1")]
    private void SetMorning() => SetTime(6, 0);
    
    [Button("День (12:00)", ButtonSizes.Medium), HorizontalGroup("QuickTime1")]
    private void SetNoon() => SetTime(12, 0);
    
    [Button("Вечер (18:00)", ButtonSizes.Medium), HorizontalGroup("QuickTime2")]
    private void SetEvening() => SetTime(18, 0);
    
    [Button("Ночь (0:00)", ButtonSizes.Medium), HorizontalGroup("QuickTime2")]
    private void SetMidnight() => SetTime(0, 0);
    
    [Title("Skip Time", "Пропустить время")]
    [Button("Пропустить", ButtonSizes.Medium), HorizontalGroup("SkipTime")]
    private void SkipTimeButton()
    {
        SkipTime(skipHours);
    }
    
    [HorizontalGroup("SkipTime"), LabelWidth(100), SuffixLabel("часов")]
    [Range(1, 24)]
    [SerializeField] private float skipHours = 1;
    
    [Button("Пропустить до утра (6:00)", ButtonSizes.Large)]
    private void SkipToMorningButton() => SkipToMorning();
    
    [Title("Time Scale", "Скорость времени")]
    [Button("Нормальная (1x)", ButtonSizes.Medium), HorizontalGroup("TimeScale")]
    private void SetNormalTimeButton() => SetNormalTime();
    
    [Button("Быстрая (5x)", ButtonSizes.Medium), HorizontalGroup("TimeScale")]
    private void SetFastTimeButton() => SetFastTime();
    
    [Button("Пауза", ButtonSizes.Medium), HorizontalGroup("TimeScale2")]
    private void PauseTimeButton() => GameTimer.PauseGame();
    
    [Button("Возобновить", ButtonSizes.Medium), HorizontalGroup("TimeScale2")]
    private void ResumeTimeButton() => GameTimer.ResumeGame();
    
    private void Start()
    {
        if (dayNightCycle == null)
        {
            dayNightCycle = FindObjectOfType<DayNightCycle>();
        }
    }

    /// <param name="hour">Час (0-23)</param>
    /// <param name="minute">Минута (0-59)</param>
    public void SetTime(int hour, int minute = 0)
    {
        hour = Mathf.Clamp(hour, 0, 23);
        minute = Mathf.Clamp(minute, 0, 59);
        
        int currentDay = GetCurrentDay();
        float newTime = ((currentDay - 1) * 24f * 3600f) + (hour * 3600f) + (minute * 60f);
        
        GameTimer.SetTime(newTime);
        Debug.Log($"[TimeManager] Время установлено на {hour:00}:{minute:00}");
    }

    /// <param name="hours">Количество часов для пропуска</param>
    public void SkipTime(float hours)
    {
        float secondsToSkip = hours * 3600f;
        GameTimer.SetTime(GameTimer.GameTime + secondsToSkip);
        Debug.Log($"[TimeManager] Пропущено {hours} часов");
    }

    public void SkipToMorning()
    {
        int currentHour = GetCurrentHour();
        int hoursToSkip;
        
        if (currentHour < 6)
        {
            hoursToSkip = 6 - currentHour;
        }
        else
        {
            hoursToSkip = 24 - currentHour + 6;
        }
        
        SkipTime(hoursToSkip);
        Debug.Log("[TimeManager] Пропущено до утра (6:00)");
    }
    public void SetTimeScale(float scale)
    {
        GameTimer.TimeScale = scale;
        Debug.Log($"[TimeManager] Скорость времени установлена на {scale}x");
    }

    public void SetFastTime()
    {
        SetTimeScale(fastTimeScale);
    }

    public void SetNormalTime()
    {
        SetTimeScale(normalTimeScale);
    }

    public int GetCurrentHour()
    {
        int totalMinutes = Mathf.FloorToInt(GameTimer.GameTime / 60f);
        return (totalMinutes / 60) % 24;
    }

    public int GetCurrentMinute()
    {
        int totalMinutes = Mathf.FloorToInt(GameTimer.GameTime / 60f);
        return totalMinutes % 60;
    }

    public int GetCurrentDay()
    {
        int totalSeconds = Mathf.FloorToInt(GameTimer.GameTime);
        int totalHours = totalSeconds / 3600;
        return (totalHours / 24) + 1;
    }

    public DayPeriod GetCurrentPeriod()
    {
        if (dayNightCycle != null)
        {
            return dayNightCycle.GetCurrentPeriod();
        }
        
        int hour = GetCurrentHour();
        if (hour >= 6 && hour < 12)
            return DayPeriod.Morning;
        else if (hour >= 12 && hour < 18)
            return DayPeriod.Day;
        else if (hour >= 18 && hour < 21)
            return DayPeriod.Evening;
        else
            return DayPeriod.Night;
    }

    public bool IsNight()
    {
        return GetCurrentPeriod() == DayPeriod.Night;
    }

    public bool IsDay()
    {
        DayPeriod period = GetCurrentPeriod();
        return period == DayPeriod.Morning || period == DayPeriod.Day;
    }

    public float GetDayProgress()
    {
        int hour = GetCurrentHour();
        int minute = GetCurrentMinute();
        return (hour * 60 + minute) / (24f * 60f);
    }
    
    /// <summary>
    /// Установить ссылку на DayNightCycle (для динамической настройки)
    /// </summary>
    public void SetDayNightCycle(DayNightCycle cycle)
    {
        dayNightCycle = cycle;
    }
}
