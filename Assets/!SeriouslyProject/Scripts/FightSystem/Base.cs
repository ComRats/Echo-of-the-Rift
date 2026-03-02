using AudioManager.Core;
using AudioManager.Locator;
using FightSystem.Enemy;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using static GameColorsDataBase;

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

    [Header("AnimatorPrefab")]
    [SerializeField] private Animator animator;

    [HideLabel]
    [InlineProperty]
    [SerializeField]
    private EntityStats stats = new EntityStats();

    public List<ActiveStatusEffect> activeEffects = new List<ActiveStatusEffect>();
    public bool IsBlinking { get; set; } = true;

    public event System.Action<int, int> OnXPChanged;
    public event System.Action<int, int> OnHealthChanged;

    public string Name { get => stats.Name; set => stats.Name = value; }
    public string Description { get => stats.Description; set => stats.Description = value; }
    public Image Sprite { get; set; }
    public int Damage { get => stats.Damage; set => stats.Damage = value; }
    public int MagicDamage { get => stats.MagicDamage; set => stats.MagicDamage = value; }
    public int Priority { get => stats.Priority; set => stats.Priority = value; }
    public int MaxMana { get => stats.MaxMana; set => stats.MaxMana = value; }
    public int Mana { get => stats.Mana; set => stats.Mana = value; }
    public int MaxHealth { get => stats.MaxHealth; set => stats.MaxHealth = value; }
    public int Health { get => stats.Health; set => stats.Health = value; }
    public int Heal { get => stats.Heal; set => stats.Heal = value; }
    public int Armor { get => stats.Armor; set => stats.Armor = value; }
    public int Lucky { get => stats.Lucky; set => stats.Lucky = value; }
    public int CreteDamage { get => stats.CreteDamage; set => stats.CreteDamage = value; }
    public int Level { get => stats.Level; set => stats.Level = value; }
    public int CurrentXP { get => stats.CurrentXP; set => stats.CurrentXP = value; }
    public int MaxXP { get => stats.MaxXP; set => stats.MaxXP = value; }
    public int XpReward { get => stats.XpReward; set => stats.XpReward = value; }
    public string AttackAnimationName { get; set; }

    public int DamagePerLevel { get => stats.DamagePerLevel; set => stats.DamagePerLevel = value; }
    public int MaxHealthPerLevel { get => stats.MaxHealthPerLevel; set => stats.MaxHealthPerLevel = value; }
    public int HealPerLevel { get => stats.HealPerLevel; set => stats.HealPerLevel = value; }
    public int ArmorPerLevel { get => stats.ArmorPerLevel; set => stats.ArmorPerLevel = value; }
    public int MaxManaPerLevel { get => stats.MaxManaPerLevel; set => stats.MaxManaPerLevel = value; }
    public int XpRewardPerLevel { get => stats.XpRewardPerLevel; set => stats.XpRewardPerLevel = value; }
    Sprite IData.Sprite { get => throw new System.NotImplementedException(); set => throw new System.NotImplementedException(); }

    private void OnValidate() => stats.RecalculateStats();

    private IData data;
    private Transform textOffset;
    private IAudioManager service;

    private void Start()
    {
        service = ServiceLocator.GetService();
    }

    public void Initialize(IData data, GameObject gameObj)
    {
        this.data = data;

        Name = gameObj.name = data.Name;
        Sprite.sprite = data.Sprite;
        Description = data.Description;
        AttackAnimationName = data.AttackAnimationName;
        Damage = data.Damage;
        MagicDamage = data.MagicDamage;
        MaxHealth = data.MaxHealth;
        Health = data.Health;
        MaxMana = data.MaxMana;
        Mana = data.Mana;
        Heal = data.Heal;
        Priority = data.Priority;
        Armor = data.Armor;
        Lucky = data.Lucky;
        CreteDamage = data.CreteDamage;
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

    public void PlayAnimation(string triggerName)
    {
        animator.SetTrigger(triggerName);
    }

    public void SetAnimationSpeed(float speed)
    {
        if (animator != null)
        {
            animator.speed = speed;
        }
    }

    public void ApplyStatusEffect(StatusEffectSO effectData)
    {
        var existing = activeEffects.Find(e => e.data == effectData);
        if (existing != null)
        {
            existing.remainingTurns = effectData.duration;
        }
        else
        {
            activeEffects.Add(new ActiveStatusEffect(effectData));
        }
        Debug.Log($"{Name} ������� ������ {effectData.effectName}");
    }

    public void ProcessStatusEffects()
    {
        for (int i = activeEffects.Count - 1; i >= 0; i--)
        {
            var effect = activeEffects[i];

            if (effect.data.damagePerTurn != 0)
            {
                FightAnimation.ShowText(textPrefab, Mathf.Abs(effect.data.damagePerTurn), transform, effect.data.tickColor, new Vector3(30f, 20f, 0f));

                if (effect.data.damagePerTurn > 0)
                    Health -= effect.data.damagePerTurn;
                else
                    TakeHeal(Mathf.Abs(effect.data.damagePerTurn));

                UpdateUI();
                OnHealthChanged?.Invoke(Health, MaxHealth);
                TryDeath();
            }

            effect.remainingTurns--;

            if (effect.remainingTurns <= 0)
            {
                activeEffects.RemoveAt(i);
            }
        }
    }

    public void TakeDamage(int _damage)
    {
        if (Random.Range(0, 100) < 10)
        {
            FightAnimation.ShowText(textPrefab, "Промах", gameObject.transform, Color.gray);
            return;
        }

        int currentDamage = _damage - Armor;
        if (currentDamage > 0)
            Health -= currentDamage;
        else currentDamage = 0;

        FightAnimation.ShowText(textPrefab, currentDamage, gameObject.transform, PhysDamage);
        service.Play("MeleeDamage");
        UpdateUI();
        OnHealthChanged?.Invoke(Health, MaxHealth);
        TryDeath();
    }

    public void TakeMagicDamage(int _magicDamage)
    {
        if (Random.Range(0, 100) < 10)
        {
            FightAnimation.ShowText(textPrefab, "Промах", gameObject.transform, Color.gray);
            return;
        }

        if (_magicDamage > 0)
            Health -= _magicDamage;
        else _magicDamage = 0;

        FightAnimation.ShowText(textPrefab, _magicDamage, gameObject.transform, MageDamage);
        service.Play("MagicDamage");
        UpdateUI();
        OnHealthChanged?.Invoke(Health, MaxHealth);
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

    public virtual int GiveDamage()
    {
        int finalDamage = Damage;

        float critChance = Lucky * 0.5f;

        if (Random.Range(0, 100) <= critChance)
        {
            float rawRandom = Random.value;

            float powerFactor = Mathf.Lerp(4.0f, 1.0f, Mathf.Clamp01(Lucky / 100f));
            float weightedRandom = Mathf.Pow(rawRandom, powerFactor);

            float minBonus = 0.1f;
            float maxBonus = CreteDamage / 100f;

            float critBonusPercent = Mathf.Lerp(minBonus, maxBonus, weightedRandom);

            finalDamage = Mathf.RoundToInt(finalDamage * (1f + critBonusPercent));

            FightAnimation.ShowText(textPrefab, "КРИТ!", transform, CritDamage, 1.2f);
        }

        return finalDamage;
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
            OnHealthChanged?.Invoke(Health, MaxHealth);
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
        FightAnimation.ShowText(textPrefab, "+" + _getXP.ToString(), transform, Experience, new Vector3(-30f, 20f, 0f), 1f);

        CurrentXP += _getXP;
        OnXPChanged?.Invoke(CurrentXP, MaxXP);

        UpdateLevel();
    }

    private void UpdateLevel()
    {
        if (CurrentXP >= MaxXP)
        {
            FightAnimation.ShowText(textPrefab, "LEVEL UP!", transform, LevelUp);

            Level++;

            CurrentXP -= MaxXP;
            Damage += data.DamagePerLevel * Level;
            MaxHealth += data.MaxHealthPerLevel * Level;
            Heal += data.HealPerLevel * Level;
            Armor += data.ArmorPerLevel * Level;
            MaxMana += data.MaxManaPerLevel * Level;
            XpReward += data.XpRewardPerLevel * Level;
            MaxXP += data.MaxXP * Level;

            AnimateStatRestore();

            SaveCharacterProgress();

            OnXPChanged?.Invoke(CurrentXP, MaxXP);

            UpdateLevel();
        }
    }

    private void AnimateStatRestore()
    {
        int startHealth = Health;
        int startMana = Mana;
        int targetHealth = MaxHealth;
        int targetMana = MaxMana;

        float duration = 1.5f;

        DOTween.To(() => startHealth, x => 
        {
            Health = x;
            UpdateUI();
        }, targetHealth, duration)
        .SetEase(Ease.OutBack);

        DOTween.To(() => startMana, x => 
        {
            Mana = x;
            UpdateUI();
        }, targetMana, duration)
        .SetEase(Ease.OutQuad);
    }

    private void SaveCharacterProgress()
    {
        if (this is FightSystem.Character.Character character)
        {
            if (GlobalLoader.Instance != null && GlobalLoader.Instance.playerInstance != null)
            {
                var playerSaver = GlobalLoader.Instance.playerInstance.playerSaver;
                
                playerSaver.Level = Level;
                playerSaver.CurrentXP = CurrentXP;
                playerSaver.MaxXP = MaxXP;
                playerSaver.Damage = Damage;
                playerSaver.MaxHealth = MaxHealth;
                playerSaver.Health = Health;
                playerSaver.Heal = Heal;
                playerSaver.Armor = Armor;
                playerSaver.MaxMana = MaxMana;
                playerSaver.Mana = Mana;
                playerSaver.XpReward = XpReward;
                
                GlobalLoader.Instance.SavePlayer();
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
