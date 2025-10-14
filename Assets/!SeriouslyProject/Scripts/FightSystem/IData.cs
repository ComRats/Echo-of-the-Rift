using UnityEngine;

public interface IData
{
    string Name { get; set; }
    string Description { get; set; }
    Sprite Sprite { get; set; }

    int Damage { get; set; }
    int Priority { get; set; }
    int MaxMana { get; set; }
    int Mana { get; set; }
    int MaxHealth { get; set; }
    int Health { get; set; }
    int Heal { get; set; }
    int Armor { get; set; }
    int Lucky { get; set; }
    int CreteChance { get; set; }

    int Level { get; set; }
    int CurrentXP { get; set; }
    int MaxXP { get; set; }

    int XpReward { get; set; }

    int DamagePerLevel { get; set; }
    int MaxHealthPerLevel { get; set; }
    int HealPerLevel { get; set; }
    int ArmorPerLevel { get; set; }
    int MaxManaPerLevel { get; set; }
    int XpRewardPerLevel { get; set; }
}
