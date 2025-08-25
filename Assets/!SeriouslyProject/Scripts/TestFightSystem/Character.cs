using FightSystem.Data;
using UnityEngine;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

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

        public void InitializeFromSettings(CharactersSettings settings)
        {
            if (settings.useCharacterData)
            {
                characterData = settings.GetCharacterData();

                if (characterData != null)
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
                }
                else
                {
                    Debug.LogWarning($"Не найден CharacterData с именем {settings.characterDataName}");
                }
            }
            else
            {
                Name = gameObject.name = settings._name;
                Sprite.sprite = settings.GetSprite();
                Description = settings._description;
                Damage = settings._damage;
                MaxHealth = settings._maxHealth;
                Health = settings._health;
                MaxMana = settings._maxMana;
                Mana = settings._mana;
                Heal = settings._heal;
                Priority = settings._priority;
                Armor = settings._armor;
            }

            // Обновление UI
            healthText.text = Health + " / " + MaxHealth;
            manaText.text = Mana + " / " + MaxMana;
            healthBar.minValue = 0;
            healthBar.maxValue = MaxHealth;
            healthBar.value = Health;

            SetGradient(1f);
        }

        public void GetXP(int _getXP)
        {
            currentXP += _getXP;

            UpdateLevel();
        }

        private void UpdateLevel()
        {
            if (currentXP >= MaxXP)
            {
                Damage = characterData._damagePerLevel * Level;
                MaxHealth = characterData._maxHealthPerLevel * Level;
                Heal = characterData._healPerLevel * Level;
                Armor = characterData._armorPerLevel * Level;
                MaxMana = characterData._maxManaPerLevel * Level;
                XPreward = characterData._xpRewardPerLevel * Level;

                Health = MaxHealth;
                Mana = MaxMana;
            }
        }
    }
}