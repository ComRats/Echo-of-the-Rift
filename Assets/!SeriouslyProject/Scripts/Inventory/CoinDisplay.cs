using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class CoinDisplay : MonoBehaviour
{
    [SerializeField] private PlayerWallet playerWallet;
    [SerializeField] private string prefix = "Монеты: ";

    [SerializeField] private TextMeshProUGUI coinText;

    private void Awake()
    {
        if (playerWallet == null)
        {
            Debug.LogWarning("[CoinDisplay] PlayerWallet не назначен! Назначь его в инспекторе.");
        }
    }

    private void OnEnable()
    {
        if (playerWallet != null)
        {
            playerWallet.OnCoinsChanged += UpdateDisplay;
            UpdateDisplay(playerWallet.Coins);
        }
    }

    private void OnDisable()
    {
        if (playerWallet != null)
        {
            playerWallet.OnCoinsChanged -= UpdateDisplay;
        }
    }

    private void UpdateDisplay(int coins)
    {
        if (coinText != null)
        {
            coinText.text = $"{prefix}{coins}";
        }
    }
}
