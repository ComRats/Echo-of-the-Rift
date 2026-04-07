using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Zenject;
using EchoRift.EchoRiftSaveLoadSystem;
using static EchoRift.EchoRiftSaveLoadSystem.SaveFileNames;

public class KeyBindingsSettingsUI : MonoBehaviour
{
    [SerializeField] private Button inventoryKeyButton;
    [SerializeField] private Button pauseMenuKeyButton;
    [SerializeField] private Button useKeyButton;
    [SerializeField] private Button questWindowKeyButton;

    [Inject]
    private GameSettings _settings;
     
    private Button _listeningButton;
    private System.Action<KeyCode> _onKeySelected;
    private System.Func<KeyCode, bool> _keyFilter;

    private void Start()
    {
        RefreshAllSettings();
    }

    public void RefreshAllSettings()
    {
        var data = SaveLoadSystem.Load<GlobalSettingsData>(SETTINGS) ?? new GlobalSettingsData();

        _settings.openInvenoryKey = data.openInventoryKey;
        _settings.openPauseMenuKey = data.openPauseMenuKey;
        _settings.useButton = data.useButton;
        _settings.questWindowKey = data.questWindowKey;

        Configure(inventoryKeyButton, _settings.openInvenoryKey, val => _settings.openInvenoryKey = val);
        Configure(pauseMenuKeyButton, _settings.openPauseMenuKey, val => _settings.openPauseMenuKey = val);
        Configure(useKeyButton, _settings.useButton, val => _settings.useButton = val, GameSettings.AllowedUseKeys);
        Configure(questWindowKeyButton, _settings.questWindowKey, val => _settings.questWindowKey = val);
    }

    private void Configure(Button button, KeyCode currentKey, System.Action<KeyCode> applyKey, KeyCode[] allowedKeys = null)
    {
        if (button == null) return;

        SetButtonLabel(button, currentKey);
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => StartListening(button, applyKey, allowedKeys));
    }

    private void StartListening(Button button, System.Action<KeyCode> applyKey, KeyCode[] allowedKeys = null)
    {
        if (_listeningButton != null)
            SetButtonLabel(_listeningButton, GetCurrentKeyForButton(_listeningButton));

        _listeningButton = button;
        _onKeySelected = applyKey;
        _keyFilter = allowedKeys != null
            ? key => System.Array.IndexOf(allowedKeys, key) >= 0
            : _ => true;

        SetButtonLabel(button, KeyCode.None, listening: true);
    }

    private void OnGUI()
    {
        if (_listeningButton == null) return;

        Event e = Event.current;
        if (e.type != EventType.KeyDown || e.keyCode == KeyCode.None) return;

        if (_keyFilter != null && !_keyFilter(e.keyCode))
            return;

        KeyCode currentForThis = GetCurrentKeyForButton(_listeningButton);
        foreach (var bound in _settings.AllBoundKeys)
        {
            if (bound == e.keyCode && bound != currentForThis)
            {
                SetButtonLabel(_listeningButton, currentForThis, conflict: true);
                return;
            }
        }

        _onKeySelected?.Invoke(e.keyCode);
        SetButtonLabel(_listeningButton, e.keyCode);
        _listeningButton = null;
        _onKeySelected = null;
        _keyFilter = null;

        SaveCurrentState();
    }

    private void SetButtonLabel(Button button, KeyCode key, bool listening = false, bool conflict = false)
    {
        var label = button.GetComponentInChildren<TMP_Text>();
        if (label == null) return;

        if (listening)
            label.text = "Нажмите клавишу";
        else if (conflict)
            label.text = "Уже занято!";
        else
            label.text = key.ToString();
    }

    private KeyCode GetCurrentKeyForButton(Button button)
    {
        if (button == inventoryKeyButton) return _settings.openInvenoryKey;
        if (button == pauseMenuKeyButton) return _settings.openPauseMenuKey;
        if (button == useKeyButton) return _settings.useButton;
        if (button == questWindowKeyButton) return _settings.questWindowKey;
        return KeyCode.None;
    }

    public void SaveCurrentState()
    {
        var data = SaveLoadSystem.Load<GlobalSettingsData>(SETTINGS) ?? new GlobalSettingsData();

        data.openInventoryKey = _settings.openInvenoryKey;
        data.openPauseMenuKey = _settings.openPauseMenuKey;
        data.useButton = _settings.useButton;
        data.questWindowKey = _settings.questWindowKey;

        SaveLoadSystem.Save(SETTINGS, data);
    }
}
