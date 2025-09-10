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

        private Button button;

        private void Awake()
        {   
            Sprite = GetComponent<Image>();

            //LocalInizialize();

            button = GetComponent<Button>();
        }

        private void LocalInizialize()
        {
            Inizialize(characterData, gameObject);

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
                    Inizialize(characterData, gameObject);
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
            UpdateUI();
        }
    }
}