using EchoRift;
using PixelCrushers.DialogueSystem;
using UnityEngine;

public class SetPlayerToDialogue : MonoBehaviour
{
    private void Start()
    {
        DialogueSystemTriggerInitialize();
        DialogueSystemEventsInitialize("start");
        DialogueSystemEventsInitialize("end"); 
    }

    private void DialogueSystemTriggerInitialize()
    {
        DialogueSystemTrigger dialogue = GetComponent<DialogueSystemTrigger>();
        if (dialogue != null)
        {
            dialogue.conversationActor = FindObjectOfType<Player>().transform;
        }
    }

    private void DialogueSystemEventsInitialize(string _event)
    {
        if (_event == "start")
        {
            DialogueSystemEvents dialogue = GetComponent<DialogueSystemEvents>();
            if (dialogue != null)
            {
                Transform playerTransform = FindObjectOfType<Movement>().transform;
                Movement playerMovement = playerTransform.GetComponent<Movement>();

                dialogue.conversationEvents.onConversationStart.AddListener((playerTransform) => { playerMovement.canMove = !playerMovement.canMove; });
            }
        }
        else if (_event == "end")
        {
            DialogueSystemEvents dialogue = GetComponent<DialogueSystemEvents>();
            if (dialogue != null)
            {
                Transform playerTransform = FindObjectOfType<Movement>().transform;
                Movement playerMovement = playerTransform.GetComponent<Movement>();

                dialogue.conversationEvents.onConversationEnd.AddListener((playerTransform) => { playerMovement.canMove = !playerMovement.canMove; });
            }
        }
    }
}
