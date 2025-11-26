using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Обновляет UI элементы для <see cref="Member"/> на основе его характеристик.
/// </summary>
[RequireComponent(typeof(Member))]
public class MemberUI : MonoBehaviour
{
    [Header("UI Элементы")]
    [SerializeField] private Slider _healthSlider;
    [SerializeField] private Slider _manaSlider;
    [SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _healthText;
    [SerializeField] private TextMeshProUGUI _levelText;
    [SerializeField] private TextMeshProUGUI _manaText;
    
    private Member _member;

    private void Awake()
    {
        _member = GetComponent<Member>();
    }

    private void OnEnable()
    {
        _member.OnStatsChanged += UpdateUI;
        UpdateUI();
    }

    private void OnDisable()
    {
        _member.OnStatsChanged -= UpdateUI;
    }

    /// <summary>
    /// Обновляет все UI элементы в соответствии с текущими характеристиками.
    /// </summary>
    private void UpdateUI()
    {
        if (_healthSlider != null)
        {
            _healthSlider.maxValue = _member.MaxHealth;
            _healthSlider.value = _member.Health;
        }
        if (_manaSlider != null)
        {
            _manaSlider.maxValue = _member.MaxMana;
            _manaSlider.value = _member.Mana;
        }
        if (_healthText != null)
        {
            _healthText.text = $"{_member.Health} / {_member.MaxHealth}";
        }
        if (_manaText != null)
        {
            _manaText.text = $"{_member.Mana} / {_member.MaxMana}";
        }
        if (_levelText != null)
        {
            _levelText.text = $"Lvl {_member.Level}";
        }
    }
}