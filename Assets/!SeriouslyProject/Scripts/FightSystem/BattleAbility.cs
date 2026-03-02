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
    [Tooltip("��� �������� � ��������� ����")]
    public string animTrigger = "Hit";

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

    public virtual bool CanUse(Base attacker)
    {
        if (attacker.Mana < ManaCost)
        {
            Debug.LogWarning($"{attacker.Name} �� ������� ����! ����� {ManaCost}, ���� {attacker.Mana}");
            return false;
        }
        return true;
    }

    public abstract void Execute(Base attacker, Base target);
}