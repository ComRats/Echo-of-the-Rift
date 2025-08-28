using FightSystem.Data;
using UnityEditor.U2D.Animation;
using UnityEngine;
using UnityEngine.UI;

namespace FightSystem.Enemy
{
    public class Enemy : Base
    {
        [SerializeField] private EnemyData enemyData;
        [SerializeField] private ContextMenu contextMenu;

        private Button button;

        private void Awake()
        {
            Sprite = GetComponent<Image>();

            Inizialize();
            StartCoroutine(Blinking());

            button = GetComponent<Button>();
            button.onClick.AddListener(SetComponent);


            if (contextMenu == null)
                contextMenu = FindObjectOfType<ContextMenu>();
        }

        public void Inizialize()
        {
            Name = gameObject.name = enemyData._name;
            Sprite.sprite = enemyData._sprite;
            Description = enemyData._description;
            Damage = enemyData._damage;
            MaxHealth = enemyData._maxHealth;
            Health = enemyData._health;
            MaxMana = enemyData._maxMana;
            Mana = enemyData._mana;
            Heal = enemyData._heal;
            Priority = enemyData._priority;
            XPreward = enemyData._xpReward;

            healthText.text = Health.ToString() + " /" + MaxHealth;
            manaText.text = Mana.ToString() + " /" + MaxMana;
            healthBar.minValue = 0;
            healthBar.maxValue = MaxHealth;
            healthBar.value = Health;

            SetGradient(1f);
        }

        private void SetComponent()
        {
            contextMenu.Enemy = GetComponent<Enemy>();
            contextMenu.FightStateController();
        }

        public void InitializeFromSettings(EnemyesSettings settings)
        {
            if (settings.useEnemyData)
            {
                enemyData = settings.GetEnemyData();

                if (enemyData != null)
                {
                    Name = gameObject.name = enemyData._name;
                    Sprite.sprite = enemyData._sprite;
                    Description = enemyData._description;
                    Damage = enemyData._damage;
                    MaxHealth = enemyData._maxHealth;
                    Health = enemyData._health;
                    MaxMana = enemyData._maxMana;
                    Mana = enemyData._mana;
                    Heal = enemyData._heal;
                    Priority = enemyData._priority;
                    XPreward = enemyData._xpReward;

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
                XPreward = settings._xpReward;

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
                Damage = enemyData._damagePerLevel * Level;
                MaxHealth = enemyData._maxHealthPerLevel * Level;
                Heal = enemyData._healPerLevel * Level;
                Armor = enemyData._armorPerLevel * Level;
                MaxMana = enemyData._maxManaPerLevel * Level;
                XPreward = enemyData._xpRewardPerLevel * Level;

                Health = MaxHealth;
                Mana = MaxMana;


            }
        }
    }
}
