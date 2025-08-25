using UnityEngine;
using DG.Tweening;
using TMPro;

public class FightAnimation
{
    public static void ShowDamage(GameObject _textPrefab, int _damage, Transform _showPosition, Color _textColor)
    {
        GameObject newTextObj = GameObject.Instantiate(_textPrefab, _showPosition.position - new Vector3(0f, 0.5f, 0f), Quaternion.identity);
        var text = newTextObj.GetComponent<TextMeshProUGUI>();
        text.color = _textColor;
        text.text = _damage.ToString();

        var rect = newTextObj.GetComponent<RectTransform>();
        rect.SetParent(_showPosition.transform);
        rect.transform.localScale = new Vector3(0.5f, 0.5f, 0);

        var moveTween = rect.DOMoveY(rect.position.y + 1f, 1f).SetEase(Ease.OutCubic);
        var fadeTween = text.DOFade(0f, 1f).SetDelay(0.5f);

        Sequence seq = DOTween.Sequence();
        seq.Join(moveTween);
        seq.Join(fadeTween);
        seq.OnComplete(() =>
        {
            if (newTextObj != null)
                Object.Destroy(newTextObj);
        });
    }
}
