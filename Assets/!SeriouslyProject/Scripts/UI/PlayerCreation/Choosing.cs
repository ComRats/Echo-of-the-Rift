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
        int multiplier = 1;
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            multiplier = 5;
        }

        if (step > 0 && currentValue < maxStatValue && pointsManager.CanAddPoint())
        {
            int add = Mathf.Min(multiplier, maxStatValue - currentValue);
            add = Mathf.Min(add, pointsManager.maxPoints - pointsManager.usedPoints);
            currentValue += add;
            for (int i = 0; i < add; i++) pointsManager.AddPoint();
        }
        else if (step < 0 && currentValue > 0)
        {
            int remove = Mathf.Min(multiplier, currentValue);
            currentValue -= remove;
            for (int i = 0; i < remove; i++) pointsManager.RemovePoint();
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