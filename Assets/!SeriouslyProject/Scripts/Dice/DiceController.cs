using UnityEngine;
using System.Collections;

public class DiceController : MonoBehaviour
{
    [SerializeField] private Dice[] diceObjects;
    [SerializeField] private DiceUI diceUI;

    [SerializeField] private string playerName = "Игрок";
    [SerializeField] private string npcName = "Компьютер";

    private int playerScore = 0;
    private int npcScore = 0;
    private int roundsPlayed = 0;

    void Start()
    {
        diceUI.PlayerName = playerName;
        diceUI.NpcName = npcName;
        diceUI.UpdateScores(0, 0);
    }

    public void StartRound()
    {
        StartCoroutine(GameLoop());
    }

    private IEnumerator GameLoop()
    {
        diceUI.SetButtonInteractable(false);

        diceUI.UpdateStatus("Твой ход...");
        yield return StartCoroutine(PlayDiceAnimations());
        playerScore += GetTotalDiceScore();
        diceUI.UpdateScores(playerScore, npcScore);

        diceUI.UpdateStatus($"{npcName} ходит...");
        yield return new WaitForSeconds(1f);
        yield return StartCoroutine(PlayDiceAnimations());
        npcScore += GetTotalDiceScore();
        diceUI.UpdateScores(playerScore, npcScore);

        roundsPlayed++;
        
        if (roundsPlayed < 3)
        {
            diceUI.UpdateStatus($"Раунд {roundsPlayed} завершен!");
            yield return new WaitForSeconds(1f);
            diceUI.SetButtonInteractable(true);
        }
        else
        {
            string result = playerScore > npcScore ? $"{playerName} победил!" : (playerScore < npcScore ? $"{npcName} победил!" : "Ничья!");
            diceUI.UpdateStatus(result);
        }
    }

    private IEnumerator PlayDiceAnimations()
    {
        foreach (var dice in diceObjects) StartCoroutine(dice.RollAnimation());
        yield return new WaitForSeconds(0.6f);
    }

    private int GetTotalDiceScore()
    {
        int sum = 0;
        foreach (var d in diceObjects) sum += d.CurrentSide;
        return sum;
    }
}