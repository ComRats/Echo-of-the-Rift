using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TimeWeatherQuest : MonoBehaviour
{
    [SerializeField] private float timeScale = 60f;
    [SerializeField] private float gameTime = 0f;

    [SerializeField] private TextMeshProUGUI time;
    [SerializeField] private TextMeshProUGUI day;
    [SerializeField] private TextMeshProUGUI quest;
    [SerializeField] private Image questImage;
    [SerializeField] private Image weatherImage;

    private int currentDay = 1;

    private void Update()
    {
        GameTime();
    }

    private void GameTime()
    {
        gameTime += Time.deltaTime * timeScale;

        // Часы и минуты
        int totalSeconds = Mathf.FloorToInt(gameTime);
        int hours = (totalSeconds / 3600) % 24;
        int minutes = (totalSeconds % 3600) / 60;

        // Проверка смены дня
        int newDay = (totalSeconds / 3600) / 24 + 1;
        if (newDay != currentDay)
        {
            currentDay = newDay;
            day.text = $"День {currentDay}";
        }

        time.text = $"{hours:00}:{minutes:00}";
    }
}
