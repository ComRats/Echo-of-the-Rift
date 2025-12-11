using System.Threading.Tasks;
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

    public async Task FadeInAsync()
    {
        //Debug.LogWarning("FadeInAsync");

        _canvasGroup.blocksRaycasts = true;

        await _canvasGroup
            .DOFade(1f, fadeDuration)
            .SetUpdate(true)
            .SetLink(gameObject)
            .AsyncWaitForCompletion();
    }

    public async Task FadeOutAsync()
    {
        //Debug.LogWarning("FadeOutAsync");

        _canvasGroup.blocksRaycasts = true;

        await _canvasGroup
            .DOFade(0f, fadeDuration)
            .SetUpdate(true)
            .SetLink(gameObject)
            .AsyncWaitForCompletion();

        _canvasGroup.blocksRaycasts = false;
    }

    public void SetAlpha(float alpha)
    {
        _canvasGroup.alpha = alpha;
        _canvasGroup.blocksRaycasts = alpha > 0.01f;
    }
}