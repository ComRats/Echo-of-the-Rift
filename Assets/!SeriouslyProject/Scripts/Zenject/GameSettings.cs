using UnityEngine;
using Zenject;

[CreateAssetMenu(fileName = "GameSettings", menuName = "Installers/GameSettings")]
public class GameSettings : ScriptableObjectInstaller<GameSettings>
{
    [Header("UI Settings")]
    public KeyCode openInvenoryKey = KeyCode.E;
    public KeyCode openPauseMenuKey = KeyCode.Escape;
    public KeyCode useButton = KeyCode.F;
    public KeyCode questWindowKey = KeyCode.J;

    [Header("Combat Settings")]
    [Tooltip("Задержка перед ходом противника (в секундах)")]
    [Range(0.5f, 5f)]
    public float enemyTurnDelay = 1.5f;
    
    [Tooltip("Скорость анимации хода противника")]
    [Range(0.5f, 3f)]
    public float enemyTurnSpeed = 1f;

    public override void InstallBindings()
    {
        Container.Bind<GameSettings>().FromInstance(this).AsSingle();
        //Debug.Log("Succeful Binding GameSettings");
    }
}