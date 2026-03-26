using EchoRift.UI;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using Zenject;

/// <summary>
/// Вешается на NPC рядом с DialogueSystemTrigger.
/// При начале диалога регистрирует моба в бестиарии.
/// </summary>
public class DialogueTriggerMobRegistrar : MonoBehaviour, IMobProvider
{
    [SerializeField] private Mob mobData;

    [Inject] MainUI mainUI;

    public Mob MobData => mobData;
    
    private void OnEnable()
    {
        DialogueSystemEvents events = GetComponent<DialogueSystemEvents>();
        if (events != null)
            events.conversationEvents.onConversationStart.AddListener(OnConversationStart);
    }

    private void OnDisable()
    {
        DialogueSystemEvents events = GetComponent<DialogueSystemEvents>();
        if (events != null)
            events.conversationEvents.onConversationStart.RemoveListener(OnConversationStart);
    }

    private void OnConversationStart(Transform actor)
    {
        if (mobData == null) return;

        MobGuide guide = mainUI.playerUI.mobGuide;
        guide.AddMob(mobData);
    }
}
