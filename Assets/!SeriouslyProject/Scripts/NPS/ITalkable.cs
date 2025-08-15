using PixelCrushers.DialogueSystem;
using UnityEngine;

public interface ITalkable
{
    public void Talk (Collider2D trigger, DialogueSystemTrigger dialogueData, DialogueSystemTrigger conversation);
}
