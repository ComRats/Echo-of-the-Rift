using System.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup), typeof(Image))]
public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;
    private CanvasGroup _canvasGroup;
    private Tween _currentTween;
    private float _customDuration = -1f;

    private void Awake() => _canvasGroup = GetComponent<CanvasGroup>();

    public void SetDuration(float duration)
    {
        _customDuration = duration;
    }

    public void ResetDuration()
    {
        _customDuration = -1f;
    }

    public float FadeInDuration => _customDuration > 0 ? _customDuration : fadeDuration;
    public float FadeOutDuration => _customDuration > 0 ? _customDuration : fadeDuration * 1.5f;

    private float GetFadeInDuration() => FadeInDuration;
    private float GetFadeOutDuration() => FadeOutDuration;

    public async Task FadeInAsync()
    {
        _currentTween?.Kill();
        _canvasGroup.blocksRaycasts = true;
        _currentTween = _canvasGroup.DOFade(1f, GetFadeInDuration()).SetUpdate(true);
        await _currentTween.AsyncWaitForCompletion();
    }

    public async Task FadeOutAsync()
    {
        _currentTween?.Kill();
        _canvasGroup.blocksRaycasts = true;
        _currentTween = _canvasGroup.DOFade(0f, GetFadeOutDuration()).SetUpdate(true);
        await _currentTween.AsyncWaitForCompletion();
        if (this != null) _canvasGroup.blocksRaycasts = false;
    }

    /// <summary>
    /// Запускает затемнение без async — для использования из Sequencer-команд.
    /// </summary>
    public void StartFadeIn()
    {
        _currentTween?.Kill();
        _canvasGroup.blocksRaycasts = true;
        _currentTween = _canvasGroup.DOFade(1f, GetFadeInDuration()).SetUpdate(true);
    }

    /// <summary>
    /// Запускает растемнение без async — для использования из Sequencer-команд.
    /// </summary>
    public void StartFadeOut()
    {
        _currentTween?.Kill();
        _canvasGroup.blocksRaycasts = true;
        _currentTween = _canvasGroup.DOFade(0f, GetFadeOutDuration()).SetUpdate(true)
            .OnComplete(() => { if (this != null) _canvasGroup.blocksRaycasts = false; });
    }

    public void SetAlpha(float alpha)
    {
        _currentTween?.Kill();
        _canvasGroup.alpha = alpha;
        _canvasGroup.blocksRaycasts = alpha > 0.01f;
    }
}