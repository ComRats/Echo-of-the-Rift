using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class StarTrigger : MonoBehaviour
{
    [SerializeField] private Image backPanel;
    [SerializeField] private Vector3 keyMassageOffset;

    private SpriteCollection sprites;
    private GameMassage message = new();
    private bool playerInside = false;

    private void Start()
    {
        sprites = FindObjectOfType<SpriteCollection>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            playerInside = true;
            message.ButtonMassage(gameObject, playerInside, sprites.sprites[14], keyMassageOffset);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            playerInside = false;
            backPanel.gameObject.SetActive(false);
            message.ButtonMassage(gameObject, playerInside, sprites.sprites[14], keyMassageOffset);
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.F))
        {
            backPanel.gameObject.SetActive(!backPanel.gameObject.activeSelf);
        }
    }
}
