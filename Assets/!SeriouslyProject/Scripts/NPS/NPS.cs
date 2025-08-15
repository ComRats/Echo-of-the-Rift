using PixelCrushers.DialogueSystem;
using UnityEngine;

public class NPS : MonoBehaviour, ITalkable
{
    [SerializeField] private bool isTalkable;
    [SerializeField] private DialogueSystemTrigger DSTrigger;
    [SerializeField] private Collider2D trigger;

    public void Talk(Collider2D _trigger, DialogueSystemTrigger _dialogueData, DialogueSystemTrigger conversation)
    {
        throw new System.NotImplementedException();
    }
}
