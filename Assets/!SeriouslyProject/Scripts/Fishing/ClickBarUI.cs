using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System;

public class ClickBarUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;
    [SerializeField] private Gradient fillGradient;
    [SerializeField] private RectTransform container;

    private float currentFill;
    private float drainSpeed;
    private bool isActive;
    private Action onComplete;
    private Action onFailed;

    private Tween fillTween;
    private Tween scaleTween;

    public void Setup(float startFill, float drain, Action completeCallback, Action failCallback = null)
    {
        isActive = false;
        drainSpeed = drain;
        currentFill = Mathf.Clamp01(startFill);
        onComplete = completeCallback;
        onFailed = failCallback;

        ResetVisuals(currentFill);

        gameObject.SetActive(true);
        isActive = true;
    }

    public void AddProgress(float amount)
    {
        if (!isActive) return;

        currentFill = Mathf.Clamp01(currentFill + amount);

        scaleTween?.Kill(true);
        container.localScale = Vector3.one;
        scaleTween = container.DOPunchScale(Vector3.one * 0.05f, 0.1f, 5, 1);

        fillTween?.Kill();
        fillTween = fillImage.DOFillAmount(currentFill, 0.1f).SetEase(Ease.OutQuad);
        fillImage.DOColor(fillGradient.Evaluate(currentFill), 0.1f);

        if (currentFill >= 1f) Complete();
    }

    private void Update()
    {
        if (!isActive) return;

        currentFill = Mathf.Max(0, currentFill - drainSpeed * Time.deltaTime);

        if (fillTween == null || !fillTween.IsActive())
        {
            fillImage.fillAmount = currentFill;
            fillImage.color = fillGradient.Evaluate(currentFill);
        }

        if (currentFill <= 0)
        {
            Failed();
        }
    }

    private void ResetVisuals(float startValue)
    {
        scaleTween?.Kill();
        fillTween?.Kill();

        container.localScale = Vector3.one;
        fillImage.fillAmount = startValue;
        fillImage.color = fillGradient.Evaluate(startValue);
    }

    private void Complete()
    {
        isActive = false;
        scaleTween?.Kill(true);
        fillTween?.Kill(true);
        onComplete?.Invoke();
        Hide();
    }

    private void Failed()
    {
        isActive = false;
        scaleTween?.Kill(true);
        fillTween?.Kill(true);
        onFailed?.Invoke();
        Hide();
    }

    public void Hide()
    {
        isActive = false;
        gameObject.SetActive(false);
    }
}