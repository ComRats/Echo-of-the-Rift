using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TongueAnimation : MonoBehaviour, 
    IPointerEnterHandler, IPointerExitHandler,
    IPointerDownHandler, IPointerUpHandler,
    ISelectHandler, IDeselectHandler
{
    [SerializeField] private float highlightOffsetY = 10f;
    [SerializeField] private float pressOffsetY = 20f;
    [SerializeField] private float tweenDuration = 0.2f;

    private RectTransform rectTransform;
    private Button button;
    private Vector2 originalPos;
    private Tween currentTween;

    private void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        button = GetComponent<Button>();
        originalPos = rectTransform.anchoredPosition;

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (button.interactable)
            AnimateTo(originalPos + Vector2.up * highlightOffsetY);

        Debug.Log("Enter");
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (button.interactable)
            AnimateTo(originalPos);

        Debug.Log("Exit");
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (button.interactable)
            AnimateTo(originalPos + Vector2.up * pressOffsetY);

        Debug.Log("Down");
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (button.interactable)
            AnimateTo(originalPos + Vector2.up * highlightOffsetY); // возвращаемся в Highlighted

        Debug.Log("Up");
    }

    public void OnSelect(BaseEventData eventData)
    {
        if (button.interactable)
            AnimateTo(originalPos + Vector2.up * pressOffsetY); // выделение как нажатие

        Debug.Log("Select");
    }

    public void OnDeselect(BaseEventData eventData)
    {
        if (button.interactable)
            AnimateTo(originalPos);
    }

    private void AnimateTo(Vector2 targetPos)
    {
        currentTween?.Kill(); // отменить предыдущую анимацию, если она ещё идёт
        currentTween = rectTransform.DOAnchorPos(targetPos, tweenDuration).SetEase(Ease.OutQuad);
    }
}
