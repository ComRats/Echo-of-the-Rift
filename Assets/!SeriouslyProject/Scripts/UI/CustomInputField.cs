using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// TMP_InputField без выделения слова по двойному клику и без Ctrl+Backspace.
/// Замени стандартный TMP_InputField на этот компонент в инспекторе.
/// </summary>
public class CustomInputField : TMP_InputField
{
    private bool _isDragging;

    // Двойной клик — просто ставим каретку, без выделения слова
    public override void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.clickCount >= 2)
        {
            ActivateInputField();
            return;
        }
        base.OnPointerClick(eventData);
    }

    public override void OnBeginDrag(PointerEventData eventData)
    {
        _isDragging = true;
        base.OnBeginDrag(eventData);
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        _isDragging = false;
        base.OnEndDrag(eventData);
    }

    protected override void LateUpdate()
    {
        base.LateUpdate();

        // Сбрасываем выделение если оно появилось не от drag-а мышью
        if (isFocused && !_isDragging && !Input.GetMouseButton(0))
        {
            if (selectionAnchorPosition != selectionFocusPosition)
            {
                int pos = caretPosition;
                selectionAnchorPosition = pos;
                selectionFocusPosition = pos;
            }
        }
    }


}
