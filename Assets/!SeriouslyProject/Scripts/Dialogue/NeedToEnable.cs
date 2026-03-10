using PixelCrushers.DialogueSystem;
using System.Collections;
using UnityEngine;

public class NeedToEnable : MonoBehaviour
{
    [SerializeField] private MonoBehaviour component;

    public void EnableComponent()
    {
        //component.enabled = true;

        DialogueManager.StartConversation("StartConverastion");
        
    }
}
