using UnityEngine;
using DG.Tweening;
using TMPro;

public static class FightAnimation
{
    private const float START_HEIGHT = 1.0f;
    private const float SIDE_SPREAD = 2.5f;
    private const float JUMP_POWER = 2.5f;
    private const float DURATION = 1.2f;

    private const float START_SCALE = 0f;
    private const float TARGET_SCALE = 0.8f;
    private const float SCALE_DURATION = 0.2f;
    private const float MIN_RANDOM_X = 0.5f;
    private const float MAX_RANDOM_X = 1.2f;
    private const float DIRECTION_MULTIPLIER = 2.0f;
    private const float SPAWN_X_OFFSET_MODIFIER = 5f;

    private const float FADE_START_PERCENT = 0.5f;
    private const float FADE_DURATION_PERCENT = 0.5f;
    private const float END_Y_OFFSET = -1.0f;

    public static void ShowText(GameObject _textPrefab, int _value, Transform _showPosition, Color _textColor)
    {
        ShowTextInternal(_textPrefab, _value.ToString(), _showPosition, _textColor, 0f);
    }

    public static void ShowText(GameObject _textPrefab, string _value, Transform _showPosition, Color _textColor, Vector3 _extraOffset = default)
    {
        ShowTextInternal(_textPrefab, _value, _showPosition, _textColor, 0f);
    }

    public static void ShowText(GameObject _textPrefab, string _value, Transform _showPosition, Color _textColor, float _animDelay)
    {
        ShowTextInternal(_textPrefab, _value, _showPosition, _textColor, _animDelay);
    }

    public static void ShowText(GameObject _textPrefab, string _value, Transform _showPosition, Color _textColor, Vector3 _extraOffset, float _animDelay)
    {
        ShowTextInternal(_textPrefab, _value, _showPosition, _textColor, _animDelay);
    }

    public static void ShowText(GameObject _textPrefab, int _value, Transform _showPosition, Color _textColor, Vector3 _extraOffset)
    {
        ShowTextInternal(_textPrefab, _value.ToString(), _showPosition, _textColor, 0f, _extraOffset);
    }

    private static void ShowTextInternal(GameObject _textPrefab, string _value, Transform _showPosition, Color _textColor, float _delay, Vector3 _extraOffset = default)
    {
        float direction = Random.value > 0.5f ? DIRECTION_MULTIPLIER : -DIRECTION_MULTIPLIER;
        float randomX = Random.Range(MIN_RANDOM_X, MAX_RANDOM_X) * direction;

        Vector3 spawnOffset = new Vector3(randomX * SPAWN_X_OFFSET_MODIFIER, START_HEIGHT, 0f);
        Vector3 spawnPosition = _showPosition.position + spawnOffset + _extraOffset;

        GameObject newTextObj = GameObject.Instantiate(_textPrefab, spawnPosition, Quaternion.identity);

        var text = newTextObj.GetComponent<TextMeshProUGUI>();
        text.color = _textColor;
        text.text = _value;

        var rect = newTextObj.GetComponent<RectTransform>();
        rect.SetParent(_showPosition.transform);
        rect.transform.localScale = Vector3.one * START_SCALE;

        Sequence seq = DOTween.Sequence();
        seq.SetLink(newTextObj);
        if (_delay > 0) seq.PrependInterval(_delay);
        seq.Append(rect.DOScale(Vector3.one * TARGET_SCALE, SCALE_DURATION).SetEase(Ease.OutBack));

        Vector3 endPos = rect.position + new Vector3(direction * SIDE_SPREAD, END_Y_OFFSET, 0f);
        seq.Join(rect.DOJump(endPos, JUMP_POWER, 1, DURATION).SetEase(Ease.OutQuad));
        seq.Insert(DURATION * FADE_START_PERCENT, text.DOFade(0f, DURATION * FADE_DURATION_PERCENT));

        seq.OnComplete(() => {
            if (newTextObj != null) Object.Destroy(newTextObj);
        });
    }
}