using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuAnimator : MonoBehaviour
{
    [SerializeField] private RectTransform[] items;
    [SerializeField] private VerticalLayoutGroup group;
    [SerializeField] private float startOffset = 600f;

    [Header("Animation")]
    [SerializeField] private float duration = 0.5f;
    [SerializeField] private float delayStep = 0.08f;
    [SerializeField] private Ease ease = Ease.OutCubic;

    private Vector2[] originalPositions;

    private void Awake()
    {
        group.enabled = true;
        LayoutRebuilder.ForceRebuildLayoutImmediate(
        group.GetComponent<RectTransform>());
        originalPositions = new Vector2[items.Length];
    }

    private void Start()
    {
        group.enabled = false;
        StartCoroutine(PlayOpenAnimationDelayed()); 
    }

    private IEnumerator PlayOpenAnimationDelayed()
    {
        yield return null;
        yield return null;

        LayoutRebuilder.ForceRebuildLayoutImmediate(group.GetComponent<RectTransform>());

        for (int i = 0; i < items.Length; i++)
        {
            originalPositions[i] = items[i].anchoredPosition;

            LayoutElement le = items[i].GetComponent<LayoutElement>();
            if (le != null)
                le.ignoreLayout = true;
        }

        group.enabled = false;

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