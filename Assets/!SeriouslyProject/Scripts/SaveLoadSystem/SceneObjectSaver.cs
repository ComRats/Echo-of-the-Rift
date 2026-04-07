using UnityEngine;
using PixelCrushers.DialogueSystem;

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
        ApplyState();
    }

    public void ApplyState()
    {
        if (string.IsNullOrEmpty(dialogueVariable)) return;

        bool value = DialogueLua.GetVariable(dialogueVariable).asBool;
        bool shouldBeActive = hideWhenTrue ? !value : value;

        if (disableGameObject)
            gameObject.SetActive(shouldBeActive);
    }

    public void SetVariableAndApply(bool value)
    {
        if (string.IsNullOrEmpty(dialogueVariable)) return;
        DialogueLua.SetVariable(dialogueVariable, value);
        ApplyState();
    }
}
