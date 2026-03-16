using EchoRift;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SaveTrigger : MonoBehaviour
{
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
        GlobalLoader.Instance.SaveGlobal(); // сохраняет Dialogue переменные, квесты, состояния
        GlobalLoader.Instance.SaveInventory();
    }
}
