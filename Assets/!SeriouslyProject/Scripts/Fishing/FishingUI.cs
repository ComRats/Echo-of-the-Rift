using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FishingUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fishingStatusText;
    [SerializeField] private TextMeshProUGUI fishingHelpText;

    public ClickBarUI clickBar;

    public void ShowWaitingForBite()
    {
        fishingStatusText.text = "Ожидание поклевки...";
        fishingHelpText.text = "Нажмите F чтобы вытянуть удочку"; 
    }

    public void ShowBite()
    {
        fishingStatusText.text = "Клюёт!";
        fishingHelpText.text = "Нажмите F чтобы подсечь!";
    }

    public void ShowCatchResult(string fishName)
    {
        fishingStatusText.text = $"Вы поймали {fishName}!";
        fishingHelpText.text = "";
    }

    public void ShowMinigameHint(string hintText)
    {
        fishingStatusText.text = "";
        fishingHelpText.text = hintText;
    }

    public void ShowMinigameHint(string hintText, float delay)
    {
        ShowMinigameHint(hintText);
        Invoke(nameof(HideText), delay);
    }

    public void ShowMissed()
    {
        fishingStatusText.text = "Упустил!";
        fishingHelpText.text = "";
    }

    public void HideText()
    {
        fishingStatusText.text = "";
        fishingHelpText.text = "";
    }
}