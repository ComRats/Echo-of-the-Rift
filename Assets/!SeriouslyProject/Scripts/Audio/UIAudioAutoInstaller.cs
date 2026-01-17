using UnityEngine;
using UnityEngine.UI;
using AudioManager.Locator;
using AudioManager.Core;

public class UIAudioAutoInstaller : MonoBehaviour
{
    [SerializeField] private string clickSoundName = "ClickSound";

    private void Start()
    {
        Button[] buttons = GetComponentsInChildren<Button>(true);

        foreach (var btn in buttons)
        {
            btn.onClick.AddListener(() => PlaySound());
        }
    }

    private void PlaySound()
    {
        ServiceLocator.GetService().Play(clickSoundName, ChildType.PARENT);
    }
}