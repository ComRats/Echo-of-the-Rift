using UnityEngine;

namespace FightSystem.Data
{
    [CreateAssetMenu(fileName = "New Characters", menuName = "ScriptableObjects/TestCharacter")]
    public class CharacterData : ScriptableObject
    {
        public string _name;
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
