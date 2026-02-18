using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Defense Ability", menuName = "Battle/Defense Ability")]
public class DefenseAbility : BattleAbility
{
    [Title("Defense Settings")]
    [Tooltip("Множитель защиты (например, 2 = удвоенная защита на 1 ход)")]
    [SerializeField] private float defenseMultiplier = 2f;

    [Tooltip("Длительность эффекта в ходах")]
    [SerializeField] private int duration = 1;

    [Title("Status Effect")]
    [Tooltip("Эффект защиты (опционально)")]
    [SerializeField] private StatusEffectSO defenseEffect;

    public override void Execute(Base attacker, Base target)
    {
        attacker.Mana -= ManaCost;

        PlayHitAnimation(attacker);

        // Временное увеличение защиты
        int armorBonus = Mathf.RoundToInt(attacker.Armor * (defenseMultiplier - 1f));
        
        Debug.Log($"{attacker.Name} использует {AbilityName}! Защита увеличена на {armorBonus}");

        if (defenseEffect != null)
        {
            attacker.ApplyStatusEffect(defenseEffect);
        }
        else
        {
            // Создаем временный эффект защиты
            StatusEffectSO tempDefense = ScriptableObject.CreateInstance<StatusEffectSO>();
            tempDefense.effectName = "Защита";
            tempDefense.duration = duration;
            tempDefense.damagePerTurn = 0;
            tempDefense.tickColor = Color.blue;
            
            attacker.ApplyStatusEffect(tempDefense);
            attacker.Armor += armorBonus;
        }

        attacker.UpdateUI();
    }
}
