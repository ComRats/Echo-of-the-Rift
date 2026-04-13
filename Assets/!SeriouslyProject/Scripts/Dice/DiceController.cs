using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using EchoRift;

public class DiceController : MonoBehaviour
{
    [SerializeField] private Dice[] diceObjects;
    [SerializeField] private DiceUI diceUI;

    [SerializeField] private string playerName = "Игрок";
    [SerializeField] private string npcName = "Компьютер";
    [SerializeField] private float returnDelay = 2.5f;

    private int playerScore = 0;
    private int npcScore = 0;
    private int roundsPlayed = 0;
    private int currentCoins = 0;
    private int currentBet = 0;
    private bool isFinishingGame;

    void Start()
    {
        GlobalLoader.Instance?.MarkIsolatedSceneLoaded(SceneManager.GetActiveScene().name);
        EnsureSingleAudioListener();
        CursorManager.Show();
        InitializeSession();

        diceUI.PlayerName = playerName;
        diceUI.NpcName = npcName;
        diceUI.UpdateScores(0, 0);
        diceUI.UpdateCoins(currentCoins);
        diceUI.UpdateStatus(currentBet > 0 ? $"Ставка: {currentBet} монет. Нажми бросить." : "Нажми бросить.");
    }

    public void StartRound()
    {
        if (isFinishingGame)
            return;

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
            FinishGame();
        }
    }

    private void InitializeSession()
    {
        if (global::EchoRift.DiceSessionState.HasActiveSession)
        {
            playerName = global::EchoRift.DiceSessionState.PlayerName;
            currentCoins = global::EchoRift.DiceSessionState.CurrentCoins;
            currentBet = global::EchoRift.DiceSessionState.BetAmount;
            return;
        }

        if (!string.IsNullOrEmpty(PlayerDataHolder.PlayerName))
            playerName = PlayerDataHolder.PlayerName;

        var wallet = GlobalLoader.Instance?.mainUI?.inventoryManager?.Wallet;
        currentCoins = wallet != null ? wallet.Coins : 0;
        currentBet = 0;
    }

    private void EnsureSingleAudioListener()
    {
        AudioListener[] listeners = FindObjectsOfType<AudioListener>(true);
        if (listeners.Length <= 1)
            return;

        AudioListener activeListener = Camera.main != null ? Camera.main.GetComponent<AudioListener>() : null;
        if (activeListener == null && listeners.Length > 0)
            activeListener = listeners[0];

        foreach (var listener in listeners)
            listener.enabled = listener == activeListener;
    }

    private void FinishGame()
    {
        if (isFinishingGame)
            return;

        isFinishingGame = true;
        diceUI.SetButtonInteractable(false);

        string resultMessage;

        if (playerScore > npcScore)
        {
            global::EchoRift.DiceSessionState.ResolveWin();
            resultMessage = currentBet > 0
                ? $"{playerName} победил! Выигрыш: +{currentBet} монет."
                : $"{playerName} победил!";
        }
        else if (playerScore < npcScore)
        {
            global::EchoRift.DiceSessionState.ResolveLoss();
            resultMessage = currentBet > 0
                ? $"{npcName} победил! Проигрыш: -{currentBet} монет."
                : $"{npcName} победил!";
        }
        else
        {
            global::EchoRift.DiceSessionState.ResolveDraw();
            resultMessage = currentBet > 0
                ? "Ничья! Ставка не изменилась."
                : "Ничья!";
        }

        currentCoins = global::EchoRift.DiceSessionState.HasActiveSession ? global::EchoRift.DiceSessionState.CurrentCoins : currentCoins;
        ApplyCoinsToWallet(currentCoins);
        diceUI.UpdateCoins(currentCoins);
        diceUI.UpdateStatus(resultMessage);

        if (global::EchoRift.DiceSessionState.HasActiveSession && !string.IsNullOrWhiteSpace(global::EchoRift.DiceSessionState.ReturnSceneName))
            StartCoroutine(ReturnToPreviousScene());
    }

    private void ApplyCoinsToWallet(int coins)
    {
        var inventory = GlobalLoader.Instance?.mainUI?.inventoryManager;
        var wallet = inventory?.Wallet;

        if (wallet == null)
        {
            Debug.LogWarning("[DiceController] PlayerWallet не найден. Монеты не синхронизированы.");
            return;
        }

        wallet.SetCoins(coins);
    }

    private IEnumerator ReturnToPreviousScene()
    {
        yield return new WaitForSeconds(returnDelay);

        string returnSceneName = global::EchoRift.DiceSessionState.ReturnSceneName;
        if (string.IsNullOrWhiteSpace(returnSceneName))
        {
            global::EchoRift.DiceSessionState.Clear();
            yield break;
        }

        GlobalLoader.Instance?.PrepareReturnFromIsolatedScene();
        global::EchoRift.DiceSessionState.Clear();
        SceneManager.LoadScene(returnSceneName);
    }

    private void OnDestroy()
    {
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
