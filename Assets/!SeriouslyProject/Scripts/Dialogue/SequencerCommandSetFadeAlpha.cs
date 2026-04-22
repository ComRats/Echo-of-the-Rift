using PixelCrushers.DialogueSystem;
using PixelCrushers.DialogueSystem.SequencerCommands;
using UnityEngine;

/// <summary>
/// Sequencer-команда: мгновенно устанавливает alpha затемнения.
///
/// Использование в поле Sequence диалога:
///   SetFadeAlpha(1)   - полностью чёрный экран
///   SetFadeAlpha(0)   - полностью прозрачный
/// </summary>
[AddComponentMenu("")]
public class SequencerCommandSetFadeAlpha : SequencerCommand
{
    public void Awake()
    {
        var fader = GlobalLoader.Instance?.mainUI?.screenFader;
        if (fader == null)
        {
            Debug.LogWarning("[SequencerCommandSetFadeAlpha] ScreenFader не найден!");
            Stop();
            return;
        }

        string param = GetParameter(0);
        if (!string.IsNullOrEmpty(param) && float.TryParse(param,
            System.Globalization.NumberStyles.Float,
            System.Globalization.CultureInfo.InvariantCulture, out float alpha))
        {
            fader.SetAlpha(Mathf.Clamp01(alpha));
        }
        else
        {
            Debug.LogWarning("[SequencerCommandSetFadeAlpha] Укажи alpha: SetFadeAlpha(0..1)");
        }

        Stop();
    }
}
