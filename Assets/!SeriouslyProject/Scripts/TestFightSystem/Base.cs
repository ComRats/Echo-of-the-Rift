using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour
{
    [SerializeField] private StateEffect stateEffect;
    [SerializeField] private float blinkDelaySeconds = 0.5f;

    [Header("HealthBar")]
    public TextMeshProUGUI healthText;
    public Slider healthBar;
    public Gradient healthGgradient;
    public Image healthFill;
    public TextMeshProUGUI manaText;
    public Slider manaBar;
    public Gradient manaGgradient;
    public Image manaFill;

    [Header("TextPrefab")]
    public GameObject textPrefab;

    public bool IsBlinking { get; set; } = true;

    public string Name { get; set; }
    public string Description { get; set; }

    public Image Sprite { get; set; }

    public int Level { get; set; } = 1;
    public int currentXP { get; set; } = 0;
    public int MaxXP { get; set; } = 100;
    public int XpReward { get; set; } = 30;

    public int Damage { get; set; }
    public int MaxHealth { get; set; }
    public int Health { get; set; }
    public int MaxMana { get; set; }
    public int Mana { get; set; }
    public int Heal { get; set; }
    public int Priority { get; set; }
    public int Armor { get; set; } = 1;

    private IData data;

    public void Inizialize(IData data, GameObject gameObj)
    {
        this.data = data;

        Name = gameObj.name = data.Name;
        Sprite.sprite = data.Sprite;
        Description = data.Description;
        Damage = data.Damage;
        MaxHealth = data.MaxHealth;
        Health = data.Health;
        MaxMana = data.MaxMana;
        Mana = data.Mana;
        Heal = data.Heal;
        Priority = data.Priority;
        Armor = data.Armor;
        XpReward = data.XpReward;
    }

    public void TakeDamage(int _damage)
    {
        int currentDamage = _damage - Armor;
        if (currentDamage > 0)
            Health -= currentDamage;
        else currentDamage = 0;

        FightAnimation.ShowText(textPrefab, currentDamage, gameObject.transform, Color.red);
        UpdateUI();
        TryDeath();
    }

    public void TakeMagicDamage(int _damage)
    {
        if (_damage > 0)
            Health -= _damage;
        else _damage = 0;

        FightAnimation.ShowText(textPrefab, _damage, gameObject.transform, Color.blue);

        UpdateUI();
        TryDeath();
    }

    public void UpdateUI()
    {
        healthText.text = $"{Health}/{MaxHealth}";
        healthBar.minValue = 0;
        healthBar.maxValue = MaxHealth;
        healthBar.value = Health;

        manaText.text = Mana + " / " + MaxMana;
        manaBar.minValue = 0;
        manaBar.maxValue = MaxMana;
        manaBar.value = Mana;

        SetGradient(healthGgradient, healthFill, healthBar.normalizedValue);
        SetGradient(manaGgradient, manaFill, manaBar.normalizedValue);
    }

    internal void SetGradient(Gradient _gradient, Image _fill, float _value)
    {
        _fill.color = _gradient.Evaluate(_value);
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
            FightAnimation.ShowText(textPrefab, _heal, gameObject.transform, Color.green);
            Health += _heal;
            UpdateUI();
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

    public void GetXP(int _getXP)
    {
        FightAnimation.ShowText(textPrefab, "+" + _getXP.ToString(), gameObject.transform, Color.magenta, 1f);

        currentXP += _getXP;

        UpdateLevel();
    }

    private void UpdateLevel()
    {
        if (currentXP >= MaxXP)
        {
            FightAnimation.ShowText(textPrefab, "Новый уровень", gameObject.transform, Color.grey, 1.25f);

            Damage = data.DamagePerLevel * Level;
            MaxHealth = data.MaxHealthPerLevel * Level;
            Heal = data.HealPerLevel * Level;
            Armor = data.ArmorPerLevel * Level;
            MaxMana = data.MaxManaPerLevel * Level;
            XpReward = data.XpRewardPerLevel * Level;

            Health = MaxHealth;
            Mana = MaxMana;

            UpdateUI();
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
