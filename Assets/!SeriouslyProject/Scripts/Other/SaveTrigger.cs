using EchoRift;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class SaveTrigger : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            GlobalLoader.Instance.SavePlayer();
        }
    }
}
