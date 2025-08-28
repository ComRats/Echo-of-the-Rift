using UnityEngine;
using Sirenix.OdinInspector;

namespace FightSystem.Data
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/TestEnemy")]
    public class EnemyData : ScriptableObject
    {
        [BoxGroup("General Info")]
        public string _name;
        [TextArea]
        public string _description;
        public Sprite _sprite;

        [BoxGroup("Stats (Base)")]
        public int _damage;
        public int _priority;
        public int _maxMana;
        public int _mana;
        public int _maxHealth;
        public int _health;
        public int _heal;
        public int _armor;

        [BoxGroup("Progression")]
        [OnValueChanged("UpdateStats")]
        public int _level;
        public int _currentXP;
        public int _maxXP;

        [BoxGroup("Rewards")]
        public int _xpReward;

        [BoxGroup("Per Level Growth")]
        public int _damagePerLevel = 1;
        public int _maxHealthPerLevel = 1;
        public int _healPerLevel = 1;
        public int _armorPerLevel = 1;
        public int _maxManaPerLevel = 1;
        public int _xpRewardPerLevel = 1;

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
