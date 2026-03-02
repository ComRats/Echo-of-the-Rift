using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Анимация для кнопок способностей в боевой системе
/// Реализует hover, press и selected эффекты
/// </summary>
[RequireComponent(typeof(RectTransform))]
public class AbilityButtonAmimation : MonoBehaviour,
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler
{
    [Header("Animation Settings")]
    [SerializeField] private bool ignoreLayoutGroup = true; // Игнорировать GridLayoutGroup для позиционной анимации
    [SerializeField] private float hoverOffsetY = 10f;
    [SerializeField] private float pressOffsetY = 20f;
    [SerializeField] private float selectedOffsetY = 15f;
    [SerializeField] private float animationDuration = 0.2f;
    [SerializeField] private Ease animationEase = Ease.OutQuad;

    [Header("Scale Animation")]
    [SerializeField] private bool useScaleAnimation = true;
    [SerializeField] private float hoverScale = 1.1f;
    [SerializeField] private float pressScale = 0.9f;
    [SerializeField] private float selectedScale = 1.05f;
    [SerializeField] private Button button;
    [SerializeField] private RectTransform rectTransform;

    private Tween positionTween;
    private Tween scaleTween;
    private Vector2 originalPosition;
    private Vector3 originalScale;
    private bool isSelected;
    private bool isPressed;

    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (isSelected == value) return;

            isSelected = value;

            if (isSelected)
                AnimateToSelected();
            else
                AnimateToNormal();
        }
    }

    private void Awake()
    {
        originalPosition = rectTransform.anchoredPosition;
        originalScale = rectTransform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!button.interactable || isSelected || isPressed) return;
        AnimateToHover();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!button.interactable || isSelected) return;
        isPressed = false;
        AnimateToNormal();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!button.interactable) return;
        isPressed = true;
        AnimateToPress();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!button.interactable) return;

        isPressed = false;

        if (isSelected)
        {
            AnimateToSelected();
        }
        else
        {
            // Задержка перед переходом к hover, чтобы анимация нажатия успела проиграться
            DOVirtual.DelayedCall(animationDuration * 0.5f, () =>
            {
                if (!isPressed && !isSelected)
                    AnimateToHover();
            });
        }
    }

    private void AnimateToNormal()
    {
        AnimateTo(originalPosition, originalScale);
    }

    private void AnimateToHover()
    {
        Vector2 targetPos = ignoreLayoutGroup ? originalPosition : rectTransform.anchoredPosition;
        targetPos += Vector2.up * hoverOffsetY;
        Vector3 targetScale = useScaleAnimation ? originalScale * hoverScale : originalScale;
        AnimateTo(targetPos, targetScale);
    }

    private void AnimateToPress()
    {
        Vector2 targetPos = ignoreLayoutGroup ? originalPosition : rectTransform.anchoredPosition;
        targetPos += Vector2.up * pressOffsetY;
        Vector3 targetScale = useScaleAnimation ? originalScale * pressScale : originalScale;
        AnimateTo(targetPos, targetScale);
    }

    private void AnimateToSelected()
    {
        Vector2 targetPos = ignoreLayoutGroup ? originalPosition : rectTransform.anchoredPosition;
        targetPos += Vector2.up * selectedOffsetY;
        Vector3 targetScale = useScaleAnimation ? originalScale * selectedScale : originalScale;
        AnimateTo(targetPos, targetScale);
    }

    private void AnimateTo(Vector2 targetPosition, Vector3 targetScale)
    {
        // Останавливаем предыдущие анимации
        positionTween?.Kill();
        scaleTween?.Kill();

        // Анимация позиции (только если не игнорируем layout group)
        if (!ignoreLayoutGroup)
        {
            positionTween = rectTransform.DOAnchorPos(targetPosition, animationDuration)
                .SetEase(animationEase);
        }

        // Анимация масштаба (работает всегда)
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

    /// <summary>
    /// Сбросить анимацию к начальному состоянию
    /// </summary>
    public void ResetAnimation()
    {
        positionTween?.Kill();
        scaleTween?.Kill();
        
        rectTransform.anchoredPosition = originalPosition;
        rectTransform.localScale = originalScale;
        isSelected = false;
    }
}
