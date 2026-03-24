using UnityEngine;

public class SettingsManager : MonoBehaviour
{
    [SerializeField] private GameSettingsUI gameSettingsUI;
    [SerializeField] private AudioSettingsUI audioSettingsUI;

    public void SaveSettings()
    {
        gameSettingsUI.SaveCurrentState();
        audioSettingsUI.SaveCurrentState();
    }
}
