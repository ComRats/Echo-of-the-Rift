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

    [Header("Pause Blink")]
    [SerializeField] private float blinkSpeed = 2f;
    [SerializeField] private float minAlpha = 0.3f;

    private int currentDay = 1;
    private bool isPaused = false;
    private float blinkTimer = 0f;

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
        UpdateTimeAndDay();
        UpdateBlink();
    }

    private void UpdateBlink()
    {
        if (!isPaused || time == null) return;

        blinkTimer += Time.unscaledDeltaTime * blinkSpeed;
        float alpha = Mathf.Lerp(minAlpha, 1f, (Mathf.Sin(blinkTimer * Mathf.PI) + 1f) * 0.5f);
        var c = time.color;
        time.color = new Color(c.r, c.g, c.b, alpha);
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
        isPaused = true;
        blinkTimer = 0f;
    }

    private void OnGameResumed()
    {
        isPaused = false;

        if (time != null)
        {
            var c = time.color;
            time.color = new Color(c.r, c.g, c.b, 1f);
        }

        UpdateTimeAndDay();
    }

    private void OnTimeScaleChanged(float newScale)
    {
    }
}