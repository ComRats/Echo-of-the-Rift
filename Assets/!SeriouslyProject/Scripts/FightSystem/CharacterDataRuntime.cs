using UnityEngine;

namespace FightSystem.Data
{
    /// <summary>
    /// Runtime копия CharacterData которая не изменяет оригинальный ScriptableObject
    /// </summary>
    [System.Serializable]
    public class CharacterDataRuntime : IData
    {
        // Ссылка на оригинальный ScriptableObject (только для чтения)
        [System.NonSerialized]
        public CharacterData originalData;

        // Runtime данные (изменяемые)
        public string _name;
        public string _description;
        [System.NonSerialized]
        public Sprite _sprite;
        public string spritePath; // Для сериализации

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

        public int _level;
        public int _currentXP;
        public int _maxXP;
        public int _xpReward;

        public int _damagePerLevel;
        public int _maxHealthPerLevel;
        public int _healPerLevel;
        public int _armorPerLevel;
        public int _maxManaPerLevel;
        public int _xpRewardPerLevel;

        [System.NonSerialized]
        public CharacterAbilitySet _abilitySet;

        // Свойства IData
        public string Name { get => _name; set => _name = value; }
        public string Description { get => _description; set => _description = value; }
        public Sprite Sprite 
        { 
            get 
            {
                if (_sprite == null && !string.IsNullOrEmpty(spritePath))
                {
                    _sprite = Resources.Load<Sprite>(spritePath);
                }
                return _sprite;
            }
            set => _sprite = value;
        }

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
        public int Level { get => _level; set => _level = value; }
        public int CurrentXP { get => _currentXP; set => _currentXP = value; }
        public int MaxXP { get => _maxXP; set => _maxXP = value; }
        public int XpReward { get => _xpReward; set => _xpReward = value; }
        public int DamagePerLevel { get => _damagePerLevel; set => _damagePerLevel = value; }
        public int MaxHealthPerLevel { get => _maxHealthPerLevel; set => _maxHealthPerLevel = value; }
        public int HealPerLevel { get => _healPerLevel; set => _healPerLevel = value; }
        public int ArmorPerLevel { get => _armorPerLevel; set => _armorPerLevel = value; }
        public int MaxManaPerLevel { get => _maxManaPerLevel; set => _maxManaPerLevel = value; }
        public int XpRewardPerLevel { get => _xpRewardPerLevel; set => _xpRewardPerLevel = value; }
        public string AttackAnimationName { get => ""; set { } }

        public CharacterAbilitySet AbilitySet { get => _abilitySet; set => _abilitySet = value; }

        /// <summary>
        /// Создает runtime копию из ScriptableObject
        /// </summary>
        public static CharacterDataRuntime CreateFromScriptableObject(CharacterData data)
        {
            var runtime = new CharacterDataRuntime
            {
                originalData = data,
                _name = data.Name,
                _description = data.Description,
                _sprite = data.Sprite,
                spritePath = data.Sprite != null ? $"CharacterData/{data.Sprite.name}" : null,
                
                _damage = data.Damage,
                _magicDamage = data.MagicDamage,
                _priority = data.Priority,
                _maxMana = data.MaxMana,
                _mana = data.Mana,
                _maxHealth = data.MaxHealth,
                _health = data.Health,
                _heal = data.Heal,
                _armor = data.Armor,
                _lucky = data.Lucky,
                _creteChance = data.CreteDamage,
                
                _level = data.Level,
                _currentXP = data.CurrentXP,
                _maxXP = data.MaxXP,
                _xpReward = data.XpReward,
                
                _damagePerLevel = data.DamagePerLevel,
                _maxHealthPerLevel = data.MaxHealthPerLevel,
                _healPerLevel = data.HealPerLevel,
                _armorPerLevel = data.ArmorPerLevel,
                _maxManaPerLevel = data.MaxManaPerLevel,
                _xpRewardPerLevel = data.XpRewardPerLevel,
                
                _abilitySet = data.AbilitySet
            };

            runtime.ValidateAndFixData();
            return runtime;
        }

        /// <summary>
        /// Валидация и исправление данных
        /// </summary>
        public void ValidateAndFixData()
        {
            // Клэмпим только если значения уже были назначены (не нулевые)
            if (_maxHealth > 0)
                _health = Mathf.Clamp(_health, 0, _maxHealth);

            if (_maxMana > 0)
                _mana = Mathf.Clamp(_mana, 0, _maxMana);

            if (_maxXP <= 0)
                _maxXP = 100;
        }

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
    }
}
