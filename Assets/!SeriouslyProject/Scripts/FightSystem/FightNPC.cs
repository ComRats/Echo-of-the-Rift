using EchoRift;
using UnityEngine;

public class FightNPC : BaseTrigger
{
    private void Start()
    {
        EventApply();
        ApplyCoinResult();
    }

    private void ApplyCoinResult()
    {
        var wallet = GlobalLoader.Instance?.mainUI?.inventoryManager?.Wallet;
        if (wallet == null) return;
    }

    public void AddCoins(int amount)
    {
        var wallet = GlobalLoader.Instance?.mainUI?.inventoryManager?.Wallet;
        wallet?.AddCoins(amount);
    }

    public void RemoveCoins(int amount)
    {
        var wallet = GlobalLoader.Instance?.mainUI?.inventoryManager?.Wallet;
        wallet?.TrySpendCoins(amount);
    }
}
