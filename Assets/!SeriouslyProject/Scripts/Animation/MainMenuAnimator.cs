using DG.Tweening;
using UnityEngine;

public class MainMenuAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform[] items;
    [SerializeField] private float startOffset = 600f;

    [Header("Animation")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float delayStep = 0.08f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private Vector2[] originalPositions;

    private void Awake()
    {
        originalPositions = new Vector2[items.Length];
    }

    private void Start()
    {
        StartCoroutine(PlayOpenAnimationDelayed());
    }

    private System.Collections.IEnumerator PlayOpenAnimationDelayed()
    {
        // Ждём один кадр чтобы VerticalLayoutGroup успел пересчитать позиции
        yield return null;

        for (int i = 0; i < items.Length; i++)
        {
            originalPositions[i] = items[i].anchoredPosition;
        }

        PlayOpenAnimation();
    }

    public void PlayOpenAnimation()
    {
        for (int i = 0; i < items.Length; i++)
        {
            RectTransform item = items[i];

            item.anchoredPosition =
                originalPositions[i] + Vector2.left * startOffset;

            item.DOAnchorPos(originalPositions[i], duration)
                .SetDelay(i * delayStep)
                .SetEase(ease);
        }
    }

    public void PlayCloseAnimation()
    {
        for (int i = 0; i < items.Length; i++)
        {
            RectTransform item = items[i];

            item.DOAnchorPos(
                originalPositions[i] + Vector2.left * startOffset,
                duration)
                .SetDelay(i * delayStep)
                .SetEase(ease);
        }
    }
}