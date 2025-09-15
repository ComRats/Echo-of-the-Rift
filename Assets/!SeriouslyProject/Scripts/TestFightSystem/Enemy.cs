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
            Inizialize(enemyData, gameObject);

            UpdateUI();

        }

        public void InitializeFromSettings(EnemyesSettings settings)
        {
            if (settings.useEnemyData)
            {
                enemyData = settings.GetEnemyData();

                if (enemyData != null)
                {
                    Inizialize(enemyData, gameObject);
                }
                else
                {
                    Debug.LogWarning($"Не найден EnemyData с именем {settings.enemyDataName}");
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
                XpReward = settings._xpReward;

            }

            // Обновление UI
            UpdateUI();
        }
    }
}
