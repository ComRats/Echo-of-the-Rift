using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Battle/Status Effect")]
public class StatusEffectSO : ScriptableObject
{
    public string effectName;
    public int damagePerTurn;
    public int duration;
    public Color tickColor = Color.red;
}

[System.Serializable]
public class ActiveStatusEffect
{
    public StatusEffectSO data;
    public int remainingTurns;

    public ActiveStatusEffect(StatusEffectSO effectData)
    {
        data = effectData;
        remainingTurns = effectData.duration;
    }
}