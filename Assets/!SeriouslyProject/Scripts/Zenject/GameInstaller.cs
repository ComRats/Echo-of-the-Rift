using EchoRift;
using EchoRift.Shop;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private Player player;
    [SerializeField] private MainUI mainUI;
    [SerializeField] private ShopManager shopManager;

    public override void InstallBindings()
    {
        var existingPlayer = Object.FindObjectOfType<Player>(true);
        var existingMainUI = Object.FindObjectOfType<MainUI>(true);
        var existingShopManager = Object.FindObjectOfType<ShopManager>(true);

        var playerInstance = existingPlayer ?? Instantiate(player);
        var mainUIInstance = existingMainUI ?? Instantiate(mainUI);
        var shopManagerInstance = existingShopManager ?? (shopManager != null ? Instantiate(shopManager) : null);

        var playerUI = mainUIInstance.playerUI;

        if (existingPlayer == null)
            Object.DontDestroyOnLoad(playerInstance.gameObject);

        if (existingMainUI == null)
            Object.DontDestroyOnLoad(mainUIInstance.gameObject);

        if (!Container.HasBinding<Player>())
            Container.BindInstance(playerInstance).AsSingle();

        if (!Container.HasBinding<MainUI>())
            Container.BindInstance(mainUIInstance).AsSingle();

        if (!Container.HasBinding<PlayerUI>())
            Container.BindInstance(playerUI).AsSingle();

        // Регистрация ShopManager
        if (shopManagerInstance != null)
        {
            if (existingShopManager == null)
                Object.DontDestroyOnLoad(shopManagerInstance.gameObject);

            if (!Container.HasBinding<ShopManager>())
                Container.BindInstance(shopManagerInstance).AsSingle();

            Container.InjectGameObject(shopManagerInstance.gameObject);

            // Инициализация ShopManager
            var inventoryManager = mainUIInstance.inventoryManager;
            var playerWallet = inventoryManager?.Wallet;
            if (inventoryManager != null && playerWallet != null)
            {
                shopManagerInstance.Initialize(inventoryManager, playerWallet);
            }
        }

        Container.InjectGameObject(playerInstance.gameObject);
        Container.InjectGameObject(mainUIInstance.gameObject);

        playerInstance.Hide();
        mainUIInstance.Hide();

        //Debug.Log("Succeful Binding GameInstaller");

    }

    //���� ���������, ����������
    //private void HidePlayerAndMenu(Player playerInst, MainMenu mainMenuInst)
    //{
    //    playerInst.Hide();
    //}
}