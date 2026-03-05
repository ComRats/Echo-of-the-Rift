using UnityEngine;
using Sirenix.OdinInspector;

namespace FightSystem.Data
{
    [CreateAssetMenu(fileName = "New Characters", menuName = "ScriptableObjects/TestCharacter")]
    public class CharacterData : ScriptableObject, IData
    {
        [BoxGroup("General Info")]
        public string _name;
        [TextArea]
        public string _description;
        public Sprite _sprite;

        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public Sprite Sprite { get => _sprite; set => _sprite = value; }

        [BoxGroup("Stats (Base)")]
        public int _damage;
        public int _magicDamage;
        public int _priority;
        public int _maxMana;
        public int _mana;
        public int _maxHealth;
        public int _health;
        public int _heal;
        public int _armor;
        public int _lucky;
        public int _creteChance;

        public int Damage { get => _damage; set => _damage = value; }
        public int MagicDamage { get => _magicDamage; set => _magicDamage = value; }
        public int Priority { get => _priority; set => _priority = value; }
        public int MaxMana { get => _maxMana; set => _maxMana = value; }
        public int Mana { get => _mana; set => _mana = value; }
        public int MaxHealth { get => _maxHealth; set => _maxHealth = value; }
        public int Health { get => _health; set => _health = value; }
        public int Heal { get => _heal; set => _heal = value; }
        public int Armor { get => _armor; set => _armor = value; }
        public int Lucky { get => _lucky; set => _lucky = value; }
        public int CreteDamage { get => _creteChance; set => _creteChance = value; }

        [BoxGroup("Progression")]
        [OnValueChanged("UpdateStats")]
        public int _level;
        public int _currentXP;
        public int _maxXP;

        public int Level { get => _level; set => _level = value; }
        public int CurrentXP { get => _currentXP; set => _currentXP = value; }
        public int MaxXP { get => _maxXP; set => _maxXP = value; }

        [BoxGroup("Rewards")]
        public int _xpReward;
        public int XpReward { get => _xpReward; set => _xpReward = value; }

        [BoxGroup("Per Level Growth")]
        public int _damagePerLevel = 1;
        public int _maxHealthPerLevel = 1;
        public int _healPerLevel = 1;
        public int _armorPerLevel = 1;
        public int _maxManaPerLevel = 1;
        public int _xpRewardPerLevel = 1;

        public int DamagePerLevel { get => _damagePerLevel; set => _damagePerLevel = value; }
        public int MaxHealthPerLevel { get => _maxHealthPerLevel; set => _maxHealthPerLevel = value; }
        public int HealPerLevel { get => _healPerLevel; set => _healPerLevel = value; }
        public int ArmorPerLevel { get => _armorPerLevel; set => _armorPerLevel = value; }
        public int MaxManaPerLevel { get => _maxManaPerLevel; set => _maxManaPerLevel = value; }
        public int XpRewardPerLevel { get => _xpRewardPerLevel; set => _xpRewardPerLevel = value; }
        [HideInInspector]
        public string AttackAnimationName { get => ""; set { } }

        [BoxGroup("Abilities")]
        [Tooltip("Набор способностей персонажа")]
        public CharacterAbilitySet _abilitySet;
        public CharacterAbilitySet AbilitySet { get => _abilitySet; set => _abilitySet = value; }

        private void UpdateStats()
        {
            _damage = _damagePerLevel * _level;
            _maxHealth = _maxHealthPerLevel * _level;
            _heal = _healPerLevel * _level;
            _armor = _armorPerLevel * _level;
            _maxMana = _maxManaPerLevel * _level;
            _xpReward = _xpRewardPerLevel * _level;

            _health = _maxHealth;
            _mana = _maxMana;
        }

        public void ValidateAndFixData()
        {
            // Если уровень 0 или не задан, устанавливаем минимум 1
            if (_level <= 0)
            {
                _level = 1;
                UpdateStats();
            }

            // Если MaxHealth не задан, вычисляем
            if (_maxHealth <= 0)
            {
                _maxHealth = _maxHealthPerLevel * _level;
            }

            // Если MaxXP не задан, устанавливаем базовое значение
            if (_maxXP <= 0)
            {
                _maxXP = 100 * _level;
            }

            // Если MaxMana не задан, вычисляем
            if (_maxMana <= 0)
            {
                _maxMana = _maxManaPerLevel * _level;
            }

            // Ограничиваем текущее здоровье максимальным
            if (_health > _maxHealth)
            {
                _health = _maxHealth;
            }

            // Если здоровье 0 или меньше, восстанавливаем до максимума
            if (_health <= 0)
            {
                _health = _maxHealth;
            }

            // Ограничиваем текущую ману максимальной
            if (_mana > _maxMana)
            {
                _mana = _maxMana;
            }

            // Если мана 0 или меньше, восстанавливаем до максимума
            if (_mana <= 0)
            {
                _mana = _maxMana;
            }
        }
    }
}
