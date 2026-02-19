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

        public CharacterAbilitySet AbilitySet { get; private set; }

        private ActionButtons actionButtons;
        private Button button;

        private void Awake()
        {   
            Sprite = GetComponent<Image>();

            actionButtons = FindObjectOfType<ActionButtons>();
            button = GetComponent<Button>();
            
            if (button != null)
            {
                button.onClick.AddListener(() => 
                {
                    actionButtons.OnCharacterSelected(this);
                });
            }
        }

        private void LocalInizialize()
        {
            Initialize(characterData, gameObject);
            AbilitySet = characterData.AbilitySet;
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
                    Debug.LogError($"�� ������ CharacterData � ������ {settings.characterDataName}");
                }
            }
            else
            {
                CharacterData character = new();

                character.Name = settings.Name;
                character.Description = settings.Description;
                character.Sprite = settings.GetSprite();
                character.Damage = settings.Damage;
                character.Priority = settings.Priority;
                character.MaxMana = settings.MaxMana;
                character.Mana = settings.Mana;
                character.MaxHealth = settings.MaxHealth;
                character.Health = settings.Health;
                character.Heal = settings.Heal;
                character.Armor = settings.Armor;
                character.Lucky = settings.Lucky;
                character.CreteDamage = settings.CreteDamage;
                character.Level = settings.Level;
                character.CurrentXP = settings.CurrentXP;
                character.MaxXP = settings.MaxXP;
                character.XpReward = settings.XpReward;
                character.DamagePerLevel = settings.DamagePerLevel;
                character.MaxHealthPerLevel = settings.MaxHealthPerLevel;
                character.HealPerLevel = settings.HealPerLevel;
                character.ArmorPerLevel = settings.ArmorPerLevel;
                character.MaxManaPerLevel = settings.MaxManaPerLevel;
                character.XpRewardPerLevel = settings.XpRewardPerLevel;

                Initialize(character, gameObject);
            }

            AbilitySet = characterData?.AbilitySet;

            // ���������� UI
            UpdateUI();
        }
    }
}