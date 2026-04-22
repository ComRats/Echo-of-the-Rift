using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.SequencerCommands;
using UnityEngine;

/// <summary>
/// Универсальная Sequencer-команда: затемняет или растемняет экран.
///
/// Использование в поле Sequence диалога:
///   ScreenFade(in)        - затемнить с дефолтной длительностью
///   ScreenFade(out)       - растемнить с дефолтной длительностью
///   ScreenFade(in, 0.5)   - затемнить за 0.5 секунды
///   ScreenFade(out, 1.2)  - растемнить за 1.2 секунды
///
/// Пример цепочки:
///   ScreenFade(in)->FadeDone; SetActive(Пушка,true)@Message(FadeDone)->ActionDone; ScreenFade(out)@Message(ActionDone)
/// </summary>
[AddComponentMenu("")]
public class SequencerCommandScreenFade : SequencerCommand
{
    private float _endTime;

    public void Awake()
    {
        var fader = GlobalLoader.Instance?.mainUI?.screenFader;
        if (fader == null)
        {
            Debug.LogWarning("[SequencerCommandScreenFade] ScreenFader не найден!");
            Stop();
            return;
        }

        string direction = GetParameter(0, "in");
        bool fadeIn = string.Equals(direction, "in", System.StringComparison.OrdinalIgnoreCase);

        string durationParam = GetParameter(1);
        if (!string.IsNullOrEmpty(durationParam) && float.TryParse(durationParam,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out float customDuration))
        {
            fader.SetDuration(customDuration);
        }

        float duration = fadeIn ? fader.FadeInDuration : fader.FadeOutDuration;
        _endTime = DialogueTime.time + duration;

        if (fadeIn)
            fader.StartFadeIn();
        else
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
