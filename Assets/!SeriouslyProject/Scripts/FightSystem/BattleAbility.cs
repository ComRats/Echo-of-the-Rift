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
        go.transform.SetSiblingIndex(target.transform.GetSiblingIndex() + 1);

        // UIParticle нужен чтобы ParticleSystem корректно рендерился внутри Canvas
        var uiParticle = go.GetComponent<Coffee.UIExtensions.UIParticle>();
        if (uiParticle == null)
            uiParticle = go.AddComponent<Coffee.UIExtensions.UIParticle>();

        uiParticle.Play();

        var ps = go.GetComponent<ParticleSystem>();
        if (ps != null)
            Object.Destroy(go, ps.main.duration + ps.main.startLifetime.constantMax);
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