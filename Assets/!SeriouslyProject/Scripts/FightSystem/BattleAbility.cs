using UnityEngine;
using Sirenix.OdinInspector;

public abstract class BattleAbility : ScriptableObject
{
    [Title("General Settings")]
    public string AbilityName;
    public int ManaCost;

    [Title("Target Settings")]
    [Tooltip("Тип цели для способности")]
    public TargetType targetType = TargetType.Enemy;

    [Title("Animation Settings")]
    [Tooltip("Триггер анимации в аниматоре цели")]
    public string animTrigger = "Hit";

    [Title("VFX")]
    [Tooltip("Prefab партикла, спавнится на цели при использовании способности")]
    public GameObject vfxPrefab;

    [TextArea]
    [Tooltip("[dmg]...[/dmg] - урон (красный цвет)" +
        "\r\n[heal]...[/heal] - исцеление (зеленый цвет)" +
        "\r\n[def]...[/def] - защита (синий цвет)" +
        "\r\n[mana]...[/mana] - мана (фиолетовый цвет)")]
    public string Description;

    protected void PlayHitAnimation(Base target)
    {
        if (target != null && !string.IsNullOrEmpty(animTrigger)) 
        {
            target.PlayAnimation(animTrigger);
        }
    }

    protected void SpawnVFX(Base target)
    {
        if (vfxPrefab == null || target == null) return;

        var go = Object.Instantiate(vfxPrefab, target.transform);
        go.transform.localPosition = Vector3.zero;

        var uiParticle = go.GetComponent<Coffee.UIExtensions.UIParticle>();
        if (uiParticle != null)
        {
            uiParticle.Play();
            float duration = uiParticle.GetComponentsInChildren<ParticleSystem>() is { Length: > 0 } systems
                ? Mathf.Max(System.Array.ConvertAll(systems, s => s.main.duration + s.main.startLifetime.constantMax))
                : 2f;
            Object.Destroy(go, duration);
        }
        else
        {
            // Fallback: обычный ParticleSystem без UIParticle (не будет виден в Canvas)
            var ps = go.GetComponent<ParticleSystem>();
            if (ps != null)
                Object.Destroy(go, ps.main.duration + ps.main.startLifetime.constantMax);
            else
                Object.Destroy(go, 2f);
        }
    }

    public virtual bool CanUse(Base attacker)
    {
        if (attacker.Mana < ManaCost)
        {
            Debug.LogWarning($"{attacker.Name} не хватает маны! Нужно {ManaCost}, есть {attacker.Mana}");
            return false;
        }
        return true;
    }

    public abstract void Execute(Base attacker, Base target);
}