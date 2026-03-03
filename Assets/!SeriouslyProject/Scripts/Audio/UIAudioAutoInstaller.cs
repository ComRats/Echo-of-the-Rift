using UnityEngine;
using UnityEngine.UI;
using AudioManager.Locator;
using AudioManager.Core;
using System.Collections;

public class UIAudioAutoInstaller : MonoBehaviour
{
    [SerializeField] private string clickSoundName = "ButtonClick1";

    private void Start()
    {
        StartCoroutine(InitializeWithDelay());
    }

    private IEnumerator InitializeWithDelay()
    {
        // Ждем несколько кадров, чтобы AudioManager точно инициализировался
        yield return new WaitForSeconds(0.1f);
        
        // Получаем AudioManager
        var audioManager = ServiceLocator.GetService();
        
        if (audioManager == null)
        {
            Debug.LogWarning($"[UIAudioAutoInstaller] AudioManager not found. Buttons will be silent.");
            yield break;
        }

        // Проверяем, что звук зарегистрирован
        if (audioManager.TryGetSource(clickSoundName, out _) != AudioError.OK)
        {
            Debug.LogWarning($"[UIAudioAutoInstaller] Sound '{clickSoundName}' not registered. Buttons will be silent.");
            yield break;
        }

        // Добавляем обработчики к кнопкам
        Button[] buttons = GetComponentsInChildren<Button>(true);
        foreach (var btn in buttons)
        {
            btn.onClick.AddListener(() => PlaySound());
        }
    }

    private void PlaySound()
    {
        var audioManager = ServiceLocator.GetService();
        if (audioManager != null)
        {
            audioManager.Play(clickSoundName, ChildType.PARENT);
        }
    }
}
