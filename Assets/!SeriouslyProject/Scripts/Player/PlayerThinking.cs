using PixelCrushers.DialogueSystem;
using TMPro;
using UnityEngine;
using System.Collections;

public class PlayerThinking : MonoBehaviour
{
    [SerializeField] private TextMeshPro text;
    [SerializeField] private TextMeshProTypewriterEffect typewriter;
    [SerializeField] private float eraseSpeed = 0.03f;

    private Coroutine eraseCoroutine;

    private bool originalAutoSize;

    private void Awake()
    {
        originalAutoSize = text.enableAutoSizing;
    }

    public void SetThink(string textToShow)
    {
        typewriter.Stop();
        if (eraseCoroutine != null)
        {
            StopCoroutine(eraseCoroutine);
            eraseCoroutine = null;
        }

        if (string.IsNullOrEmpty(textToShow))
        {
            text.enableAutoSizing = false;

            eraseCoroutine = StartCoroutine(EraseText());
            return;
        }

        text.enableAutoSizing = originalAutoSize;

        text.text = textToShow;
        typewriter.StartTyping(textToShow);
    }

    private IEnumerator EraseText()
    {
        while (text.text.Length > 0)
        {
            text.text = text.text.Substring(0, text.text.Length - 1);
            yield return new WaitForSeconds(eraseSpeed);
        }

        text.text = "";

        text.enableAutoSizing = originalAutoSize;
    }
}