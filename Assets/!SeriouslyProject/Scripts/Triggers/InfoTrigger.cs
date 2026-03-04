using EchoRift;
using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;
using AudioManager.Core;
using AudioManager.Locator;

public class InfoTrigger : BaseTrigger
{
    [Title("Info Text")]
    [Tooltip(
        "<br>  Ч перенос строки\n" +
        "<b>text</b> Ч жирный текст\n" +
        "<i>text</i> Ч курсив\n" +
        "<u>text</u> Ч подчЄркнутый\n" +
        "<s>text</s> Ч зачЄркнутый\n\n" +
        "<color=red>text</color> Ч цвет\n" +
        "<size=150%>text</size> Ч размер\n\n" +
        "<sprite index=0> Ч иконка (TMP)"
    )]
    [SerializeField, TextArea(3, 6)]
    private string infoText;

    [Title("Interaction")]
    [SerializeField]
    private bool useButton = false;

    [ShowIf(nameof(useButton))]
    [SerializeField]
    private string useButtText = "ќсмотреть";

    [Title("View")]
    [ShowIf(nameof(useButton))]
    [SerializeField]
    private Vector3 keyMassageOffset;

    [ShowIf(nameof(useButton))]
    [SerializeField]
    private Vector3 textMassageOffset;

    [ShowIf(nameof(useButton))]
    [SerializeField, Range(0, 30)]
    private int spriteIndex = 14;

    [ShowIf(nameof(useButton))]
    [SerializeField]
    private string useButtonMusic = "LockDoor1";

    private bool playerInside;
    private bool textShown;
    private bool isWasPressed = false;
    private IAudioManager service;

    private SpriteCollection sprites;

    [Inject] private MainUI mainUI;
    [Inject] private GameSettings gameSettings;
    [Inject] private Player player;

    private void Start()
    {
        sprites = mainUI.spriteCollection;
        service = ServiceLocator.GetService();
    }

    private void Update()
    {
        if (!playerInside) return;
        if (mainUI.isOpenUI) return;

        if (useButton && Input.GetKeyDown(gameSettings.useButton) && !isWasPressed)
        {
            ToggleInfo();
        }
    }

    private void ToggleInfo()
    {
        textShown = !textShown;

        if (player == null) return;

        service.PlayOneShot(useButtonMusic);

        isWasPressed = true;
        ShowButtonPrompt(false);
        player.thinking.SetThink(textShown ? infoText : "");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Player>(out _)) return;

        playerInside = true;
        textShown = false;

        if (useButton && !mainUI.isOpenUI)
            ShowButtonPrompt(true);
        else
            ShowInstantInfo();
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Player>(out Player player)) return;

        playerInside = false;
        textShown = false;
        isWasPressed = false;

        player.thinking.SetThink("");
        ShowButtonPrompt(false);
    }

    private void ShowInstantInfo()
    {
        if (!useButton)
        {
            player?.thinking.SetThink(infoText);
        }
    }

    private void ShowButtonPrompt(bool show)
    {
        GameMassage.ButtonMassageWithText(
            gameObject,
            false,
            null,
            "",
            Vector3.zero,
            Vector3.zero
        );

        if (!show || sprites?.sprites == null || spriteIndex >= sprites.sprites.Count)
            return;

        GameMassage.ButtonMassageWithText(
            gameObject,
            true,
            sprites.sprites[spriteIndex],
            useButtText,
            keyMassageOffset,
            textMassageOffset,
            textColor: Color.yellow
        );
    }
}