using TMPro;
using UnityEngine;

public class ContextText : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI fjghtTurn;
    [SerializeField] private int turn;

    public void ChangeTurnText()
    {
        turn++;
        fjghtTurn.text = $"’Ó‰: {turn}";
    }
}
