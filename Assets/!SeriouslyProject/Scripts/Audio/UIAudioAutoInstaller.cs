using UnityEngine;
using UnityEngine.UI;
using AudioManager.Locator;
using AudioManager.Core;
using System.Collections;

public class UIAudioAutoInstaller : MonoBehaviour
{
    [SerializeField] private string clickSoundName = "ClickSound";
    private bool isAudioManagerReady = false;

    private void Start()
    {
        StartCoroutine(InitializeWithDelay());
    }

    private IEnumerator InitializeWithDelay()
    {
        // Ждем один кадр, чтобы все Awake() методы успели выполниться
        yield return null;
        
        // Проверяем, что AudioManager инициализирован и звук зарегистрирован
        var audioManager = ServiceLocator.GetService();
        if (audioManager != null)
        {
            // Проверяем, что звук существует
            if (audioManager.TryGetSource(clickSoundName, out _) == AudioError.OK)
            {
                isAudioManagerReady = true;
            }
            else
            {
                Debug.LogWarning($"[UIAudioAutoInstaller] Sound '{clickSoundName}' not registered yet. Buttons will be silent.");
            }
        }

        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (var btn in buttons)
        {
            btn.onClick.AddListener(() => PlaySound());
        }
    }

    private void PlaySound()
    {
        if (!isAudioManagerReady) return;
        
        ServiceLocator.GetService().Play(clickSoundName, ChildType.PARENT);
    }
}