using EchoRift;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DiceDebugLauncher : MonoBehaviour
{
    [Header("Debug Settings")]
    [SerializeField] private KeyCode launchKey = KeyCode.F8;
    [SerializeField] private int betAmount = 10;
    [SerializeField] private bool showDebugMessages = true;

    private void Update()
    {
        if (!Input.GetKeyDown(launchKey))
            return;

        TryLaunchDice();
    }

    public void TryLaunchDice()
    {
        if (SceneManager.GetActiveScene().name == "Dice")
            return;

        var inventory = GlobalLoader.Instance?.mainUI?.inventoryManager;
        var wallet = inventory?.Wallet;

        if (inventory == null || wallet == null)
        {
            Debug.LogWarning("[DiceDebugLauncher] InventoryManager или PlayerWallet не найден.");
            return;
        }

        inventory.SyncFromUI();

        int currentCoins = wallet.Coins;
        if (!DiceSessionState.CanStart(currentCoins, betAmount))
        {
            if (showDebugMessages)
                Debug.LogWarning($"[DiceDebugLauncher] Недостаточно монет для Dice. Ставка: {betAmount}, монет: {currentCoins}");
            return;
        }

        string playerName = string.IsNullOrWhiteSpace(PlayerDataHolder.PlayerName)
            ? "Игрок"
            : PlayerDataHolder.PlayerName;

        string returnSceneName = SceneManager.GetActiveScene().name;
        if (!DiceSessionState.TryStartSession(playerName, currentCoins, betAmount, returnSceneName))
        {
            Debug.LogWarning("[DiceDebugLauncher] Не удалось подготовить сессию Dice.");
            return;
        }

        GlobalLoader.Instance?.EnterIsolatedScene();
        GlobalLoader.Instance?.LoadToScene("Dice");

        if (showDebugMessages)
            Debug.Log($"[DiceDebugLauncher] Запуск Dice по клавише {launchKey}. Ставка: {betAmount}");
    }
}
