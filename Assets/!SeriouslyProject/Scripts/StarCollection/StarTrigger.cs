using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class StarTrigger : MonoBehaviour
{
    [SerializeField] private Image backPanel;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {

        }
    }
}
