using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using VInspector.Libs;

public class Base : MonoBehaviour
{
    [SerializeField] StateEffect stateEffect;
    [SerializeField] float blinkDelaySeconds = 0.5f;

    [Header("HealthBar")]
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    public Slider healthBar;
    public Slider manaBar;
    public Gradient healthGgradient;
    public Image fill;

    public bool IsBlinking { get; set; } = true;

    public string Name { get; set; }
    public string Description { get; set; }

    public Image Sprite { get; set; }

    public int Level { get; set; } = 1;
    public int currentXP { get; set; } = 0;
    public int MaxXP { get; set; } = 100;
    public int XPreward { get; set; } = 30;

    public int Damage { get; set; }
    public int MaxHealth { get; set; }
    public int Health { get; set; }
    public int MaxMana { get; set; }
    public int Mana { get; set; }
    public int Heal { get; set; }
    public int Priority { get; set; }
    public int Armor { get; set; } = 1;

    public void TakeDamage(int _damage)
    {
        Health -= _damage - (Armor);
        healthBar.value = Health;
        healthText.text = Health.ToString() + " / " + MaxHealth;
        SetGradient(healthBar.normalizedValue);
        TryDeath();
    }

    public void TakeMagicDamage(int _damage)
    {
        Mana -= _damage - (_damage / 100);
        manaBar.value = Mana;
        manaText.text = Mana.ToString() + " / " + MaxMana;
        SetGradient(manaBar.normalizedValue);
        TryDeath();
    }

    internal void SetGradient(float _value)
    {
        fill.color = healthGgradient.Evaluate(_value);
    }

    public int GiveHeal()
    {
        return Heal;
    }

    public int GiveDamage()
    {
        return Damage;
    }

    public void TryDeath()
    {
        if (Health <= 0)
        {
            Health = 0;
            healthText.text = Health.ToString() + " / " + MaxHealth;
            Debug.Log(Name + ": Was Killed");
        }
    }

    public void TakeHeal(int _heal)
    {
        if (Health < MaxHealth)
        {
            Health += _heal;
            healthText.text = Health.ToString() + " / " + MaxHealth;
            healthBar.value = Health;
            SetGradient(healthBar.normalizedValue);
        }
        else Health = MaxHealth;
    }

    public IEnumerator Blinking()
    {
        IsBlinking = true;
        Color color = Sprite.color;

        while (IsBlinking)
        {
            for (float i = 0; i < blinkDelaySeconds; i += Time.deltaTime)
            {
                color.a = Mathf.Lerp(1f, 0.5f, i / blinkDelaySeconds);
                Sprite.color = color;
                yield return null;
            }

            for (float i = 0; i < blinkDelaySeconds; i += Time.deltaTime)
            {
                color.a = Mathf.Lerp(0.5f, 1f, i / blinkDelaySeconds);
                Sprite.color = color;
                yield return null;
            }
        }
    }

    public enum StateEffect
    {
        None,
        Fire,
        Water,
        Air,
        Ground
    }
}
