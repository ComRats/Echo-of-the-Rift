using EchoRift;
using UnityEngine;

public class FightNPC : BaseTrigger
{
    [Header("Coin Reward")]
    [Tooltip("Монеты за победу (положительное значение). За поражение отнимаются.")]
    [SerializeField] private int coinRewardOnWin = 0;
    [SerializeField] private int coinPenaltyOnLose = 0;

    private void Start()
    {
        EventApply();
        ApplyCoinResult();
    }

    private void ApplyCoinResult()
    {
        var wallet = GlobalLoader.Instance?.mainUI?.inventoryManager?.Wallet;
        if (wallet == null) return;

        switch (Player.Result)
        {
            case FightResult.Win when coinRewardOnWin > 0:
                wallet.AddCoins(coinRewardOnWin);
                break;

            case FightResult.Lose when coinPenaltyOnLose > 0:
                wallet.TrySpendCoins(coinPenaltyOnLose);
                break;
        }
    }
}
