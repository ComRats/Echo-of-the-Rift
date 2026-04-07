using EchoRift;
using Sirenix.OdinInspector;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

[RequireComponent(typeof(Collider2D))]
public class StarTrigger : MonoBehaviour
{
    [SerializeField] private Image backPanel;
    [SerializeField] private Vector3 keyMassageOffset;

    [ValueDropdown("GetSpriteNames")]
    [SerializeField] private SpriteCollection sprites;

    private bool playerInside = false;
    [Inject] private MainUI mainUI;
    [Inject] private GameSettings gameSettings;

    private void Start()
    {
        sprites = mainUI.spriteCollection;
        backPanel = mainUI.starPanel.GetComponent<Image>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            playerInside = true;
            ShowPrompt(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            playerInside = false;
            backPanel.gameObject.SetActive(false);
            ShowPrompt(false);
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(gameSettings.useButton))
        {
            backPanel.gameObject.SetActive(!backPanel.gameObject.activeSelf);
        }
    }

    private void ShowPrompt(bool show)
    {
        if (sprites?.sprites == null) return;
        int idx = gameSettings.GetSpriteIndex(gameSettings.useButton);
        if (idx < sprites.sprites.Count)
            GameMassage.ButtonMassage(gameObject, show, sprites.sprites[idx], keyMassageOffset);
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
