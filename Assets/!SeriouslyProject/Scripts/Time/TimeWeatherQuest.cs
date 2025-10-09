using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeWeatherQuest : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI time;
    [SerializeField] private TextMeshProUGUI day;
    [SerializeField] private TextMeshProUGUI quest;
    [SerializeField] private Image questImage;
    [SerializeField] private Image weatherImage;

    private int currentDay = 1;

    private void Awake()
    {
        GameTimer.OnGamePaused += OnGamePaused;
        GameTimer.OnGameResumed += OnGameResumed;
        GameTimer.OnTimeScaleChanged += OnTimeScaleChanged;
    }

    private void OnDestroy()
    {
        GameTimer.OnGamePaused -= OnGamePaused;
        GameTimer.OnGameResumed -= OnGameResumed;
        GameTimer.OnTimeScaleChanged -= OnTimeScaleChanged;
    }

    private void Update()
    {
        GameTimer.Update();
        UpdateTimeAndDay();
    }

    private void UpdateTimeAndDay()
    {
        time.text = GameTimer.GetFormattedTime();

        int totalSeconds = Mathf.FloorToInt(GameTimer.GameTime);
        int newDay = (totalSeconds / 3600) / 24 + 1;

        if (newDay != currentDay)
        {
            currentDay = newDay;
            day.text = $"День {currentDay}";
            UpdateQuestAndWeather();
        }
    }

    private void UpdateQuestAndWeather()
    {
    }

    private void OnGamePaused()
    {
    }

    private void OnGameResumed()
    {
        UpdateTimeAndDay();
    }

    private void OnTimeScaleChanged(float newScale)
    {
    }
}