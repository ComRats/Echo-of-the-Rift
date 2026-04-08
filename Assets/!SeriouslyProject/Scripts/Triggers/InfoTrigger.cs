using EchoRift;
using UnityEngine;
using Zenject;
using Sirenix.OdinInspector;
using AudioManager.Core;
using AudioManager.Locator;
using UnityEngine.Events;

public class InfoTrigger : BaseTrigger
{
    [Title("Info Text")]
    [Tooltip(
        "<br>  � ������� ������\n" +
        "<b>text</b> � ������ �����\n" +
        "<i>text</i> � ������\n" +
        "<u>text</u> � ������������\n" +
        "<s>text</s> � �����������\n\n" +
        "<color=red>text</color> � ����\n" +
        "<size=150%>text</size> � ������\n\n" +
        "<sprite index=0> � ������ (TMP)"
    )]
    [SerializeField, TextArea(3, 6)]
    private string infoText;

    [Title("Interaction")]
    [SerializeField]
    private bool useButton = false;

    [ShowIf(nameof(useButton))]
    [SerializeField]
    private string useButtText = "���������";

    [Title("View")]
    [ShowIf(nameof(useButton))]
    [SerializeField]
    private Vector3 keyMassageOffset;

    [ShowIf(nameof(useButton))]
    [SerializeField]
    private Vector3 textMassageOffset;

    [ShowIf(nameof(useButton))]
    [SerializeField]
    private string useButtonMusic = "LockDoor1";

    [SerializeField]
    private UnityEvent onPressedButton;
    [SerializeField]
    private UnityEvent onTriggerEnter;
    [SerializeField]
    private UnityEvent onTriggerExit;
    [SerializeField]
    private UnityEvent onExitAfterPressed;

    private bool playerInside;
    private bool textShown;
    private bool isWasPressed = false;
    private IAudioManager service;

    private SpriteCollection sprites;
    private TimeManager timeManager;

    [Inject] private MainUI mainUI;
    [Inject] private GameSettings gameSettings;
    [Inject] private Player player;

    private void Start()
    {
        sprites = mainUI.spriteCollection;
        service = ServiceLocator.GetService();
        timeManager = GlobalLoader.Instance.timeManager;
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

        onPressedButton?.Invoke();
        if(!string.IsNullOrEmpty(useButtonMusic))
        {
            service = ServiceLocator.GetService();
            service.PlayOneShot(useButtonMusic);
        }

        isWasPressed = true;
        ShowButtonPrompt(false);
        player.thinking.SetThink(textShown ? infoText : "");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!collision.TryGetComponent<Player>(out _)) return;

        playerInside = true;
        textShown = false;
        onTriggerEnter?.Invoke();

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

        onTriggerExit?.Invoke();

        if (isWasPressed)
            onExitAfterPressed?.Invoke();

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

        if (!show || sprites?.sprites == null) return;

        int idx = gameSettings.GetSpriteIndex(gameSettings.useButton);
        if (idx >= sprites.sprites.Count) return;

        GameMassage.ButtonMassageWithText(
            gameObject,
            true,
            sprites.sprites[idx],
            useButtText,
            keyMassageOffset,
            textMassageOffset,
            textColor: Color.yellow
        );
    }

    public void SkipTimeToMorning() => timeManager.SkipToMorning();
    public void SkipTimeToEvening() => timeManager.SkipToEvening();
    public void Skip6hour() => timeManager.SkipTime(6);
}