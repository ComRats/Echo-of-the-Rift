using DG.Tweening;
using TMPro;
using UnityEngine;

public static class GameMassage
{
    private static GameObject newMessage;
    private static GameAlert activeAlert;

    public static void ButtonMassage(GameObject target, bool isShow, Sprite sprite, Vector3 offset = default)
    {
        if (isShow)
        {
            if (newMessage != null) return;

            newMessage = new GameObject("KeyMassage");
            newMessage.transform.position = target.transform.position + offset;
            newMessage.transform.localScale = new(2f, 2f, 0f);
            newMessage.transform.SetParent(target.transform);

            var sr = newMessage.AddComponent<SpriteRenderer>();
            sr.sprite = sprite;

            sr.sortingLayerName = "Player";
            sr.sortingOrder = 500;
        }
        else
        {
            if (newMessage != null)
            {
                Object.Destroy(newMessage);
                newMessage = null;
            }
        }
    }

    public static void WarningMassage(GameObject textPrefab, string massage, float duration, Color textColor)
    {
        GameObject textObj = Object.Instantiate(textPrefab, FindCanvas().transform);
        textObj.name = "WarningMassage";
        textObj.transform.SetAsLastSibling();

        RectTransform rect = textObj.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = Vector2.zero;

        TextMeshProUGUI textComponent = textObj.GetComponent<TextMeshProUGUI>();
        if (textComponent == null)
            textComponent = textObj.GetComponentInChildren<TextMeshProUGUI>();

        textComponent.text = massage;
        textComponent.color = textColor;

        float fadeDuration = 0.5f;

        Sequence seq = DOTween.Sequence();
        seq.Append(textComponent.DOFade(1f, fadeDuration));
        seq.AppendInterval(duration - fadeDuration * 2);
        seq.Append(textComponent.DOFade(0f, fadeDuration));
        seq.OnComplete(() => Object.Destroy(textObj));
    }

    public static void GameAlert(
        GameAlert alertPrefab,
        string message,
        string leftButtonText, System.Action leftButtonAction,
        string rightButtonText, System.Action rightButtonAction,
        float duration,
        Color textColor)
    {
        if (activeAlert != null) return;

        activeAlert = Object.Instantiate(alertPrefab, FindCanvas().transform);
        activeAlert.name = "GameAlert";
        activeAlert.transform.SetAsLastSibling();

        activeAlert.mainText.text = message;
        activeAlert.mainText.color = textColor;

        activeAlert.leftButtonText.text = leftButtonText;
        activeAlert.leftButton.onClick.RemoveAllListeners();
        activeAlert.leftButton.onClick.AddListener(() =>
        {
            leftButtonAction?.Invoke();
            CloseAlert();
        });

        activeAlert.rightButtonText.text = rightButtonText;
        activeAlert.rightButton.onClick.RemoveAllListeners();
        activeAlert.rightButton.onClick.AddListener(() =>
        {
            rightButtonAction?.Invoke();
            CloseAlert();
        });

        var group = activeAlert.GetComponent<CanvasGroup>() ?? activeAlert.gameObject.AddComponent<CanvasGroup>();

        activeAlert.transform.DOKill();
        group.DOKill();

        group.alpha = 0f;
        group.DOFade(1f, 0.4f);

        activeAlert.transform.localScale = Vector3.zero;
        activeAlert.transform.DOScale(1f, 0.4f).SetEase(Ease.OutBack);
    }

    private static Canvas FindCanvas()
    {
        Canvas targetCanvas = null;
        var canvases = Object.FindObjectsOfType<Canvas>(true);
        foreach (var canvas in canvases)
        {
            if (!canvas.gameObject.activeInHierarchy) continue;
            if (canvas.CompareTag("Main")) return canvas;
            if (targetCanvas == null) targetCanvas = canvas;
        }
        return targetCanvas;
    }

    public static void CloseAlert()
    {
        if (activeAlert == null) return;

        var group = activeAlert.GetComponent<CanvasGroup>();
        var seq = DOTween.Sequence();
        seq.Append(group.DOFade(0f, 0.3f));
        seq.Join(activeAlert.transform.DOScale(0.8f, 0.3f));
        seq.OnComplete(() =>
        {
            Object.Destroy(activeAlert.gameObject);
            activeAlert = null;
        });
    }
}
