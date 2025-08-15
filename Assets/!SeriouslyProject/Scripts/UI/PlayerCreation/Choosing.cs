using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Choosing : MonoBehaviour, IUpdatableUI
{
    public TextMeshProUGUI descriptionText;
    public TextMeshProUGUI DescriptionText => descriptionText;

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private PointsManager pointsManager;
    [SerializeField] private List<string> descriptions;

    private int currentValue = 0;

    private void Start()
    {
        leftButton.onClick.AddListener(() => ChangeValue(-1));
        rightButton.onClick.AddListener(() => ChangeValue(1));
    }

    private void ChangeValue(int step)
    {
        int newValue = currentValue + step;

        if (step > 0)
        {
            if (pointsManager.usedPoints < pointsManager.maxPoints && newValue < descriptions.Count)
            {
                pointsManager.usedPoints++;
                currentValue = newValue;
                UpdateUI();
            }
        }

        else if (step < 0 && currentValue > 0)
        {
            pointsManager.usedPoints--;
            currentValue = newValue;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        valueText.text = currentValue.ToString();
        descriptionText.text = descriptions[currentValue];
    }
}
