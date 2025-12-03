using UnityEngine;
using Sirenix.OdinInspector;
using System;

[Serializable]
public class EntityStats : IData
{
    [BoxGroup("General Info")][SerializeField] protected string _name;
    [BoxGroup("General Info")][TextArea][SerializeField] protected string _description;
    [BoxGroup("General Info")][SerializeField] protected Sprite _sprite;

    [BoxGroup("Stats")][SerializeField] protected int _damage;
    [BoxGroup("Stats")][SerializeField] protected int _priority;
    [BoxGroup("Stats")][SerializeField] protected int _maxMana;
    [BoxGroup("Stats")][SerializeField] protected int _mana;
    [BoxGroup("Stats")][SerializeField] protected int _maxHealth;
    [BoxGroup("Stats")][SerializeField] protected int _health;
    [BoxGroup("Stats")][SerializeField] protected int _heal;
    [BoxGroup("Stats")][SerializeField] protected int _armor;
    [BoxGroup("Stats")][SerializeField] protected int _lucky;
    [BoxGroup("Stats")][SerializeField] protected int _creteChance;

    [BoxGroup("Progression")][SerializeField] protected int _level = 1;
    [BoxGroup("Progression")][SerializeField] protected int _currentXP;
    [BoxGroup("Progression")][SerializeField] protected int _maxXP = 100;
    [BoxGroup("Rewards")][SerializeField] protected int _xpReward = 30;

    [BoxGroup("Growth")][SerializeField] protected int _damagePerLevel = 1;
    [BoxGroup("Growth")][SerializeField] protected int _maxHealthPerLevel = 1;
    [BoxGroup("Growth")][SerializeField] protected int _healPerLevel = 1;
    [BoxGroup("Growth")][SerializeField] protected int _armorPerLevel = 1;
    [BoxGroup("Growth")][SerializeField] protected int _maxManaPerLevel = 1;
    [BoxGroup("Growth")][SerializeField] protected int _xpRewardPerLevel = 1;

    public virtual string Name { get => _name; set => _name = value; }
    public virtual string Description { get => _description; set => _description = value; }
    public virtual Sprite Sprite { get => _sprite; set => _sprite = value; }

    public virtual int Damage { get => _damage; set => _damage = value; }
    public virtual int Priority { get => _priority; set => _priority = value; }
    public virtual int MaxMana { get => _maxMana; set => _maxMana = value; }
    public virtual int Mana { get => _mana; set => _mana = value; }
    public virtual int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public virtual int Health { get => _health; set => _health = value; }
    public virtual int Heal { get => _heal; set => _heal = value; }
    public virtual int Armor { get => _armor; set => _armor = value; }
    public virtual int Lucky { get => _lucky; set => _lucky = value; }
    public virtual int CreteChance { get => _creteChance; set => _creteChance = value; }

    public virtual int Level { get => _level; set => _level = value; }
    public virtual int CurrentXP { get => _currentXP; set => _currentXP = value; }
    public virtual int MaxXP { get => _maxXP; set => _maxXP = value; }
    public virtual int XpReward { get => _xpReward; set => _xpReward = value; }

    public virtual int DamagePerLevel { get => _damagePerLevel; set => _damagePerLevel = value; }
    public virtual int MaxHealthPerLevel { get => _maxHealthPerLevel; set => _maxHealthPerLevel = value; }
    public virtual int HealPerLevel { get => _healPerLevel; set => _healPerLevel = value; }
    public virtual int ArmorPerLevel { get => _armorPerLevel; set => _armorPerLevel = value; }
    public virtual int MaxManaPerLevel { get => _maxManaPerLevel; set => _maxManaPerLevel = value; }
    public virtual int XpRewardPerLevel { get => _xpRewardPerLevel; set => _xpRewardPerLevel = value; }

    public virtual void RecalculateStats()
    {
        _damage = _damagePerLevel * _level;
        _maxHealth = _maxHealthPerLevel * _level;
        _heal = _healPerLevel * _level;
        _armor = _armorPerLevel * _level;
        _maxMana = _maxManaPerLevel * _level;
        _xpReward = _xpRewardPerLevel * _level;

        if (!Application.isPlaying)
        {
            _health = _maxHealth;
            _mana = _maxMana;
        }
    }
}