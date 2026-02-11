using UnityEngine;
using Sirenix.OdinInspector; // Ты используешь Odin, это круто

[CreateAssetMenu(fileName = "New Melee Attack", menuName = "Battle/Melee Ability")]
public class MeleeAbility : BattleAbility
{
    [Title("Damage Settings")]
    [SerializeField] private int baseDamageMultiplier = 1;

    [Title("Status Effect Settings")]
    [SerializeField] private bool hasStatusEffect = false;

    [ShowIf("hasStatusEffect")]
    [SerializeField] private StatusEffectSO statusEffect;

    [ShowIf("hasStatusEffect")]
    [Range(0, 100)]
    [SerializeField] private float chanceToApply = 100f;

    public override void Execute(Base attacker, Base target)
    {
        int finalDamage = attacker.GiveDamage() * baseDamageMultiplier;
        Debug.Log($"{attacker.Name} использует {AbilityName} на {target.Name} с уроном {finalDamage}");
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
                Debug.Log($"Наложен эффект {statusEffect.effectName}!");
            }
        }
    }
}