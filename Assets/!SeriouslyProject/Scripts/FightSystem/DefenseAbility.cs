using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Defense Ability", menuName = "Battle/Defense Ability")]
public class DefenseAbility : BattleAbility
{
    [Title("Defense Settings")]
    [Tooltip("Дополнительная защита")]
    [SerializeField] private int bonusDefense = 2;

    [Tooltip("Длительность эффекта в ходах")]
    [SerializeField] private int duration = 1;

    [Title("Status Effect")]
    [Tooltip("Эффект защиты (опционально)")]
    [SerializeField] private StatusEffectSO defenseEffect;

    public override void Execute(Base attacker, Base target)
    {
        attacker.Mana -= ManaCost;

        PlayHitAnimation(attacker);
        
        Debug.Log($"{attacker.Name} использует {AbilityName}! Защита увеличена на {bonusDefense}");

        if (defenseEffect != null)
        {
            attacker.ApplyStatusEffect(defenseEffect);
            if (defenseEffect.armorBonus != 0)
            {
                attacker.Armor += defenseEffect.armorBonus;
            }
        }
        else
        {
            StatusEffectSO tempDefense = CreateInstance<StatusEffectSO>();
            tempDefense.effectName = "Защита";
            tempDefense.duration = duration;
            tempDefense.damagePerTurn = 0;
            tempDefense.tickColor = Color.blue;
            tempDefense.armorBonus = bonusDefense;
            
            attacker.ApplyStatusEffect(tempDefense);
            attacker.Armor += bonusDefense;
        }

        attacker.UpdateUI();
    }
}
