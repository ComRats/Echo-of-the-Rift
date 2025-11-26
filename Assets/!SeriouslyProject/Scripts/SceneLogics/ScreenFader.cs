using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup), typeof(Image))]
public class ScreenFader : MonoBehaviour
{
    [SerializeField] private float fadeDuration = 0.5f;
    private CanvasGroup _canvasGroup;

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    public void FadeIn()
    {
        _canvasGroup.blocksRaycasts = true;
        _canvasGroup.DOFade(1f, fadeDuration).SetUpdate(true);
    }

    public void FadeOut()
    {
        _canvasGroup.DOFade(0f, fadeDuration).SetUpdate(true).OnComplete(() =>
        {
            _canvasGroup.blocksRaycasts = false;
        });
    }

    public void SetAlpha(float alpha)
    {
        _canvasGroup.alpha = alpha;
        _canvasGroup.blocksRaycasts = alpha > 0.01f;
    }
}