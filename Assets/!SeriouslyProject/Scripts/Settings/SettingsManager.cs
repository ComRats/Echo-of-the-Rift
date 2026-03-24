using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameSettingsUI gameSettingsUI;
    [SerializeField] private AudioSettingsUI audioSettingsUI;
    [SerializeField] private KeyBindingsSettingsUI keyBindingsSettingsUI;

    public void SaveSettings()
    {
        gameSettingsUI.SaveCurrentState();
        audioSettingsUI.SaveCurrentState();
        keyBindingsSettingsUI.SaveCurrentState();
    }
}
