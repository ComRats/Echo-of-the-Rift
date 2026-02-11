using UnityEngine;
using UnityEngine.Rendering.Universal;
using System;
using Sirenix.OdinInspector;

public class DayNightCycle : MonoBehaviour
{
    [Title("Lighting Setup")]
    [InfoBox("Назначьте Global Light 2D из сцены. Если не назначен, будет найден автоматически.", InfoMessageType.Info)]
    [SerializeField] private Light2D globalLight;
    
    [Title("Time Periods", "Настройка периодов суток")]
    [InfoBox("Оставьте пустым для использования настроек по умолчанию", InfoMessageType.None)]
    [SerializeField] private TimeOfDay[] timeOfDaySettings;
    
    [Title("Debug")]
    [SerializeField] private bool showDebugInfo = false;
    
    [ShowInInspector, ReadOnly, ShowIf("@globalLight != null")]
    private Color CurrentLightColor => globalLight != null ? globalLight.color : Color.white;
    
    [ShowInInspector, ReadOnly, ShowIf("@globalLight != null"), ProgressBar(0, 2)]
    private float CurrentLightIntensity => globalLight != null ? globalLight.intensity : 0f;
    
    [ShowInInspector, ReadOnly]
    private string CurrentPeriodName => currentTimeSettings?.periodName ?? "Неизвестно";
    
    [ShowInInspector, ReadOnly, ProgressBar(0, 1, ColorGetter = "GetProgressBarColor")]
    private float TransitionProgress => transitionProgress;

    public static event Action<DayPeriod> OnDayPeriodChanged;
    
    private DayPeriod currentPeriod = DayPeriod.Day;
    private TimeOfDay currentTimeSettings;
    private TimeOfDay nextTimeSettings;
    private float transitionProgress = 0f;
    
    private void Start()
    {
        InitializeTimeSettings();
        
        if (globalLight == null)
        {
            Debug.LogError("[DayNightCycle] Global Light2D не назначен! Назначьте глобальный свет в инспекторе.");
        }
    }
    
    private void Update()
    {
        if (globalLight == null) return;
        
        UpdateLighting();
        
        if (showDebugInfo)
        {
            DrawDebugInfo();
        }
    }
    
    private void InitializeTimeSettings()
    {
        if (timeOfDaySettings == null || timeOfDaySettings.Length == 0)
        {
            timeOfDaySettings = CreateDefaultTimeSettings();
        }

        Array.Sort(timeOfDaySettings, (a, b) => a.startHour.CompareTo(b.startHour));
    }
    
    private void UpdateLighting()
    {
        float currentHour = GetCurrentHour();

        FindCurrentAndNextPeriod(currentHour, out currentTimeSettings, out nextTimeSettings);

        if (currentTimeSettings == null || nextTimeSettings == null)
        {
            Debug.LogWarning("[DayNightCycle] Настройки времени не инициализированы!");
            return;
        }

        transitionProgress = CalculateTransitionProgress(currentHour, currentTimeSettings, nextTimeSettings);

        globalLight.color = Color.Lerp(currentTimeSettings.lightColor, nextTimeSettings.lightColor, transitionProgress);
        globalLight.intensity = Mathf.Lerp(currentTimeSettings.lightIntensity, nextTimeSettings.lightIntensity, transitionProgress);

        DayPeriod newPeriod = GetCurrentDayPeriod(currentHour);
        if (newPeriod != currentPeriod)
        {
            currentPeriod = newPeriod;
            OnDayPeriodChanged?.Invoke(currentPeriod);
            Debug.Log($"[DayNightCycle] Период суток изменён на: {currentPeriod}");
        }
    }
    
    private float GetCurrentHour()
    {
        int totalMinutes = Mathf.FloorToInt(GameTimer.GameTime / 60f);
        int hours = (totalMinutes / 60) % 24;
        int minutes = totalMinutes % 60;
        return hours + (minutes / 60f);
    }
    
    private void FindCurrentAndNextPeriod(float currentHour, out TimeOfDay current, out TimeOfDay next)
    {
        current = timeOfDaySettings[0];
        next = timeOfDaySettings[0];
        
        for (int i = 0; i < timeOfDaySettings.Length; i++)
        {
            if (currentHour >= timeOfDaySettings[i].startHour)
            {
                current = timeOfDaySettings[i];
                next = timeOfDaySettings[(i + 1) % timeOfDaySettings.Length];
            }
        }
    }
    
    private float CalculateTransitionProgress(float currentHour, TimeOfDay current, TimeOfDay next)
    {
        float duration = next.startHour - current.startHour;

        if (duration < 0)
        {
            duration += 24f;
        }
        
        float elapsed = currentHour - current.startHour;
        if (elapsed < 0)
        {
            elapsed += 24f;
        }
        
        return Mathf.Clamp01(elapsed / duration);
    }
    
    private DayPeriod GetCurrentDayPeriod(float hour)
    {
        if (hour >= 6f && hour < 12f)
            return DayPeriod.Morning;
        else if (hour >= 12f && hour < 18f)
            return DayPeriod.Day;
        else if (hour >= 18f && hour < 21f)
            return DayPeriod.Evening;
        else
            return DayPeriod.Night;
    }
    
    private TimeOfDay[] CreateDefaultTimeSettings()
    {
        return new TimeOfDay[]
        {
            new TimeOfDay
            {
                periodName = "Ночь",
                startHour = 0f,
                lightColor = new Color(0.2f, 0.2f, 0.4f, 1f), // Тёмно-синий
                lightIntensity = 0.3f
            },
            new TimeOfDay
            {
                periodName = "Рассвет",
                startHour = 5f,
                lightColor = new Color(1f, 0.7f, 0.5f, 1f), // Оранжево-розовый
                lightIntensity = 0.6f
            },
            new TimeOfDay
            {
                periodName = "Утро",
                startHour = 7f,
                lightColor = new Color(1f, 0.95f, 0.8f, 1f), // Светло-жёлтый
                lightIntensity = 0.9f
            },
            new TimeOfDay
            {
                periodName = "День",
                startHour = 12f,
                lightColor = new Color(1f, 1f, 1f, 1f), // Яркий белый
                lightIntensity = 1f
            },
            new TimeOfDay
            {
                periodName = "Вечер",
                startHour = 18f,
                lightColor = new Color(1f, 0.6f, 0.3f, 1f), // Оранжевый
                lightIntensity = 0.7f
            },
            new TimeOfDay
            {
                periodName = "Сумерки",
                startHour = 20f,
                lightColor = new Color(0.4f, 0.3f, 0.6f, 1f), // Фиолетовый
                lightIntensity = 0.4f
            }
        };
    }
    
    private void DrawDebugInfo()
    {
        float currentHour = GetCurrentHour();
        int hours = Mathf.FloorToInt(currentHour);
        int minutes = Mathf.FloorToInt((currentHour - hours) * 60f);
        
        Debug.Log($"[DayNightCycle] Время: {hours:00}:{minutes:00} | Период: {currentPeriod} | " +
                  $"Переход: {transitionProgress:F2} | Цвет: {globalLight.color} | Яркость: {globalLight.intensity:F2}");
    }

    public DayPeriod GetCurrentPeriod()
    {
        return currentPeriod;
    }

    public string GetCurrentPeriodName()
    {
        return currentTimeSettings?.periodName ?? "Неизвестно";
    }
    
    /// <summary>
    /// Установить глобальный свет (для динамической настройки)
    /// </summary>
    public void SetGlobalLight(Light2D light)
    {
        globalLight = light;
        Debug.Log("[DayNightCycle] Global Light2D установлен");
    }
    
    private Color GetProgressBarColor()
    {
        return currentTimeSettings != null ? currentTimeSettings.lightColor : Color.white;
    }
    
    [Button("Создать настройки по умолчанию", ButtonSizes.Large)]
    [ShowIf("@timeOfDaySettings == null || timeOfDaySettings.Length == 0")]
    private void CreateDefaultSettingsButton()
    {
        timeOfDaySettings = CreateDefaultTimeSettings();
        Debug.Log("[DayNightCycle] Созданы настройки по умолчанию");
    }
}

[Serializable]
public class TimeOfDay
{
    [HorizontalGroup("Main"), LabelWidth(80)]
    [Tooltip("Название периода (для отладки)")]
    public string periodName;
    
    [HorizontalGroup("Main"), LabelWidth(80)]
    [Tooltip("Час начала периода (0-23)")]
    [Range(0f, 23f)]
    public float startHour;
    
    [HorizontalGroup("Light"), LabelWidth(80)]
    [Tooltip("Цвет освещения")]
    public Color lightColor = Color.white;
    
    [HorizontalGroup("Light"), LabelWidth(80)]
    [Tooltip("Интенсивность света (0-2)")]
    [Range(0f, 2f)]
    public float lightIntensity = 1f;
}

public enum DayPeriod
{
    Morning,    // Утро (6:00-12:00)
    Day,        // День (12:00-18:00)
    Evening,    // Вечер (18:00-21:00)
    Night       // Ночь (21:00-6:00)
}
