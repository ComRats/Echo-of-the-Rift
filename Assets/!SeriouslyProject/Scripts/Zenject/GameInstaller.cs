using EchoRift;
using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private Player player;
    [SerializeField] private MainUI mainUI;

    public override void InstallBindings()
    {
        var existingPlayer = Object.FindObjectOfType<Player>(true);
        var existingMainUI = Object.FindObjectOfType<MainUI>(true);
        var existingInventoryManager = Object.FindObjectOfType<InventoryManager>(true);

        var playerInstance = existingPlayer ?? Instantiate(player);
        var mainUIInstance = existingMainUI ?? Instantiate(mainUI);
        var inventoryManagerInstance = existingInventoryManager ?? Object.FindObjectOfType<InventoryManager>();

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

        // Регистрируем InventoryManager, если он существует на сцене
        if (inventoryManagerInstance != null && !Container.HasBinding<InventoryManager>())
        {
            Container.BindInstance(inventoryManagerInstance).AsSingle();
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