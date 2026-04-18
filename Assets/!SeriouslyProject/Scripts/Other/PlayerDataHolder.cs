namespace EchoRift
{
    /// <summary>
    /// Статический хранитель данных игрока, доступный между сценами.
    /// Заполняется при создании персонажа и читается в изолированных сценах (например, Dice).
    /// </summary>
    public static class PlayerDataHolder
    {
        public static string PlayerName { get; set; } = "Игрок";
    }

    /// <summary>
    /// Runtime-сессия мини-игры Dice. Хранит ставку и итоговый баланс между сценами.
    /// </summary>
    public static class DiceSessionState
    {
        public static bool HasActiveSession { get; private set; }
        public static bool IsResolved { get; private set; }

        public static string PlayerName { get; private set; } = "Игрок";
        public static string NpcName { get; private set; } = "Компьютер";
        public static string ReturnSceneName { get; private set; } = string.Empty;
        public static int StartingCoins { get; private set; }
        public static int CurrentCoins { get; private set; }
        public static int BetAmount { get; private set; }

        public static bool CanStart(int currentCoins, int betAmount)
        {
            return betAmount > 0 && currentCoins >= betAmount;
        }

        public static bool TryStartSession(string playerName, int currentCoins, int betAmount, string returnSceneName, string npcName = "Компьютер")
        {
            if (!CanStart(currentCoins, betAmount))
                return false;

            PlayerName = string.IsNullOrWhiteSpace(playerName) ? "Игрок" : playerName;
            NpcName = string.IsNullOrWhiteSpace(npcName) ? "Компьютер" : npcName;
            ReturnSceneName = returnSceneName ?? string.Empty;
            StartingCoins = UnityEngine.Mathf.Max(0, currentCoins);
            CurrentCoins = StartingCoins;
            BetAmount = betAmount;
            IsResolved = false;
            HasActiveSession = true;

            return true;
        }

        public static void ResolveWin()
        {
            if (!HasActiveSession || IsResolved)
                return;

            CurrentCoins = StartingCoins + BetAmount;
            IsResolved = true;
        }

        public static void ResolveLoss()
        {
            if (!HasActiveSession || IsResolved)
                return;

            CurrentCoins = UnityEngine.Mathf.Max(0, StartingCoins - BetAmount);
            IsResolved = true;
        }

        public static void ResolveDraw()
        {
            if (!HasActiveSession || IsResolved)
                return;

            CurrentCoins = StartingCoins;
            IsResolved = true;
        }

        public static void Clear()
        {
            HasActiveSession = false;
            IsResolved = false;
            PlayerName = "Игрок";
            NpcName = "Компьютер";
            ReturnSceneName = string.Empty;
            StartingCoins = 0;
            CurrentCoins = 0;
            BetAmount = 0;
        }
    }
}
