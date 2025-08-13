using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Apperance : MonoBehaviour, IUpdatableUI
{
    [SerializeField] private TextMeshProUGUI descriptionText;
    public TextMeshProUGUI DescriptionText => descriptionText;

    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private List<ApperanceProperty> appProperty;

    private int currentValue = 0;
    private RectTransform lastParent;

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
            if (newValue < appProperty.Count)
            {
                currentValue = newValue;
                UpdateUI();
            }
        }

        else if (step < 0 && currentValue > 0)
        {
            currentValue = newValue;
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        valueText.text = currentValue.ToString();
        descriptionText.text = appProperty[currentValue].description;

        if (lastParent != null)
        {
            for (int i = lastParent.childCount - 1; i >= 0; i--)
            {
                Destroy(lastParent.GetChild(i).gameObject);
            }
        }

        RectTransform parent = appProperty[currentValue].transform;
        lastParent = parent;

        if (parent != null)
        {

            for (int i = parent.childCount - 1; i >= 0; i--)
            {
                Destroy(parent.GetChild(i).gameObject);
            }

            GameObject newObj = new GameObject(appProperty[currentValue].sprite.name);
            newObj.transform.SetParent(parent, false);

            Image img = newObj.AddComponent<Image>();
            img.sprite = appProperty[currentValue].sprite;
        }
    }


    [Serializable]
    private struct ApperanceProperty
    {
        public Sprite sprite;
        public RectTransform transform;
        public string description;
    }
}
