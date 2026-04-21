using EchoRift;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class FightNPC : BaseTrigger
{
    private void Start()
    {
        EventApply();
        ApplyCoinResult();
    }

    public void StartDialogue(string conversationName)
    {
        if (string.IsNullOrEmpty(conversationName)) return;
        var player = GlobalLoader.Instance?.playerInstance;
        DialogueManager.StartConversation(conversationName, player?.transform);
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
