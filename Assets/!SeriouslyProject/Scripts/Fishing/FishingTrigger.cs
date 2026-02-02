using System.Linq;
using EchoRift;
using Sirenix.OdinInspector;
using UnityEngine;

public class FishingTrigger : MonoBehaviour
{
    [SerializeField] private Fishing fishing;
    [SerializeField] private Vector3 keyMassageOffset;
    [SerializeField, Range(0, 30)] private int spriteIndex;
    [ValueDropdown("GetSpriteNames")]
    [SerializeField] private SpriteCollection sprites;
    
    private Player currentPlayer;
    private bool playerInside = false;
    private GameObject buttonUI;

    private void Start()
    {
        sprites = FindObjectOfType<SpriteCollection>();

        if (transform.childCount > 0)
            buttonUI = transform.GetChild(0).gameObject;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out currentPlayer))
        {
            playerInside = true;
            ShowButtonPrompt(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.TryGetComponent<Player>(out var player))
        {
            playerInside = false;
            currentPlayer = null;
            ShowButtonPrompt(false);
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.F) && fishing != null && !fishing.IsFishing)
        {
            Debug.Log("Рыбалка началась");

            ShowButtonPrompt(false);
            
            fishing.StartFishingProcess(this);
            
            if (buttonUI != null)
                buttonUI.SetActive(false);
        }
    }

    private void ShowButtonPrompt(bool show)
    {
        if (sprites != null && sprites.sprites != null && spriteIndex < sprites.sprites.Count)
        {
            GameMassage.ButtonMassage(gameObject, show, sprites.sprites[spriteIndex], keyMassageOffset);
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

    public void ShowButtonAfterFishing()
    {
        if (playerInside)
        {
            ShowButtonPrompt(true);
            if (buttonUI != null)
                buttonUI.SetActive(true);
        }
    }
}