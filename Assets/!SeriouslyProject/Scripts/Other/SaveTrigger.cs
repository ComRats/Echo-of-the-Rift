using EchoRift;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SaveTrigger : MonoBehaviour
{
    [SerializeField] private bool showDebugMessage = true;
    [SerializeField] private bool saveOnEnter = true;
    [SerializeField] private bool saveOnExit = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (saveOnEnter && collision.TryGetComponent<Player>(out var player))
        {
            SaveGame();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (saveOnExit && collision.TryGetComponent<Player>(out var player))
        {
            SaveGame();
        }
    }

    private void SaveGame()
    {
        GlobalLoader.Instance.SavePlayer();
        GlobalLoader.Instance.SaveGlobal();
        GlobalLoader.Instance.SaveInventory();

        if (showDebugMessage)
        {
            Debug.Log($"[SaveTrigger] Игра сохранена в точке '{gameObject.name}'");
        }
    }
}
