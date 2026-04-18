using System.Reflection;
using EchoRift;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PixelCrushers.DialogueSystem
{
    public class LuaFunctions : MonoBehaviour
    {
        private InventoryManager inventoryManager;

        private void Awake()
        {
            Lua.RegisterFunction("HasItem", this, SymbolExtensions.GetMethodInfo(() => HasItem(string.Empty)));
            Lua.RegisterFunction("HasItemCount", this, SymbolExtensions.GetMethodInfo(() => HasItemCount(string.Empty, 0)));
            Lua.RegisterFunction("GetItemCount", this, SymbolExtensions.GetMethodInfo(() => GetItemCount(string.Empty)));
            Lua.RegisterFunction("AddItem", this, SymbolExtensions.GetMethodInfo(() => AddItem(string.Empty, 0)));
            Lua.RegisterFunction("RemoveItem", this, SymbolExtensions.GetMethodInfo(() => RemoveItem(string.Empty, 0)));
            Lua.RegisterFunction("HasCoins", this, SymbolExtensions.GetMethodInfo(() => HasCoins(0)));
            Lua.RegisterFunction("AddCoins", this, SymbolExtensions.GetMethodInfo(() => AddCoins(0)));
            Lua.RegisterFunction("RemoveCoins", this, SymbolExtensions.GetMethodInfo(() => RemoveCoins(0)));
            Lua.RegisterFunction("StartDiceGame", this, SymbolExtensions.GetMethodInfo(() => StartDiceGame(0)));
            Lua.RegisterFunction("StartDiceGameWith", this, SymbolExtensions.GetMethodInfo(() => StartDiceGameWith(0, string.Empty)));

            Lua.RegisterFunction("GetStat", this, SymbolExtensions.GetMethodInfo(() => GetStat(string.Empty)));
            Lua.RegisterFunction("HasStat", this, SymbolExtensions.GetMethodInfo(() => HasStat(string.Empty, 0)));
            Lua.RegisterFunction("ShowAlert", this, SymbolExtensions.GetMethodInfo(() => ShowAlert(string.Empty)));
            Lua.RegisterFunction("IncludeInvalidEntries", this, typeof(LuaFunctions).GetMethod(nameof(IncludeInvalidEntries)));

            DialogueLua.SetVariable("DiceCanStart", false);
            DialogueLua.SetVariable("DiceBet", 0);
        }

        private void Start()
        {
            MainUI mainUI = GlobalLoader.Instance?.mainUI;
            if (mainUI != null)
            {
                inventoryManager = mainUI.inventoryManager;
            }

            if (inventoryManager == null)
            {
                Debug.LogError("[LuaFunctions] InventoryManager не найден!");
            }
        }

        public bool HasItem(string itemName)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[HasItem] InventoryManager не найден!");
                return false;
            }

            inventoryManager.SyncFromUI();

            bool result = inventoryManager.HasItem(itemName);
            Debug.Log($"[HasItem] Проверка '{itemName}': {result} (количество: {inventoryManager.GetItemCount(itemName)})");
            return result;
        }

        public bool HasItemCount(string itemName, double count)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[HasItemCount] InventoryManager не найден!");
                return false;
            }

            inventoryManager.SyncFromUI();

            bool result = inventoryManager.HasItem(itemName, (int)count);
            Debug.Log($"[HasItemCount] Проверка '{itemName}' >= {count}: {result} (фактически: {inventoryManager.GetItemCount(itemName)})");
            return result;
        }

        public double GetItemCount(string itemName)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[GetItemCount] InventoryManager не найден!");
                return 0;
            }

            inventoryManager.SyncFromUI();

            return inventoryManager.GetItemCount(itemName);
        }

        public void AddItem(string itemName, double amount)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[AddItem] InventoryManager не найден!");
                return;
            }

            inventoryManager.AddItem(itemName, (int)amount);
        }

        public void RemoveItem(string itemName, double amount)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[RemoveItem] InventoryManager не найден!");
                return;
            }

            inventoryManager.RemoveItem(itemName, (int)amount);
        }

        public bool HasCoins(double amount)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[HasCoins] InventoryManager не найден!");
                return false;
            }

            inventoryManager.SyncFromUI();

            return inventoryManager.Wallet.HasEnoughCoins((int)amount);
        }

        public void AddCoins(double amount)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[AddCoins] InventoryManager не найден!");
                return;
            }

            inventoryManager.Wallet.AddCoins((int)amount);
        }

        public void RemoveCoins(double amount)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[RemoveCoins] InventoryManager не найден!");
                return;
            }

            inventoryManager.Wallet.TrySpendCoins((int)amount);
        }

        public void StartDiceGame(double betAmount)
        {
            StartDiceGameWith(betAmount, "Компьютер");
        }

        public void StartDiceGameWith(double betAmount, string npcName)
        {
            Debug.Log($"[StartDiceGame] Вызов: betAmount={betAmount}, npcName='{npcName}'");
            if (inventoryManager == null)
            {
                Debug.LogWarning("[StartDiceGame] InventoryManager не найден!");
                return;
            }

            if (inventoryManager.Wallet == null)
            {
                Debug.LogWarning("[StartDiceGame] PlayerWallet не найден!");
                return;
            }

            inventoryManager.SyncFromUI();

            int normalizedBet = Mathf.RoundToInt((float)betAmount);
            int currentCoins = inventoryManager.Wallet.Coins;

            if (!global::EchoRift.DiceSessionState.CanStart(currentCoins, normalizedBet))
            {
                DialogueLua.SetVariable("DiceCanStart", false);
                DialogueLua.SetVariable("DiceBet", normalizedBet);
                Debug.LogWarning($"[StartDiceGame] Нельзя начать Dice. Ставка: {normalizedBet}, монет: {currentCoins}");
                return;
            }

            string playerName = GetCurrentPlayerName();
            string returnSceneName = SceneManager.GetActiveScene().name;

            if (!global::EchoRift.DiceSessionState.TryStartSession(playerName, currentCoins, normalizedBet, returnSceneName, npcName))
            {
                DialogueLua.SetVariable("DiceCanStart", false);
                DialogueLua.SetVariable("DiceBet", normalizedBet);
                Debug.LogWarning("[StartDiceGame] Не удалось инициализировать сессию Dice.");
                return;
            }

            PlayerDataHolder.PlayerName = playerName;
            DialogueLua.SetVariable("DiceCanStart", true);
            DialogueLua.SetVariable("DiceBet", normalizedBet);

            DialogueManager.StopConversation();
            GlobalLoader.Instance?.EnterIsolatedScene();
            GlobalLoader.Instance?.LoadToScene("Dice");
        }

        public double GetStat(string statName)
        {
            var stats = GetPlayerStats();
            if (stats == null) return 0;

            return statName.ToLower() switch
            {
                "health"      => stats.Health,
                "maxhealth"   => stats.MaxHealth,
                "mana"        => stats.Mana,
                "maxmana"     => stats.MaxMana,
                "damage"      => stats.Damage,
                "magicdamage" => stats.MagicDamage,
                "armor"       => stats.Armor,
                "heal"        => stats.Heal,
                "priority"    => stats.Priority,
                "lucky"       => stats.Lucky,
                "cratedamage" => stats.CreteDamage,
                "level"       => stats.Level,
                "xp"          => stats.CurrentXP,
                "maxxp"       => stats.MaxXP,
                _ => LogUnknownStat(statName)
            };
        }

        public bool HasStat(string statName, double amount)
        {
            return GetStat(statName) >= amount;
        }

        public void ShowAlert(string message)
        {
            DialogueManager.ShowAlert(message);
        }

        /// <summary>
        /// Включает Include Invalid Entries для текущего разговора.
        /// Вызывай в поле Script диалога: IncludeInvalidEntries()
        /// Сбрасывается автоматически после окончания разговора.
        /// </summary>
        public void IncludeInvalidEntries()
        {
            if (!DialogueManager.hasInstance) return;
            DialogueManager.displaySettings.inputSettings.includeInvalidEntries = true;
            DialogueManager.instance.conversationEnded += ResetIncludeInvalidEntries;
        }

        private void ResetIncludeInvalidEntries(Transform actor)
        {
            if (DialogueManager.hasInstance)
                DialogueManager.displaySettings.inputSettings.includeInvalidEntries = false;
            DialogueManager.instance.conversationEnded -= ResetIncludeInvalidEntries;
        }

        private EntityStats GetPlayerStats()
        {
            var player = GlobalLoader.Instance?.playerInstance;
            if (player == null)
            {
                Debug.LogWarning("[LuaFunctions] playerInstance не найден!");
                return null;
            }
            return player.playerSaver;
        }

        private double LogUnknownStat(string statName)
        {
            Debug.LogWarning($"[GetStat] Неизвестная характеристика: '{statName}'");
            return 0;
        }

        private string GetCurrentPlayerName()
        {
            string dialogueName = DialogueLua.GetVariable("PlayerName").asString;
            if (!string.IsNullOrWhiteSpace(dialogueName))
                return dialogueName;

            if (!string.IsNullOrWhiteSpace(PlayerDataHolder.PlayerName))
                return PlayerDataHolder.PlayerName;

            return "Игрок";
        }

        private void OnDestroy()
        {
            Lua.UnregisterFunction("HasItem");
            Lua.UnregisterFunction("HasItemCount");
            Lua.UnregisterFunction("GetItemCount");
            Lua.UnregisterFunction("AddItem");
            Lua.UnregisterFunction("RemoveItem");
            Lua.UnregisterFunction("HasCoins");
            Lua.UnregisterFunction("AddCoins");
            Lua.UnregisterFunction("RemoveCoins");
            Lua.UnregisterFunction("StartDiceGame");
            Lua.UnregisterFunction("StartDiceGameWith");
            Lua.UnregisterFunction("GetStat");
            Lua.UnregisterFunction("HasStat");
            Lua.UnregisterFunction("ShowAlert");
            Lua.UnregisterFunction("IncludeInvalidEntries");
        }
    }
}
