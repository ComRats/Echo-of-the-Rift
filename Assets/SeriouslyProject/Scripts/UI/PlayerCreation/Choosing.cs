using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Choosing : MonoBehaviour
{
    [SerializeField] private Button leftButton; 
    [SerializeField] private Button rightButton;
    [SerializeField] private TextMeshProUGUI valueText;
    [SerializeField] private List<Stats> stats;

    private int value;
    private int Value
    {
        get => value;
        set
        {
            this.value = value;
            valueText.text = this.value.ToString();
        }
    }

    private void Start()
    {
        leftButton.onClick.AddListener(() => ChangeValue(-1));
        rightButton.onClick.AddListener(() => ChangeValue(1));

        Value = 0;
    }

    private void ChangeValue(int step)
    {
        int newValue = Value + step;
        if (newValue >= 0 && newValue <= stats.Count)
        {
            Value = newValue;
        }
    }

    [Serializable]
    private struct Stats
    {
        public TextMeshProUGUI statsText;
        public Sprite sprite;
    }
}
