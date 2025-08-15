using PixelCrushers.DialogueSystem;
using UnityEngine;

public class SetPlayerToDialogueTriger : MonoBehaviour
{
    private void Start()
    {
        DialogueSystemTrigger dialogue = GetComponent<DialogueSystemTrigger>();
        dialogue.conversationActor = FindObjectOfType<TestMovement>().transform;
    }
}
