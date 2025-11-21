using FightSystem.Data;
using UnityEngine;
using UnityEngine.UI;

namespace FightSystem.Enemy
{
    public class Enemy : Base
    {
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private ActionButtons actionButtons;

        private Button button;

        private void Awake()
        {
            Sprite = GetComponent<Image>();

            //LocalInizialize();

            StartCoroutine(Blinking());

            button = GetComponent<Button>();
            button.onClick.AddListener(() => 
            {
                actionButtons.currentEnemy = this;
                actionButtons.OnEnemySelected(this);
            });

            actionButtons = FindObjectOfType<ActionButtons>();
        }

        public void LocalInizialize()
        {
            Initialize(enemyData, gameObject);

            UpdateUI();

        }

        public void InitializeFromSettings(EnemyesSettings settings)
        {
            if (settings.useEnemyData)
            {
                enemyData = settings.GetEnemyData();

                if (enemyData != null)
                {
                    Initialize(enemyData, gameObject);
                }
                else
                {
                    Debug.LogError($"Не найден EnemyData с именем {settings.enemyDataName}");
                }
            }
            else
            {
                EnemyData enemy = new();

                enemy.Name = settings.Name;
                enemy.Description = settings.Description;
                enemy.Sprite = settings.GetSprite();
                enemy.Damage = settings.Damage;
                enemy.Priority = settings.Priority;
                enemy.MaxMana = settings.MaxMana;
                enemy.Mana = settings.Mana;
                enemy.MaxHealth = settings.MaxHealth;
                enemy.Health = settings.Health;
                enemy.Heal = settings.Heal;
                enemy.Armor = settings.Armor;
                enemy.Lucky = settings.Lucky;
                enemy.CreteChance = settings.CreteChance;
                enemy.Level = settings.Level;
                enemy.CurrentXP = settings.CurrentXP;
                enemy.MaxXP = settings.MaxXP;
                enemy.XpReward = settings.XpReward;
                enemy.DamagePerLevel = settings.DamagePerLevel;
                enemy.MaxHealthPerLevel = settings.MaxHealthPerLevel;
                enemy.HealPerLevel = settings.HealPerLevel;
                enemy.ArmorPerLevel = settings.ArmorPerLevel;
                enemy.MaxManaPerLevel = settings.MaxManaPerLevel;
                enemy.XpRewardPerLevel = settings.XpRewardPerLevel;

                Initialize(enemy, gameObject);
            }

            // Обновление UI
            UpdateUI();
        }
    }
}
