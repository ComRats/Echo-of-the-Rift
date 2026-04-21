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
            Lua.RegisterFunction("LockForTutorial", this, typeof(LuaFunctions).GetMethod(nameof(LockForTutorial)));
            Lua.RegisterFunction("UnlockAfterTutorial", this, typeof(LuaFunctions).GetMethod(nameof(UnlockAfterTutorial)));
            Lua.RegisterFunction("UnlockAbility", this, SymbolExtensions.GetMethodInfo(() => UnlockAbility(string.Empty)));
            Lua.RegisterFunction("HasAbility", this, SymbolExtensions.GetMethodInfo(() => HasAbility(string.Empty)));

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

        /// <summary>
        /// Блокирует весь UI и движение игрока для обучения/катсцен.
        /// Вызывай в поле Script диалога: LockForTutorial()
        /// </summary>
        public void LockForTutorial()
        {
            var mainUI = GlobalLoader.Instance?.mainUI;
            if (mainUI != null)
            {
                mainUI.LockForTutorial();
                Debug.Log("[LuaFunctions] UI заблокирован для обучения");
            }
            else
            {
                Debug.LogWarning("[LuaFunctions] MainUI не найден для блокировки");
            }
        }

        /// <summary>
        /// Снимает блокировку после обучения/катсцены.
        /// Вызывай в поле Script диалога: UnlockAfterTutorial()
        /// </summary>
        public void UnlockAfterTutorial()
        {
            var mainUI = GlobalLoader.Instance?.mainUI;
            if (mainUI != null)
            {
                mainUI.UnlockAfterTutorial();
                Debug.Log("[LuaFunctions] UI разблокирован после обучения");
            }
            else
            {
                Debug.LogWarning("[LuaFunctions] MainUI не найден для разблокировки");
            }
        }

        /// <summary>
        /// Разблокирует способность игроку по имени.
        /// Вызывай в поле Script диалога: UnlockAbility("RudeBlow")
        /// Имя должно совпадать с именем объекта BattleAbility (не AbilityName, а название ассета).
        /// </summary>
        public void UnlockAbility(string abilityName)
        {
            var player = GlobalLoader.Instance?.playerInstance;
            if (player == null)
            {
                Debug.LogWarning("[UnlockAbility] playerInstance не найден!");
                return;
            }

            // Ищем CharacterAbilitySet через Team → первый персонаж
            var team = player.GetComponent<Team>();
            if (team == null || team.characters.Count == 0)
            {
                Debug.LogWarning("[UnlockAbility] Team не найден!");
                return;
            }

            var abilitySet = team.characters[0].GetCharacterData()?.AbilitySet;
            if (abilitySet == null)
            {
                // Пробуем напрямую через characterData поле
                abilitySet = team.characters[0].characterData?.AbilitySet;
            }
            if (abilitySet == null)
            {
                Debug.LogWarning("[UnlockAbility] AbilitySet не найден!");
                return;
            }

            // Ищем индекс способности по имени ассета или AbilityName
            int index = -1;
            for (int i = 0; i < abilitySet.abilities.Count; i++)
            {
                var a = abilitySet.abilities[i];
                if (a.ability == null) continue;
                if (a.ability.name == abilityName || a.ability.AbilityName == abilityName)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                Debug.LogWarning($"[UnlockAbility] Способность '{abilityName}' не найдена в AbilitySet. Доступные: {string.Join(", ", System.Linq.Enumerable.Select(abilitySet.abilities, a => a.ability?.name + "/" + a.ability?.AbilityName))}");
                return;
            }

            // Добавляем в activeAbilityIndices ScriptableObject
            abilitySet.ActivateAbility(index);

            // Сохраняем в PlayerSaver чтобы пережило перезапуск
            var saver = player.playerSaver;
            if (!saver.unlockedAbilityIndices.Contains(index))
                saver.unlockedAbilityIndices.Add(index);

            Debug.Log($"[UnlockAbility] Способность '{abilityName}' (индекс {index}) разблокирована");
        }

        /// <summary>
        /// Проверяет, разблокирована ли способность у игрока.
        /// </summary>
        public bool HasAbility(string abilityName)
        {
            var player = GlobalLoader.Instance?.playerInstance;
            if (player == null) return false;

            var team = player.GetComponent<Team>();
            if (team == null || team.characters.Count == 0) return false;

            var abilitySet = team.characters[0].GetCharacterData()?.AbilitySet;
            if (abilitySet == null) return false;

            for (int i = 0; i < abilitySet.abilities.Count; i++)
            {
                var a = abilitySet.abilities[i];
                if (a.ability == null) continue;
                if ((a.ability.name == abilityName || a.ability.AbilityName == abilityName)
                    && abilitySet.activeAbilityIndices.Contains(i))
                    return true;
            }
            return false;
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
            Lua.UnregisterFunction("LockForTutorial");
            Lua.UnregisterFunction("UnlockAfterTutorial");
            Lua.UnregisterFunction("UnlockAbility");
            Lua.UnregisterFunction("HasAbility");
        }
    }
}
