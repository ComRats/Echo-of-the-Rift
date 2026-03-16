using UnityEngine;
using PixelCrushers.DialogueSystem;

/// <summary>
/// Привязывает активность GameObject к переменной Dialogue System.
/// Добавь на NPC или объект сцены, укажи имя переменной и условие.
/// 
/// Пример: переменная "Quest_Merchant_Done" = true → объект скрывается.
/// </summary>
public class SceneObjectSaver : MonoBehaviour
{
    [Tooltip("Имя переменной в Dialogue System (Variable['...'])")]
    [SerializeField] private string dialogueVariable;

    [Tooltip("При каком значении переменной объект должен быть СКРЫТ")]
    [SerializeField] private bool hideWhenTrue = true;

    [Tooltip("Скрывать весь GameObject или только отключать компоненты")]
    [SerializeField] private bool disableGameObject = true;

    private void Start()
    {
        ApplyState();
    }

    private void OnEnable()
    {
        // Обновляем состояние при каждой загрузке сцены
        ApplyState();
    }

    /// <summary>
    /// Читает переменную из Dialogue System и применяет состояние объекта.
    /// </summary>
    public void ApplyState()
    {
        if (string.IsNullOrEmpty(dialogueVariable)) return;

        bool value = DialogueLua.GetVariable(dialogueVariable).asBool;
        bool shouldBeActive = hideWhenTrue ? !value : value;

        if (disableGameObject)
            gameObject.SetActive(shouldBeActive);
    }

    /// <summary>
    /// Устанавливает переменную и сразу применяет состояние.
    /// Вызывай из Sequencer: SendMessage(SetAndApply) или напрямую из кода.
    /// </summary>
    public void SetVariableAndApply(bool value)
    {
        if (string.IsNullOrEmpty(dialogueVariable)) return;
        DialogueLua.SetVariable(dialogueVariable, value);
        ApplyState();
    }
}
