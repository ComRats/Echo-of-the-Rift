using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeUI : MonoBehaviour
{
    [Header("Time Display")]
    [SerializeField] private TextMeshProUGUI timeText;
    [SerializeField] private TextMeshProUGUI dayText;
    [SerializeField] private TextMeshProUGUI periodText;
    
    [Header("Period Icon")]
    [SerializeField] private Image periodIcon;
    [SerializeField] private Sprite morningIcon;
    [SerializeField] private Sprite dayIcon;
    [SerializeField] private Sprite eveningIcon;
    [SerializeField] private Sprite nightIcon;
    
    [Header("Colors")]
    [SerializeField] private Color morningColor = new Color(1f, 0.9f, 0.6f);
    [SerializeField] private Color dayColor = new Color(1f, 1f, 1f);
    [SerializeField] private Color eveningColor = new Color(1f, 0.7f, 0.4f);
    [SerializeField] private Color nightColor = new Color(0.6f, 0.6f, 0.8f);
    
    private int currentDay = 1;
    private DayPeriod currentPeriod = DayPeriod.Day;
    
    private void Awake()
    {
        GameTimer.OnGamePaused += OnGamePaused;
        GameTimer.OnGameResumed += OnGameResumed;
        DayNightCycle.OnDayPeriodChanged += OnDayPeriodChanged;
    }
    
    private void OnDestroy()
    {
        GameTimer.OnGamePaused -= OnGamePaused;
        GameTimer.OnGameResumed -= OnGameResumed;
        DayNightCycle.OnDayPeriodChanged -= OnDayPeriodChanged;
    }
    
    private void Update()
    {
        UpdateTimeDisplay();
    }
    
    private void UpdateTimeDisplay()
    {
        if (timeText != null)
        {
            timeText.text = GameTimer.GetFormattedTime();
        }

        int totalSeconds = Mathf.FloorToInt(GameTimer.GameTime);
        int totalMinutes = totalSeconds / 60;
        int totalHours = totalMinutes / 60;
        int newDay = (totalHours / 24) + 1;
        
        if (newDay != currentDay)
        {
            currentDay = newDay;
            if (dayText != null)
            {
                dayText.text = $"День {currentDay}";
            }
        }
    }
    
    private void OnDayPeriodChanged(DayPeriod newPeriod)
    {
        currentPeriod = newPeriod;
        UpdatePeriodDisplay();
    }
    
    private void UpdatePeriodDisplay()
    {
        if (periodText != null)
        {
            periodText.text = GetPeriodName(currentPeriod);
            periodText.color = GetPeriodColor(currentPeriod);
        }

        if (periodIcon != null)
        {
            periodIcon.sprite = GetPeriodIcon(currentPeriod);
            periodIcon.color = GetPeriodColor(currentPeriod);
        }
    }
    
    private string GetPeriodName(DayPeriod period)
    {
        switch (period)
        {
            case DayPeriod.Morning:
                return "Утро";
            case DayPeriod.Day:
                return "День";
            case DayPeriod.Evening:
                return "Вечер";
            case DayPeriod.Night:
                return "Ночь";
            default:
                return "";
        }
    }
    
    private Sprite GetPeriodIcon(DayPeriod period)
    {
        switch (period)
        {
            case DayPeriod.Morning:
                return morningIcon;
            case DayPeriod.Day:
                return dayIcon;
            case DayPeriod.Evening:
                return eveningIcon;
            case DayPeriod.Night:
                return nightIcon;
            default:
                return null;
        }
    }
    
    private Color GetPeriodColor(DayPeriod period)
    {
        switch (period)
        {
            case DayPeriod.Morning:
                return morningColor;
            case DayPeriod.Day:
                return dayColor;
            case DayPeriod.Evening:
                return eveningColor;
            case DayPeriod.Night:
                return nightColor;
            default:
                return Color.white;
        }
    }
    
    private void OnGamePaused()
    {
    }
    
    private void OnGameResumed()
    {
        UpdateTimeDisplay();
    }
}
