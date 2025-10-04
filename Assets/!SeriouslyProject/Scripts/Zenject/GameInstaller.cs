using UnityEngine;
using Zenject;

public class GameInstaller : MonoInstaller
{
    [SerializeField] private Player player;
    [SerializeField] private MainUI mainUI;

    public override void InstallBindings()
    {
        var UIInstance = Container.InstantiatePrefabForComponent<MainUI>(mainUI);
        PlayerUI playerUI = UIInstance.GetComponentInChildren<PlayerUI>();
        Container.Bind<PlayerUI>().FromInstance(playerUI).AsSingle().NonLazy();
        DontDestroyOnLoad(UIInstance.gameObject);

        var playerInstance = Container.InstantiatePrefabForComponent<Player>(player);
        Container.Bind<Player>().FromInstance(playerInstance).AsSingle().NonLazy();
        DontDestroyOnLoad(playerInstance.gameObject);

        Debug.Log("Install Bindings in GameInstaller is succeful");
    }
}
