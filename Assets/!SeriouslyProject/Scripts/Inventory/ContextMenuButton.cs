using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ContextMenuButton : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI buttonText;
    [SerializeField] private Button button;
    
    private System.Action onClickAction;
    
    private void Awake()
    {
        if (button == null)
        {
            button = GetComponent<Button>();
        }
        
        if (button != null)
        {
            button.onClick.AddListener(OnButtonClick);
        }
    }
    
    public void Initialize(string text, System.Action onClick)
    {
        if (buttonText != null)
        {
            buttonText.text = text;
        }
        
        onClickAction = onClick;
    }
    
    private void OnButtonClick()
    {
        onClickAction?.Invoke();
    }
    
    private void OnDestroy()
    {
        if (button != null)
        {
            button.onClick.RemoveListener(OnButtonClick);
        }
    }
}