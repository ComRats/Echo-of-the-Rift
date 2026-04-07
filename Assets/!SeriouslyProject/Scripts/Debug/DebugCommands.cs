using UnityEngine;
using UnityEngine.SceneManagement;
using IngameDebugConsole;
using EchoRift.EchoRiftSaveLoadSystem;
using EchoRift;

/// <summary>
/// Команды отладочной консоли для EchoRift.
/// Открыть консоль: тильда (~) или кнопка в углу экрана.
/// Префаб должен лежать в Resources/AutoCreateObjects/IngameDebugConsole.prefab
/// </summary>
public static class DebugCommands
{
    // ── Шорткаты ──────────────────────────────────────────────────────────────
    // GlobalLoader.Instance — синглтон, обращаемся напрямую везде

    static Player           Player      => GlobalLoader.Instance?.playerInstance;
    static MainUI           UI          => GlobalLoader.Instance?.mainUI;
    static InventoryManager Inventory   => UI?.inventoryManager;
    static PlayerWallet     Wallet      => Inventory?.Wallet;
    static Team             Team        => Player?.team;

    // ── Инициализация ─────────────────────────────────────────────────────────

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void SpawnConsole()
    {
#if !UNITY_EDITOR && !DEVELOPMENT_BUILD
        return;
#endif
        var prefab = Resources.Load<GameObject>("AutoCreateObjects/IngameDebugConsole");
        if (prefab == null)
        {
            Debug.LogWarning("[DebugCommands] Префаб не найден в Resources/AutoCreateObjects/IngameDebugConsole");
            return;
        }
        Object.DontDestroyOnLoad(Object.Instantiate(prefab));
    }

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    private static void RegisterCommands()
    {
        DebugLogConsole.AddCommand<string>  ("scene.load",    "Загрузить сцену по имени",              LoadScene);
        DebugLogConsole.AddCommand          ("scene.reload",  "Перезагрузить текущую сцену",           ReloadScene);
        DebugLogConsole.AddCommand          ("scene.info",    "Показать текущую сцену",                SceneInfo);

        DebugLogConsole.AddCommand          ("player.info",   "Показать данные игрока",                PlayerInfo);
        DebugLogConsole.AddCommand<int>     ("player.hp",     "Установить HP игрока",                  SetPlayerHP);
        DebugLogConsole.AddCommand<int>     ("player.mana",   "Установить Mana игрока",                SetPlayerMana);
        DebugLogConsole.AddCommand<int>     ("player.xp",     "Добавить XP игроку",                    AddPlayerXP);
        DebugLogConsole.AddCommand<float,float>("player.tp",  "Телепортировать игрока (x y)",          TeleportPlayer);

        DebugLogConsole.AddCommand          ("team.info",     "Показать состояние команды",            TeamInfo);

        DebugLogConsole.AddCommand          ("save.now",      "Сохранить игру",                        SaveGame);
        DebugLogConsole.AddCommand          ("save.delete",   "Удалить все сохранения",                DeleteAllSaves);
        DebugLogConsole.AddCommand          ("save.path",     "Показать путь к сохранениям",           ShowSavePath);

        DebugLogConsole.AddCommand<int>     ("coins.add",     "Добавить монеты",                       AddCoins);
        DebugLogConsole.AddCommand<int>     ("coins.set",     "Установить монеты",                     SetCoins);
        DebugLogConsole.AddCommand<int>     ("coins.remove",  "Снять монеты",                          RemoveCoins);
        DebugLogConsole.AddCommand          ("coins.info",    "Показать баланс",                       CoinsInfo);

        DebugLogConsole.AddCommand<string,int>("item.add",    "Добавить предмет (itemName amount)",    AddItem);
        DebugLogConsole.AddCommand<string,int>("item.remove", "Удалить предмет (itemName amount)",     RemoveItem);

        DebugLogConsole.AddCommand<string>  ("ally.add",      "Добавить союзника (CharacterData name)",AddAlly);
        DebugLogConsole.AddCommand<int>     ("ally.remove",   "Удалить союзника по индексу",           RemoveAlly);

        DebugLogConsole.AddCommand          ("fight.info",    "Показать состояние боя",                FightInfo);
        DebugLogConsole.AddCommand          ("fight.win",     "Убить всех врагов",                     ForceFightWin);

        DebugLogConsole.AddCommand          ("time.info",     "Показать игровое время",                TimeInfo);
        DebugLogConsole.AddCommand<float>   ("time.scale",    "Установить Time.timeScale",             SetTimeScale);

        Debug.Log("[DebugCommands] Зарегистрировано. Введите 'help' для списка команд.");
    }

    // ── Сцены ─────────────────────────────────────────────────────────────────

    static void LoadScene(string name)  => SceneManager.LoadScene(name);
    static void ReloadScene()           => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    static void SceneInfo()             => Debug.Log($"[Scene] {SceneManager.GetActiveScene().name} (index: {SceneManager.GetActiveScene().buildIndex})");

    // ── Игрок ─────────────────────────────────────────────────────────────────

    static void PlayerInfo()
    {
        if (Player == null) { Debug.LogWarning("[Debug] Player не найден"); return; }
        var p = Player.playerSaver;
        Debug.Log($"[Player] HP: {p.Health}/{p.MaxHealth} | Mana: {p.Mana}/{p.MaxMana} | Lv: {p.Level} | XP: {p.CurrentXP}/{p.MaxXP} | DMG: {p.Damage} | Armor: {p.Armor}");
    }

    static void SetPlayerHP(int value)
    {
        if (Player == null) { Debug.LogWarning("[Debug] Player не найден"); return; }
        Player.playerSaver.Health = Mathf.Clamp(value, 0, Player.playerSaver.MaxHealth);
        Debug.Log($"[Player] HP = {Player.playerSaver.Health}");
    }

    static void SetPlayerMana(int value)
    {
        if (Player == null) { Debug.LogWarning("[Debug] Player не найден"); return; }
        Player.playerSaver.Mana = Mathf.Clamp(value, 0, Player.playerSaver.MaxMana);
        Debug.Log($"[Player] Mana = {Player.playerSaver.Mana}");
    }

    static void AddPlayerXP(int amount)
    {
        if (Player == null) { Debug.LogWarning("[Debug] Player не найден"); return; }
        Player.playerSaver.CurrentXP += amount;
        Debug.Log($"[Player] XP +{amount} → {Player.playerSaver.CurrentXP}");
    }

    static void TeleportPlayer(float x, float y)
    {
        if (Player == null) { Debug.LogWarning("[Debug] Player не найден"); return; }
        Player.transform.position = new Vector3(x, y, Player.transform.position.z);
        Debug.Log($"[Player] Телепортирован в ({x}, {y})");
    }

    // ── Команда ───────────────────────────────────────────────────────────────

    static void TeamInfo()
    {
        if (Team == null) { Debug.LogWarning("[Debug] Team не найден"); return; }
        foreach (var c in Team.characters)
            Debug.Log($"[Team] {c.Name} | HP: {c.Health}/{c.MaxHealth} | Mana: {c.Mana}/{c.MaxMana} | Lv: {c.Level}");
    }

    // ── Сохранения ────────────────────────────────────────────────────────────

    static void SaveGame()
    {
        GlobalLoader.Instance?.SaveGlobal();
        GlobalLoader.Instance?.SavePlayer();
        Debug.Log("[Save] Сохранено");
    }

    static void DeleteAllSaves()
    {
        SaveLoadSystem.ClearAllSaves(SaveFileNames.GAME_DIRECTORY);
        Debug.Log("[Save] Все сохранения удалены");
    }

    static void ShowSavePath() =>
        Debug.Log($"[Save] {Application.persistentDataPath}/{SaveFileNames.GAME_DIRECTORY}");

    // ── Монеты ────────────────────────────────────────────────────────────────

    static void AddCoins(int amount)
    {
        if (Wallet == null) { Debug.LogWarning("[Debug] Wallet не найден"); return; }
        Wallet.AddCoins(amount);
        Debug.Log($"[Coins] +{amount} → {Wallet.Coins}");
    }

    static void SetCoins(int amount)
    {
        if (Wallet == null) { Debug.LogWarning("[Debug] Wallet не найден"); return; }
        Wallet.SetCoins(amount);
        Debug.Log($"[Coins] = {Wallet.Coins}");
    }

    static void RemoveCoins(int amount)
    {
        if (Wallet == null) { Debug.LogWarning("[Debug] Wallet не найден"); return; }
        Wallet.TrySpendCoins(amount);
        Debug.Log($"[Coins] -{amount} → {Wallet.Coins}");
    }

    static void CoinsInfo()
    {
        if (Wallet == null) { Debug.LogWarning("[Debug] Wallet не найден"); return; }
        Debug.Log($"[Coins] {Wallet.Coins}");
    }

    // ── Предметы ──────────────────────────────────────────────────────────────

    static void AddItem(string itemName, int amount)
    {
        if (Inventory == null) { Debug.LogWarning("[Debug] Inventory не найден"); return; }
        bool ok = Inventory.AddItem(itemName, amount);
        Debug.Log(ok ? $"[Item] +{itemName} x{amount}" : $"[Item] Не найден предмет '{itemName}'");
    }

    static void RemoveItem(string itemName, int amount)
    {
        if (Inventory == null) { Debug.LogWarning("[Debug] Inventory не найден"); return; }
        bool ok = Inventory.RemoveItem(itemName, amount);
        Debug.Log(ok ? $"[Item] -{itemName} x{amount}" : $"[Item] '{itemName}' не найден в инвентаре");
    }

    // ── Союзники ──────────────────────────────────────────────────────────────

    static void AddAlly(string characterDataName)
    {
        if (Team == null) { Debug.LogWarning("[Debug] Team не найден"); return; }
        Team.AddCharacter(characterDataName);
        UI?.teamManager?.UpdateTeamUI();
        Debug.Log($"[Ally] Добавлен: {characterDataName}");
    }

    static void RemoveAlly(int index)
    {
        if (Team == null) { Debug.LogWarning("[Debug] Team не найден"); return; }
        if (index < 0 || index >= Team.characters.Count)
        {
            Debug.LogWarning($"[Ally] Индекс {index} вне диапазона (0–{Team.characters.Count - 1})");
            return;
        }
        string charName = Team.characters[index].Name;
        Team.characters.RemoveAt(index);
        UI?.teamManager?.UpdateTeamUI();
        Debug.Log($"[Ally] Удалён [{index}]: {charName}");
    }

    // ── Бой ───────────────────────────────────────────────────────────────────

    static void FightInfo()
    {
        var fm = Object.FindObjectOfType<FightManager>();
        if (fm == null) { Debug.Log("[Fight] Не в сцене боя"); return; }
        Debug.Log($"[Fight] Персонажи: {fm.characters.Count} | Враги: {fm.enemies.Count}");
        foreach (var c in fm.characters) Debug.Log($"  [Char]  {c.name} HP: {c.Health}");
        foreach (var e in fm.enemies)    Debug.Log($"  [Enemy] {e.name} HP: {e.Health}");
    }

    static void ForceFightWin()
    {
        var fm = Object.FindObjectOfType<FightManager>();
        if (fm == null) { Debug.LogWarning("[Fight] FightManager не найден"); return; }
        foreach (var e in fm.enemies.ToArray()) e.TakeDamage(99999);
        Debug.Log("[Fight] Все враги убиты");
    }

    // ── Время ─────────────────────────────────────────────────────────────────

    static void TimeInfo() =>
        Debug.Log($"[Time] scale: {Time.timeScale} | GameTime: {GameTimer.GameTime:F1}s");

    static void SetTimeScale(float scale)
    {
        Time.timeScale = Mathf.Clamp(scale, 0f, 10f);
        Debug.Log($"[Time] scale = {Time.timeScale}");
    }
}
