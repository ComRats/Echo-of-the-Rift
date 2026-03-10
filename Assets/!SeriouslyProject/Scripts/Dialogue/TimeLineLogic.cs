using PixelCrushers.DialogueSystem;
using UnityEngine;

public class TimeLineLogic : MonoBehaviour
{
    public void StartConversationDelay()
    {
        Invoke(nameof(StartConversation), 1f);
    }

    public void ContinueConversationDelay()
    {
        Invoke(nameof(ContinueConversation), 1f);
    }

    private void StartConversation()
    {
        DialogueManager.StartConversation("StartConverastion");
    }

    private void ContinueConversation(string conversationName = "Horsemen1")
    {
        DialogueManager.StartConversation(conversationName);
    }

    public void StopDialogue()
    {
        DialogueManager.Pause();
        DialogueManager.SetDialoguePanel(false);
    }

    public void UnPauseDialogue()
    {
        DialogueManager.Unpause();
        DialogueManager.SetDialoguePanel(true);
    }
}
