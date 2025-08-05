using FightSystem.Data;
using UnityEngine;
using UnityEngine.UI;

namespace FightSystem.Character
{
    public class Character : Base
    {
        public bool IsTurn { get; set; } = false;

        [Header("DataCharacter")]
        [SerializeField] private CharacterData characterData;
        [Header("ContextMenu")]
        [SerializeField] private ContextMenu contextMenu;

        private Button button;

        private void Awake()
        {   
            Sprite = GetComponent<Image>();

            Inizialize();

            button = GetComponent<Button>();

            if (contextMenu == null)
                contextMenu = FindObjectOfType<ContextMenu>();

            button.onClick.AddListener(() =>
            {
                contextMenu.SetCharacter(this); // Передаём себя напрямую
                contextMenu.ChangePosition(transform.GetChild(0), this);
                contextMenu.FightStateController();
            });

        }

        private void Inizialize()
        {
            Name = gameObject.name = characterData._name;
            Sprite.sprite = characterData._sprite;
            Description = characterData._description;
            Damage = characterData._damage;
            MaxHealth = characterData._maxHealth;
            Health = characterData._health;
            MaxMana = characterData._maxMana;
            Mana = characterData._mana;
            Heal = characterData._heal;
            Priority = characterData._priority;
            Armor = characterData._armor;

            healthText.text = Health.ToString() + " / " + MaxHealth;
            manaText.text = Mana.ToString() + " / " + MaxMana;
            healthBar.minValue = 0;
            healthBar.maxValue = MaxHealth;
            healthBar.value = Health;

            SetGradient(1f);
        }
    }
}
