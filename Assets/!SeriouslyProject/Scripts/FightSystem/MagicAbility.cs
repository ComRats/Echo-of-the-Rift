using UnityEngine;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "New Magic Spell", menuName = "Battle/Magic Ability")]
public class MagicAbility : BattleAbility
{
    [Title("Magic Settings")]
    [SerializeField] private int magicDamage = 10;
    [SerializeField] private GameObject vfxEffect;

    [Title("Status Effect Settings")]
    [SerializeField] private StatusEffectSO statusEffect;
    [Range(0, 100)]
    [SerializeField] private float chance = 100f;

    public override void Execute(Base attacker, Base target)
    {
        attacker.Mana -= ManaCost;

        target.TakeMagicDamage(magicDamage);

        if (vfxEffect != null)
            Instantiate(vfxEffect, target.transform.position, Quaternion.identity);

        if (statusEffect != null && Random.Range(0, 100) <= chance)
        {
            target.ApplyStatusEffect(statusEffect);
        }
    }
}