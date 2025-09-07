using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Collider2D))]
public class StarTrigger : MonoBehaviour
{
    [SerializeField] private Image backPanel;
    [SerializeField] private Vector3 keyMassageOffset;
    [SerializeField, Range(0, 30)] private int spriteIndex;

    [ValueDropdown("GetSpriteNames")]
    [SerializeField] private SpriteCollection sprites;
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
            GameMassage.ButtonMassage(gameObject, playerInside, sprites.sprites[spriteIndex], keyMassageOffset);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            playerInside = false;
            backPanel.gameObject.SetActive(false);
            GameMassage.ButtonMassage(gameObject, playerInside, sprites.sprites[spriteIndex], keyMassageOffset);
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.F))
        {
            backPanel.gameObject.SetActive(!backPanel.gameObject.activeSelf);
        }
    }

    private string[] GetSpriteNames()
    {
        if (sprites == null || sprites.sprites == null)
            return new string[0];

        return sprites.sprites
            .Select((s, i) => $"{i}: {(s != null ? s.name : "<empty>")}")
            .ToArray();
    }
}
