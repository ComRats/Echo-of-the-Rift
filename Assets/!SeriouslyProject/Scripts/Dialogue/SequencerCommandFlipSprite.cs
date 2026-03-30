using UnityEngine;
using PixelCrushers.DialogueSystem.SequencerCommands;

public class SequencerCommandFlipSprite : SequencerCommand
{
    void Start()
    {
        string objName = GetParameter(0);

        bool flip = GetParameterAsBool(1);

        GameObject obj = GameObject.Find(objName);

        if (obj != null)
        {
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                sr.flipX = flip;
            }
        }

        Stop();
    }
}