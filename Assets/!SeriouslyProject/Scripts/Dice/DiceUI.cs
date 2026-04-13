using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;

public class DiceUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI playerLabel;
    [SerializeField] private TextMeshProUGUI npcLabel;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private TextMeshProUGUI coinsText;
    [SerializeField] private Button rollButton;
    [SerializeField] private CanvasGroup buttonGroup;

    public string PlayerName { get; set; } = "Игрок";
    public string NpcName { get; set; } = "NPC";

    public void UpdateCoins(int coins)
    {
        if (coinsText != null)
            coinsText.text = $"💰 {coins}";
    }

    public void UpdateScores(int player, int npc)
    {
        playerLabel.text = $"{PlayerName}: {player}";
        npcLabel.text = $"{NpcName}: {npc}";

        playerLabel.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
        npcLabel.transform.DOPunchScale(Vector3.one * 0.1f, 0.2f);
    }

    public void UpdateStatus(string message)
    {
        statusText.text = message;
        statusText.DOFade(0, 0f).OnComplete(() => statusText.DOFade(1, 0.3f));
    }

    public void SetButtonInteractable(bool state)
    {
        rollButton.interactable = state;
        buttonGroup.DOFade(state ? 1f : 0.5f, 0.3f);
    }
}