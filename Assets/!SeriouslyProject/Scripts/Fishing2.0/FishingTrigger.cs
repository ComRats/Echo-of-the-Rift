using System.Linq;
using EchoRift;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Fishing2
{

public class FishingTrigger : MonoBehaviour
{
    [SerializeField] private Fishing fishing;
    [SerializeField] private Vector3 keyMassageOffset;
    [SerializeField, Range(0, 30)] private int spriteIndex;

    [ValueDropdown("GetSpriteNames")]
    [SerializeField] private SpriteCollection sprites;
    private bool playerInside = false;

    private void Start()
    {
        sprites = FindObjectOfType<SpriteCollection>();
    }

    private void OnTriggerStay2D(Collider2D collision)
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
            GameMassage.ButtonMassage(gameObject, playerInside, sprites.sprites[spriteIndex], keyMassageOffset);
        }
    }

    private void Update()
    {
        if (playerInside && Input.GetKeyDown(KeyCode.F) && !fishing.IsFishing)
        {
            Debug.Log("Рыбалоуства пачалося");

            fishing.StartFishingProcess(this);
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
}
