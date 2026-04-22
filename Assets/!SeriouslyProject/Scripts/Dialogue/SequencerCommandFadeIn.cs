using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.SequencerCommands;
using UnityEngine;

/// <summary>
/// Sequencer-команда: затемняет экран (alpha 0 -> 1).
///
/// Использование в поле Sequence диалога:
///   FadeIn()        - затемнить с дефолтной длительностью
///   FadeIn(0.5)     - затемнить за 0.5 секунды
/// </summary>
[AddComponentMenu("")]
public class SequencerCommandFadeIn : SequencerCommand
{
    private float _endTime;

    public void Awake()
    {
        var fader = GlobalLoader.Instance?.mainUI?.screenFader;
        if (fader == null)
        {
            Debug.LogWarning("[SequencerCommandFadeIn] ScreenFader не найден!");
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

        float duration = fader.FadeInDuration;
        _endTime = DialogueTime.time + duration;

        fader.StartFadeIn();
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
