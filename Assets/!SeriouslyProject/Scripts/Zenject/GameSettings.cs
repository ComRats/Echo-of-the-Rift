using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Installers/GameSettings")]
public class GameSettings : ScriptableObjectInstaller<GameSettings>
{
    [Header("UI Settings")]
    public KeyCode openInvenoryKey = KeyCode.E;
    public KeyCode openPauseMenuKey = KeyCode.Escape;

    public override void InstallBindings()
    {
        Container.Bind<GameSettings>().FromInstance(this).AsSingle();
        //Debug.Log("Succeful Binding GameSettings");
    }
}