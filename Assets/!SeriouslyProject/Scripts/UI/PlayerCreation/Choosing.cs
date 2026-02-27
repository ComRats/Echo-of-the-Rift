using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Choosing : MonoBehaviour, IUpdatableUI
{
    [SerializeField] private StatType statType;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private PointsManager pointsManager;
    [SerializeField] private int maxStatValue = 5;

    public TextMeshProUGUI DescriptionText => descriptionText;
    public int currentValue { get; private set; }

    private void Start()
    {
        leftButton.onClick.AddListener(() => ChangeValue(-1)); 
        rightButton.onClick.AddListener(() => ChangeValue(1));
        UpdateUI();
    }

    private void ChangeValue(int step)
    {
        if (step > 0 
            && currentValue < maxStatValue 
            && pointsManager.CanAddPoint())
        {
            currentValue++;
            pointsManager.AddPoint();
        }
        else if (step < 0 && currentValue > 0)
        {
            currentValue--;
            pointsManager.RemovePoint();
        }

        UpdateUI();
    }

    public void UpdateUI()
    {
        valueText.text = currentValue.ToString();
        DescriptionText.text =
            pointsManager.GetDescription(statType, currentValue);
    }
}