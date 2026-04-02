using UnityEngine;

[CreateAssetMenu(fileName = "New Status Effect", menuName = "Battle/Status Effect")]
public class StatusEffectSO : ScriptableObject
{
    public string effectName;
    public int damagePerTurn;
    public int duration;
    public Color tickColor = Color.red;
    public int armorBonus;

    [Tooltip("Prefab партикла, живёт пока эффект активен (looping)")]
    public GameObject vfxPrefab;
}

[System.Serializable]
public class ActiveStatusEffect
{
    public StatusEffectSO data;
    public int remainingTurns;
    [System.NonSerialized] public GameObject vfxInstance;

    public ActiveStatusEffect(StatusEffectSO effectData)
    {
        data = effectData;
        remainingTurns = effectData.duration;
    }
}