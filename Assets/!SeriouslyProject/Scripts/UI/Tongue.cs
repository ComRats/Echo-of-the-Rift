using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Tongue : SelectableTab, 
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    [SerializeField] private float hoverOffsetY = 10f;
    [SerializeField] private float pressOffsetY = 20f;
    [SerializeField] private float selectedOffsetY = 15f;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private Ease animationEase = Ease.OutQuad;

    [Header("Scale Animation")]
    [SerializeField] private bool useScaleAnimation = true;
    [SerializeField] private float hoverScale = 1.05f;
    [SerializeField] private float pressScale = 0.95f;

    private Button button;
    private Tween positionTween;
    private Tween scaleTween;
    private Vector3 originalScale;

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (isSelected == value) return;

            isSelected = value;
            objectToOpen.SetActive(isSelected);

            if (isSelected)
                AnimateToSelected();
            else
                AnimateToNormal();
        }
    }

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
    }

    public void Init(System.Action<int> onClickCallback)
    {
        if (button == null)
            button = GetComponent<Button>();

        button.onClick.AddListener(() => onClickCallback(index));
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable || isSelected) return;
        AnimateToHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable || isSelected) return;
        AnimateToNormal();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        AnimateToPress();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;
        
        if (isSelected)
            AnimateToSelected();
        else
            AnimateToHover();
    }

    private void AnimateToNormal()
    {
        AnimateTo(originalPosition, originalScale);
    }

    private void AnimateToHover()
    {
        Vector2 targetPos = originalPosition + Vector2.up * hoverOffsetY;
        Vector3 targetScale = useScaleAnimation ? originalScale * hoverScale : originalScale;
        AnimateTo(targetPos, targetScale);
    }

    private void AnimateToPress()
    {
        Vector2 targetPos = originalPosition + Vector2.up * pressOffsetY;
        Vector3 targetScale = useScaleAnimation ? originalScale * pressScale : originalScale;
        AnimateTo(targetPos, targetScale);
    }

    private void AnimateToSelected()
    {
        Vector2 targetPos = originalPosition + Vector2.up * selectedOffsetY;
        AnimateTo(targetPos, originalScale);
    }

    private void AnimateTo(Vector2 targetPosition, Vector3 targetScale)
    {
        positionTween?.Kill();
        scaleTween?.Kill();

        positionTween = rectTransform.DOAnchorPos(targetPosition, animationDuration)
            .SetEase(animationEase);

        if (useScaleAnimation)
        {
            scaleTween = rectTransform.DOScale(targetScale, animationDuration)
                .SetEase(animationEase);
        }
    }

    private void OnDestroy()
    {
        positionTween?.Kill();
        scaleTween?.Kill();
    }
}
