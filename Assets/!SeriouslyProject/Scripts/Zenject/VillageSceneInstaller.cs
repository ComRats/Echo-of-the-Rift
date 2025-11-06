using UnityEngine;
using Zenject;

public class VillageSceneInstaller : MonoInstaller
{
    
    //[SerializeField] private Player player;
    //[SerializeField] private MainUI mainUI;

    //public override void InstallBindings()
    //{
    //    var existingPlayer = Object.FindObjectOfType<Player>(true);
    //    var existingMainUI = Object.FindObjectOfType<MainUI>(true);

    //    var playerInstance = existingPlayer ?? Instantiate(player);
    //    var mainUIInstance = existingMainUI ?? Instantiate(mainUI);

    //    var playerUI = mainUIInstance.playerUI;

    //    if (existingPlayer == null)
    //        Object.DontDestroyOnLoad(playerInstance.gameObject);

    //    if (existingMainUI == null)
    //        Object.DontDestroyOnLoad(mainUIInstance.gameObject);

    //    if (!Container.HasBinding<Player>())
    //        Container.BindInstance(playerInstance).AsSingle();

    //    if (!Container.HasBinding<MainUI>())
    //        Container.BindInstance(mainUIInstance).AsSingle();

    //    if (!Container.HasBinding<PlayerUI>())
    //        Container.BindInstance(playerUI).AsSingle();

    //    Container.InjectGameObject(playerInstance.gameObject);
    //    Container.InjectGameObject(mainUIInstance.gameObject);
    //}
}
