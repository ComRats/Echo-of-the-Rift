using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.SequencerCommands;
using UnityEngine;

/// <summary>
/// Sequencer-команда: растемняет экран (alpha 1 -> 0).
///
/// Использование в поле Sequence диалога:
///   FadeOut()       - растемнить с дефолтной длительностью
///   FadeOut(1.2)    - растемнить за 1.2 секунды
/// </summary>
[AddComponentMenu("")]
public class SequencerCommandFadeOut : SequencerCommand
{
    private float _endTime;

    public void Awake()
    {
        var fader = GlobalLoader.Instance?.mainUI?.screenFader;
        if (fader == null)
        {
            Debug.LogWarning("[SequencerCommandFadeOut] ScreenFader не найден!");
            Stop();
            return;
        }

        string param = GetParameter(0);
        if (!string.IsNullOrEmpty(param) && float.TryParse(param,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out float customDuration))
        {
            fader.SetDuration(customDuration);
        }

        float duration = fader.FadeOutDuration;
        _endTime = DialogueTime.time + duration;

        fader.StartFadeOut();
    }

    public void Update()
    {
        if (DialogueTime.time >= _endTime)
        {
            GlobalLoader.Instance?.mainUI?.screenFader?.ResetDuration();
            Stop();
        }
    }
}
