using TMPro;
using UnityEditor;
using UnityEngine;

/// Выдели GameObject с TMP_InputField → правая кнопка → Replace with CustomInputField
public class ReplaceInputField
{
    [MenuItem("CONTEXT/TMP_InputField/Replace with CustomInputField")]
    static void Replace(MenuCommand command)
    {
        var old = (TMP_InputField)command.context;
        var go = old.gameObject;

        // Сохраняем все нужные ссылки
        var textViewport   = old.textViewport;
        var textComponent  = old.textComponent;
        var placeholder    = old.placeholder;
        var caretWidth     = old.caretWidth;
        var caretColor     = old.caretColor;
        var selectionColor = old.selectionColor;
        var charLimit      = old.characterLimit;
        var contentType    = old.contentType;
        var lineType       = old.lineType;
        var fontSize       = old.pointSize;
        var text           = old.text;

        Undo.DestroyObjectImmediate(old);

        var next = Undo.AddComponent<CustomInputField>(go);
        next.textViewport   = textViewport;
        next.textComponent  = textComponent;
        next.placeholder    = placeholder;
        next.caretWidth     = caretWidth;
        next.caretColor     = caretColor;
        next.selectionColor = selectionColor;
        next.characterLimit = charLimit;
        next.contentType    = contentType;
        next.lineType       = lineType;
        next.pointSize      = fontSize;
        next.text           = text;

        EditorUtility.SetDirty(go);
        Debug.Log($"[ReplaceInputField] Заменён на CustomInputField на {go.name}");
    }
}
