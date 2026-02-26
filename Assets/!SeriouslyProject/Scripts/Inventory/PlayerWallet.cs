using UnityEngine;
using System;

public class PlayerWallet : MonoBehaviour
{
    [SerializeField] private int coins = 100;

    public int Coins => coins;

    public event Action<int> OnCoinsChanged;

    public bool HasEnoughCoins(int amount)
    {
        return coins >= amount;
    }

    public bool TrySpendCoins(int amount)
    {
        if (!HasEnoughCoins(amount))
        {
            Debug.Log($"Недостаточно монет! Нужно: {amount}, Есть: {coins}");
            return false;
        }

        coins -= amount;
        OnCoinsChanged?.Invoke(coins);
        Debug.Log($"Потрачено {amount} монет. Осталось: {coins}");
        return true;
    }

    public void AddCoins(int amount)
    {
        if (amount <= 0) return;

        coins += amount;
        OnCoinsChanged?.Invoke(coins);
    }

    public void SetCoins(int amount)
    {
        int oldCoins = coins;
        coins = Mathf.Max(0, amount);
        OnCoinsChanged?.Invoke(coins);
    }
}
