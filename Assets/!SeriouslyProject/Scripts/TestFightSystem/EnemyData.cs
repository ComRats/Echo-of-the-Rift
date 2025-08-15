using UnityEngine;

namespace FightSystem.Data
{
    [CreateAssetMenu(fileName = "New Enemy", menuName = "ScriptableObjects/TestEnemy")]
    public class EnemyData : ScriptableObject
    {
        public string _name;
        [TextArea(3, 10)]
        public string _description;

        public Sprite _sprite;

        public int _damage;
        public int _priority;
        public int _maxMana;
        public int _mana;
        public int _maxHealth;
        public int _health;
        public int _heal;
        public int _armor;
    }
}
