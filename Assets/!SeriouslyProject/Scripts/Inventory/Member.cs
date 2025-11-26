using UnityEngine;
using System;

/// <summary>
/// Представляет члена команды, хранит его основные характеристики.
/// </summary>
public class Member : MonoBehaviour
{
    [Header("Характеристики")]
    [SerializeField] private int _health = 100;
    [SerializeField] private int _mana = 50;
    [SerializeField] private int _level = 1;
    [SerializeField] private int _xp = 0;

    [Header("Максимумы")]
    [SerializeField] private int _maxHealth = 100;
    [SerializeField] private int _maxMana = 50;

    public event Action OnStatsChanged;

    public int Health => _health;
    public int Mana => _mana;
    public int Level => _level;
    public int XP => _xp;
    public int MaxHealth => _maxHealth;
    public int MaxMana => _maxMana;

    /// <summary>
    /// Устанавливает здоровье члена команды в пределах от 0 до максимума.
    /// </summary>
    public void SetHealth(int value)
    {
        _health = Mathf.Clamp(value, 0, _maxHealth);
        OnStatsChanged?.Invoke();
    }

    /// <summary>
    /// Устанавливает ману члена команды в пределах от 0 до максимума.
    /// </summary>
    public void SetMana(int value)
    {
        _mana = Mathf.Clamp(value, 0, _maxMana);
        OnStatsChanged?.Invoke();
    }
    
    // Здесь можно добавить другие методы для изменения характеристик, например, AddXP, SetLevel и т.д.
}
