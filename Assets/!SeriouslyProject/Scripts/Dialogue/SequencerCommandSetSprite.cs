using UnityEngine;
using PixelCrushers.DialogueSystem.SequencerCommands;

// Использование в Sequence:
// SetSprite(ЦелевойОбъект, ОбъектИсточник)
// Пример: SetSprite(NPC_Guard, NPC_Guard_Happy)
// Берёт спрайт у ОбъектаИсточника и ставит его ЦелевомуОбъекту.
// Работает даже если объекты выключены.
public class SequencerCommandSetSprite : SequencerCommand
{
    void Start()
    {
        string targetName = GetParameter(0);
        string sourceName = GetParameter(1);

        SpriteRenderer target = FindSpriteRenderer(targetName);
        SpriteRenderer source = FindSpriteRenderer(sourceName);

        if (target == null)
        {
            Debug.LogWarning($"[SetSprite] SpriteRenderer не найден на целевом объекте '{targetName}'");
            Stop();
            return;
        }

        if (source == null)
        {
            Debug.LogWarning($"[SetSprite] SpriteRenderer не найден на объекте-источнике '{sourceName}'");
            Stop();
            return;
        }

        target.sprite = source.sprite;
        Stop();
    }

    // FindObjectsOfType с true ищет в том числе выключенные объекты
    private SpriteRenderer FindSpriteRenderer(string objName)
    {
        foreach (var sr in FindObjectsOfType<SpriteRenderer>(true))
        {
            if (sr.gameObject.name == objName)
                return sr;
        }
        return null;
    }
}
