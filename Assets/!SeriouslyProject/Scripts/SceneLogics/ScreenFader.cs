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

    private void Awake() => _canvasGroup = GetComponent<CanvasGroup>();

    public async Task FadeInAsync()
    {
        _currentTween?.Kill();
        _canvasGroup.blocksRaycasts = true;
        _currentTween = _canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true).SetLink(gameObject);
        await _currentTween.AsyncWaitForCompletion();
    }

    public async Task FadeOutAsync()
    {
        _currentTween?.Kill();
        _canvasGroup.blocksRaycasts = true;
        _currentTween = _canvasGroup.DOFade(0f, fadeDuration * 1.5f).SetUpdate(true).SetLink(gameObject);
        await _currentTween.AsyncWaitForCompletion();
        if (this != null) _canvasGroup.blocksRaycasts = false;
    }

    public void SetAlpha(float alpha)
    {
        _currentTween?.Kill();
        _canvasGroup.alpha = alpha;
        _canvasGroup.blocksRaycasts = alpha > 0.01f;
    }
}