using Sirenix.OdinInspector;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Base : MonoBehaviour, IData
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

    [HideLabel]
    [InlineProperty]
    [SerializeField]
    private EntityStats stats = new EntityStats();

    public bool IsBlinking { get; set; } = true;

    public string Name { get => stats.Name; set => stats.Name = value; }
    public string Description { get => stats.Description; set => stats.Description = value; }
    public Image Sprite { get; set; }
    public int Damage { get => stats.Damage; set => stats.Damage = value; }
    public int Priority { get => stats.Priority; set => stats.Priority = value; }
    public int MaxMana { get => stats.MaxMana; set => stats.MaxMana = value; }
    public int Mana { get => stats.Mana; set => stats.Mana = value; }
    public int MaxHealth { get => stats.MaxHealth; set => stats.MaxHealth = value; }
    public int Health { get => stats.Health; set => stats.Health = value; }
    public int Heal { get => stats.Heal; set => stats.Heal = value; }
    public int Armor { get => stats.Armor; set => stats.Armor = value; }
    public int Lucky { get => stats.Lucky; set => stats.Lucky = value; }
    public int CreteChance { get => stats.CreteChance; set => stats.CreteChance = value; }
    public int Level { get => stats.Level; set => stats.Level = value; }
    public int CurrentXP { get => stats.CurrentXP; set => stats.CurrentXP = value; }
    public int MaxXP { get => stats.MaxXP; set => stats.MaxXP = value; }
    public int XpReward { get => stats.XpReward; set => stats.XpReward = value; }

    public int DamagePerLevel { get => stats.DamagePerLevel; set => stats.DamagePerLevel = value; }
    public int MaxHealthPerLevel { get => stats.MaxHealthPerLevel; set => stats.MaxHealthPerLevel = value; }
    public int HealPerLevel { get => stats.HealPerLevel; set => stats.HealPerLevel = value; }
    public int ArmorPerLevel { get => stats.ArmorPerLevel; set => stats.ArmorPerLevel = value; }
    public int MaxManaPerLevel { get => stats.MaxManaPerLevel; set => stats.MaxManaPerLevel = value; }
    public int XpRewardPerLevel { get => stats.XpRewardPerLevel; set => stats.XpRewardPerLevel = value; }
    Sprite IData.Sprite { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private void OnValidate() => stats.RecalculateStats();

    private IData data;

    public void Initialize(IData data, GameObject gameObj)
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
        Lucky = data.Lucky;
        CreteChance = data.CreteChance;
        XpReward = data.XpReward;
        Level = data.Level;
        CurrentXP = data.CurrentXP;
        MaxXP = data.MaxXP;
        DamagePerLevel = data.DamagePerLevel;
        MaxHealthPerLevel = data.MaxHealthPerLevel;
        HealPerLevel = data.HealPerLevel;
        ArmorPerLevel = data.ArmorPerLevel;
        MaxManaPerLevel = data.MaxManaPerLevel;
        XpRewardPerLevel = data.XpRewardPerLevel;
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

        CurrentXP += _getXP;

        UpdateLevel();
    }

    private void UpdateLevel()
    {
        if (CurrentXP >= MaxXP)
        {
            FightAnimation.ShowText(textPrefab, "Новый уровень", gameObject.transform, Color.grey, 1.25f);

            Level++;

            CurrentXP -= MaxXP;
            Damage += data.DamagePerLevel * Level;
            MaxHealth += data.MaxHealthPerLevel * Level;
            Heal += data.HealPerLevel * Level;
            Armor += data.ArmorPerLevel * Level;
            MaxMana += data.MaxManaPerLevel * Level;
            XpReward += data.XpRewardPerLevel * Level;
            MaxXP += data.MaxXP * Level;

            Health = MaxHealth;
            Mana = MaxMana;

            UpdateUI();
            
            GlobalLoader.Instance.SavePlayer();

            UpdateLevel();
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
