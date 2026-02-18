using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Heal Ability", menuName = "Battle/Heal Ability")]
public class HealAbility : BattleAbility
{
    [Title("Heal Settings")]
    [Tooltip("Базовое количество лечения")]
    [SerializeField] private int baseHealAmount = 20;

    [Tooltip("Использовать параметр Heal персонажа")]
    [SerializeField] private bool useCharacterHealStat = true;

    [Tooltip("Множитель лечения")]
    [SerializeField] private float healMultiplier = 1.5f;

    [Title("Visual Effects")]
    [SerializeField] private GameObject healVFX;

    [Title("Status Effect")]
    [Tooltip("Дополнительный эффект (например, регенерация)")]
    [SerializeField] private StatusEffectSO healOverTimeEffect;

    public override void Execute(Base attacker, Base target)
    {
        attacker.Mana -= ManaCost;

        PlayHitAnimation(target);

        int healAmount;
        if (useCharacterHealStat)
        {
            healAmount = Mathf.RoundToInt(attacker.GiveHeal() * healMultiplier);
        }
        else
        {
            healAmount = baseHealAmount;
        }

        target.TakeHeal(healAmount);

        Debug.Log($"{attacker.Name} использует {AbilityName} на {target.Name}! Восстановлено {healAmount} HP");

        if (healVFX != null)
        {
            Instantiate(healVFX, target.transform.position, Quaternion.identity);
        }

        if (healOverTimeEffect != null)
        {
            target.ApplyStatusEffect(healOverTimeEffect);
        }

        attacker.UpdateUI();
        target.UpdateUI();
    }
}
