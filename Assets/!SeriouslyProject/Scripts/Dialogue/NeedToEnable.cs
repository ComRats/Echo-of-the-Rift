using PixelCrushers.DialogueSystem;
using UnityEngine;

public class NeedToEnable : MonoBehaviour
{
    [SerializeField] private DialogueSystemTrigger dialogueTrigger;

    public void EnableComponent()
    {
        dialogueTrigger.enabled = true;
    }
}
