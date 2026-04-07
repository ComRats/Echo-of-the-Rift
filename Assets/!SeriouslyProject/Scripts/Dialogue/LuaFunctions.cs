using UnityEngine;

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

            Lua.RegisterFunction("GetStat", this, SymbolExtensions.GetMethodInfo(() => GetStat(string.Empty)));
            Lua.RegisterFunction("HasStat", this, SymbolExtensions.GetMethodInfo(() => HasStat(string.Empty, 0)));
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
            Lua.UnregisterFunction("GetStat");
            Lua.UnregisterFunction("HasStat");
        }
    }
}
