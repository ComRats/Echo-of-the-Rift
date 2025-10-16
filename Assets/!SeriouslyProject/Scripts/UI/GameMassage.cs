using DG.Tweening;
using TMPro;
using UnityEngine;

public static class GameMassage
{
    private static GameObject newMessage;

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

    public static void WarningMassage(GameObject textPrefab, string massage, int duration, Color textColor)
    {
        Canvas canvas = Object.FindObjectOfType<Canvas>();
        GameObject textObj = Object.Instantiate(textPrefab, canvas.transform);
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


}
