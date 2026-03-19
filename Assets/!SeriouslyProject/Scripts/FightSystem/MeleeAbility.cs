using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Melee Attack", menuName = "Battle/Melee Ability")]
public class MeleeAbility : BattleAbility
{
    [Title("Damage Settings")]
    [SerializeField] private int baseDamageMultiplier = 1;
    [Tooltip("Плоский бонус урона, прибавляется к итогу независимо от модификатора")]
    [SerializeField] private int flatDamageBonus = 0;

    [Title("Status Effect Settings")]
    [SerializeField] private bool hasStatusEffect = false;

    [ShowIf("hasStatusEffect")]
    [SerializeField] private StatusEffectSO statusEffect;

    [ShowIf("hasStatusEffect")]
    [Range(0, 100)]
    [SerializeField] private float chanceToApply = 100f;

    public override void Execute(Base attacker, Base target)
    {
        PlayHitAnimation(target);

        int finalDamage = attacker.GiveDamage() * baseDamageMultiplier + flatDamageBonus;
        Debug.Log($"{attacker.Name} ���������� {AbilityName} �� {target.Name} � ������ {finalDamage}");
        target.TakeDamage(finalDamage);

        TryApplyEffect(target);
    }

    private void TryApplyEffect(Base target)
    {
        if (hasStatusEffect && statusEffect != null)
        {
            if (Random.Range(0f, 100f) <= chanceToApply)
            {
                target.ApplyStatusEffect(statusEffect);
                Debug.Log($"������� ������ {statusEffect.effectName}!");
            }
        }
    }
}