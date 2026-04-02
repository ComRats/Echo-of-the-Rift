using UnityEngine;

namespace PixelCrushers.DialogueSystem
{
    /// <summary>
    /// Регистрирует Lua функции для работы с инвентарем в Dialogue System
    /// Позволяет проверять наличие предметов и управлять ими через Conditions и Scripts
    /// </summary>
    public class LuaFunctions : MonoBehaviour
    {
        private InventoryManager inventoryManager;

        private void Awake()
        {
            // Регистрируем Lua функции при старте
            Lua.RegisterFunction("HasItem", this, SymbolExtensions.GetMethodInfo(() => HasItem(string.Empty)));
            Lua.RegisterFunction("HasItemCount", this, SymbolExtensions.GetMethodInfo(() => HasItemCount(string.Empty, 0)));
            Lua.RegisterFunction("GetItemCount", this, SymbolExtensions.GetMethodInfo(() => GetItemCount(string.Empty)));
            Lua.RegisterFunction("AddItem", this, SymbolExtensions.GetMethodInfo(() => AddItem(string.Empty, 0)));
            Lua.RegisterFunction("RemoveItem", this, SymbolExtensions.GetMethodInfo(() => RemoveItem(string.Empty, 0)));
            Lua.RegisterFunction("HasCoins", this, SymbolExtensions.GetMethodInfo(() => HasCoins(0)));
            Lua.RegisterFunction("AddCoins", this, SymbolExtensions.GetMethodInfo(() => AddCoins(0)));
            Lua.RegisterFunction("RemoveCoins", this, SymbolExtensions.GetMethodInfo(() => RemoveCoins(0)));

            // Характеристики игрока
            ///<summury>
            ///Доступные имена статов: health, maxhealth, mana, maxmana, 
            ///damage, magicdamage, armor, heal, priority, lucky, cratedamage, level, xp, maxxp
            ///</summury>
            Lua.RegisterFunction("GetStat", this, SymbolExtensions.GetMethodInfo(() => GetStat(string.Empty)));
            Lua.RegisterFunction("HasStat", this, SymbolExtensions.GetMethodInfo(() => HasStat(string.Empty, 0)));
        }

        private void Start()
        {
            // Получаем ссылку на InventoryManager
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

        /// <summary>
        /// Проверяет наличие предмета в инвентаре (хотя бы 1 штука)
        /// Использование в Conditions: HasItem("apple")
        /// </summary>
        public bool HasItem(string itemName)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[HasItem] InventoryManager не найден!");
                return false;
            }

            // ВАЖНО: Принудительно синхронизируем перед каждой проверкой
            inventoryManager.SyncFromUI();

            bool result = inventoryManager.HasItem(itemName);
            Debug.Log($"[HasItem] Проверка '{itemName}': {result} (количество: {inventoryManager.GetItemCount(itemName)})");
            return result;
        }

        /// <summary>
        /// Проверяет наличие определенного количества предмета
        /// Использование в Conditions: HasItemCount("apple", 5)
        /// </summary>
        public bool HasItemCount(string itemName, double count)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[HasItemCount] InventoryManager не найден!");
                return false;
            }

            // ВАЖНО: Принудительно синхронизируем перед каждой проверкой
            inventoryManager.SyncFromUI();

            bool result = inventoryManager.HasItem(itemName, (int)count);
            Debug.Log($"[HasItemCount] Проверка '{itemName}' >= {count}: {result} (фактически: {inventoryManager.GetItemCount(itemName)})");
            return result;
        }

        /// <summary>
        /// Возвращает количество предмета в инвентаре
        /// Использование в Conditions: GetItemCount("apple") >= 5
        /// </summary>
        public double GetItemCount(string itemName)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[GetItemCount] InventoryManager не найден!");
                return 0;
            }

            // ВАЖНО: Принудительно синхронизируем перед каждой проверкой
            inventoryManager.SyncFromUI();

            return inventoryManager.GetItemCount(itemName);
        }

        /// <summary>
        /// Добавляет предмет в инвентарь
        /// Использование в Scripts: AddItem("apple", 5)
        /// </summary>
        public void AddItem(string itemName, double amount)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[AddItem] InventoryManager не найден!");
                return;
            }

            inventoryManager.AddItem(itemName, (int)amount);
        }

        /// <summary>
        /// Удаляет предмет из инвентаря
        /// Использование в Scripts: RemoveItem("apple", 3)
        /// </summary>
        public void RemoveItem(string itemName, double amount)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[RemoveItem] InventoryManager не найден!");
                return;
            }

            inventoryManager.RemoveItem(itemName, (int)amount);
        }

        /// <summary>
        /// Проверяет наличие монет
        /// Использование в Conditions: HasCoins(100)
        /// </summary>
        public bool HasCoins(double amount)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[HasCoins] InventoryManager не найден!");
                return false;
            }

            // Синхронизация не нужна для монет, но добавим для консистентности
            inventoryManager.SyncFromUI();

            return inventoryManager.Wallet.HasEnoughCoins((int)amount);
        }

        /// <summary>
        /// Добавляет монеты
        /// Использование в Scripts: AddCoins(50)
        /// </summary>
        public void AddCoins(double amount)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[AddCoins] InventoryManager не найден!");
                return;
            }

            inventoryManager.Wallet.AddCoins((int)amount);
        }

        /// <summary>
        /// Удаляет монеты
        /// Использование в Scripts: RemoveCoins(50)
        /// </summary>
        public void RemoveCoins(double amount)
        {
            if (inventoryManager == null)
            {
                Debug.LogWarning($"[RemoveCoins] InventoryManager не найден!");
                return;
            }

            inventoryManager.Wallet.TrySpendCoins((int)amount);
        }

        /// <summary>
        /// Возвращает значение характеристики игрока по имени.
        /// Доступные имена: health, maxhealth, mana, maxmana, damage, magicdamage,
        ///                  armor, heal, priority, lucky, cratedamage, level, xp, maxxp
        /// Использование в Conditions: GetStat("mana") >= 30
        /// </summary>
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

        /// <summary>
        /// Проверяет, что характеристика игрока >= заданного значения.
        /// Использование в Conditions: HasStat("mana", 30)
        /// </summary>
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
