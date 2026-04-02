using UnityEngine;

[CreateAssetMenu(fileName = "New Bleed Attack", menuName = "Battle/Abilities/Status Ability")]
public class StatusAbility : BattleAbility
{
    public StatusEffectSO effect;
    public float damageMultiplier = 1.0f;

    public override void Execute(Base attacker, Base target)
    {
        PlayHitAnimation(target);

        int damage = Mathf.RoundToInt(attacker.GiveDamage() * damageMultiplier);
        target.TakeDamage(damage);
        SpawnVFX(target);

        if (effect != null)
            target.ApplyStatusEffect(effect);
    }

    public override bool CanUse(Base attacker) => attacker.Mana >= ManaCost;
}