using FightSystem.Data;
using UnityEngine;

public class Player : MonoBehaviour
{
    public SpriteRenderer spriteRenderer;
    public PressableButtons pressableButtons;
    public CameraSettings cameraSettings;
    public TestMovement movement;
    public PlayerSaver playerSaver;

    public static FightResult Result = FightResult.None;
    public Vector3 startPosition;

    public void Hide()
    {
        Debug.LogWarning("Hide player");

        spriteRenderer.enabled = false;
        movement.enabled = false;
        cameraSettings.enabled = false;
        pressableButtons.enabled = false;
    }

    public void Show()
    {
        Debug.LogWarning("Show player");

        spriteRenderer.enabled = true;
        movement.enabled = true;
        cameraSettings.enabled = true;
        pressableButtons.enabled = true;
    }

    [System.Serializable]
    public class PlayerSaver : IData
    {
        [SerializeField] private string name;
        [SerializeField] private string description;
        [SerializeField] private Sprite sprite;
        [SerializeField] private string spritePath;

        [SerializeField] private int damage;
        [SerializeField] private int priority;
        [SerializeField] private int maxMana;
        [SerializeField] private int mana;
        [SerializeField] private int maxHealth;
        [SerializeField] private int health;
        [SerializeField] private int heal;
        [SerializeField] private int armor;
        [SerializeField] private int lucky;
        [SerializeField] private int creteChance;

        [SerializeField] private int level;
        [SerializeField] private int currentXP;
        [SerializeField] private int maxXP;

        [SerializeField] private int xpReward;

        [SerializeField] private int damagePerLevel;
        [SerializeField] private int maxHealthPerLevel;
        [SerializeField] private int healPerLevel;
        [SerializeField] private int armorPerLevel;
        [SerializeField] private int maxManaPerLevel;
        [SerializeField] private int xpRewardPerLevel;
            
        public string Name { get => name; set => name = value; }
        public string Description { get => description; set => description = value; }
        public Sprite Sprite { get => sprite; set => sprite = value; }

        public int Damage { get => damage; set => damage = value; }
        public int Priority { get => priority; set => priority = value; }
        public int MaxMana { get => maxMana; set => maxMana = value; }
        public int Mana { get => mana; set => mana = value; }
        public int MaxHealth { get => maxHealth; set => maxHealth = value; }
        public int Health { get => health; set => health = value; }
        public int Heal { get => heal; set => heal = value; }
        public int Armor { get => armor; set => armor = value; }
        public int Lucky { get => lucky; set => lucky = value; }
        public int CreteChance { get => creteChance; set => creteChance = value; }

        public int Level { get => level; set => level = value; }
        public int CurrentXP { get => currentXP; set => currentXP = value; }
        public int MaxXP { get => maxXP; set => maxXP = value; }

        public int XpReward { get => xpReward; set => xpReward = value; }

        public int DamagePerLevel { get => damagePerLevel; set => damagePerLevel = value; }
        public int MaxHealthPerLevel { get => maxHealthPerLevel; set => maxHealthPerLevel = value; }
        public int HealPerLevel { get => healPerLevel; set => healPerLevel = value; }
        public int ArmorPerLevel { get => armorPerLevel; set => armorPerLevel = value; }
        public int MaxManaPerLevel { get => maxManaPerLevel; set => maxManaPerLevel = value; }
        public int XpRewardPerLevel { get => xpRewardPerLevel; set => xpRewardPerLevel = value; }

        public void LoadFrom(CharacterData data)
        {
            Name = data.Name;
            Description = data.Description;

            spritePath = data.Sprite != null ? $"CharacterData/{data.Sprite.name}" : null;

            Damage = data.Damage;
            Priority = data.Priority;
            MaxMana = data.MaxMana;
            Mana = data.Mana;
            MaxHealth = data.MaxHealth;
            Health = data.Health;
            Heal = data.Heal;
            Armor = data.Armor;
            Lucky = data.Lucky;
            CreteChance = data.CreteChance;

            Level = data.Level;
            CurrentXP = data.CurrentXP;
            MaxXP = data.MaxXP;
            XpReward = data.XpReward;

            DamagePerLevel = data.DamagePerLevel;
            MaxHealthPerLevel = data.MaxHealthPerLevel;
            HealPerLevel = data.HealPerLevel;
            ArmorPerLevel = data.ArmorPerLevel;
            MaxManaPerLevel = data.MaxManaPerLevel;
            XpRewardPerLevel = data.XpRewardPerLevel;
        }

        public Sprite GetSprite()
        {
            return spritePath != null ? Resources.Load<Sprite>(spritePath) : null;
        }
    }

}
