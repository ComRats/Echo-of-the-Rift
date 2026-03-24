using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SliderValuePresenter : MonoBehaviour
{
    [Header("Настройки текста")]
    [SerializeField] private Slider _slider;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private string format = "F1";
    [SerializeField] private string suffix = "";

    private void Awake()
    {
        UpdateLabel(_slider.value);
    }

    public void UpdateLabel(float value)
    {
        if (valueText == null) return;

        if (format == "P0")
        {
            valueText.text = (value * 100f).ToString("F0") + suffix;
        }
        else
        {
            valueText.text = value.ToString(format) + suffix;
        }
    }
}