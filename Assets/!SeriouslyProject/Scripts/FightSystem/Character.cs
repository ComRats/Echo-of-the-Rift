using FightSystem.Data;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;

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
            Initialize(characterData, gameObject);

            UpdateUI();
        }

        public void InitializeFromSettings(CharactersSettings settings)
        {
            if (settings.useCharacterData)
            {
                characterData = settings.GetCharacterData();

                if (characterData != null)
                {
                    Initialize(characterData, gameObject);
                }
                else
                {
                    Debug.LogWarning($"Не найден CharacterData с именем {settings.characterDataName}");
                }
            }
            else
            {
                Name = characterData.Name;
                Description = characterData.Description;
                Damage = characterData.Damage;
                Priority = characterData.Priority;
                MaxMana = characterData.MaxMana;
                Mana = characterData.Mana;
                MaxHealth = characterData.MaxHealth;
                Health = characterData.Health;
                Heal = characterData.Heal;
                Armor = characterData.Armor;
                Lucky = characterData.Lucky;
                CreteChance = characterData.CreteChance;
                Level = characterData.Level;
                CurrentXP = characterData.CurrentXP;
                MaxXP = characterData.MaxXP;
                XpReward = characterData.XpReward;
                DamagePerLevel = characterData.DamagePerLevel;
                MaxHealthPerLevel = characterData.MaxHealthPerLevel;
                HealPerLevel = characterData.HealPerLevel;
                ArmorPerLevel = characterData.ArmorPerLevel;
                MaxManaPerLevel = characterData.MaxManaPerLevel;
                XpRewardPerLevel = characterData.XpRewardPerLevel;
                Sprite.sprite = characterData.Sprite;
            }

            // Обновление UI
            UpdateUI();
        }
    }
}