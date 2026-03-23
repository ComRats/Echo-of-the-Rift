using FightSystem.Data;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;

public enum StatType
{
    Power,
    Intellect,
    Charisma,
    Lucky,
    HP,
    MP
}

public class PointsManager : MonoBehaviour
{
    [Header("Points")]
    public int maxPoints = 15;
    public int usedPoints = 0;

    [Header("Stats")]
    [SerializeField] private List<PointsData> pointsData = new();

    private CharacterData characterData;

    private void Start()
    {
        characterData = Resources.Load<CharacterData>("CharacterData/Human");

        if (characterData == null)
            Debug.LogError("Не удалось загрузить CharacterData/Human");
    }

    private int Calc(int baseValue, int v, float multiplier)
    {
        return Mathf.RoundToInt(baseValue + multiplier * v);
    }

    #region Points control
    public bool CanAddPoint() => usedPoints < maxPoints;
    public void AddPoint() => usedPoints++;
    public void RemovePoint() => usedPoints = Mathf.Max(0, usedPoints - 1);
    #endregion

    #region Apply stats
    public void AddPointsToPlayer()
    {
        if (characterData == null) return;

        foreach (var data in pointsData)
        {
            int v = data.UsedPoints;

            switch (data.statType)
            {
                case StatType.Power:
                    characterData.Damage = Calc(data.baseValue, v, data.multiplier);
                    break;

                case StatType.Intellect:
                    characterData.MagicDamage = Calc(data.baseValue, v, data.multiplier);
                    characterData.Heal = Calc(data.baseValue2, v, data.multiplier);
                    break;

                case StatType.Charisma:
                    characterData.Priority = Calc(data.baseValue, v, data.multiplier);
                    characterData.Armor    = Calc(data.baseValue2, v, data.multiplier2);
                    break;

                case StatType.Lucky:
                    characterData.Lucky      = Calc(data.baseValue, v, data.multiplier);
                    characterData.CreteDamage = Calc(data.baseValue2, v, data.multiplier2);
                    break;

                case StatType.HP:
                    characterData.Health =
                    characterData.MaxHealth = Calc(data.baseValue, v, data.multiplier);
                    break;

                case StatType.MP:
                    characterData.Mana =
                    characterData.MaxMana = Calc(data.baseValue, v, data.multiplier);
                    break;
            }
        }
    }
    #endregion

    #region UI description
    public string GetDescription(StatType type, int value)
    {
        var data = pointsData.Find(d => d.statType == type);
        if (data == null) return "";

        return type switch
        {
            StatType.Power =>     $"Физический урон: {Calc(data.baseValue, value, data.multiplier)}",
            StatType.Intellect => $"Магический урон: {Calc(data.baseValue, value, data.multiplier)}, Сила эффектов: {Calc(data.baseValue2, value, data.multiplier)}",
            StatType.Charisma =>  $"Приоритет хода: {Calc(data.baseValue, value, data.multiplier)}, Защита: {Calc(data.baseValue2, value, data.multiplier2)} %",
            StatType.Lucky =>     $"Крит шанс: {Calc(data.baseValue, value, data.multiplier)}, Крит урон: {Calc(data.baseValue2, value, data.multiplier2)}",
            StatType.HP =>        $"Здоровье: {Calc(data.baseValue, value, data.multiplier)}",
            StatType.MP =>        $"Мана: {Calc(data.baseValue, value, data.multiplier)}",
            _ => ""
        };
    }
    #endregion

    [System.Serializable]
    public class PointsData
    {
        public StatType statType;
        public Choosing choosing;

        [Tooltip("Базовое значение при 0 очков")]
        public int baseValue;
        [Tooltip("Множитель за каждое очко (дробный)")]
        public float multiplier;

        [Tooltip("Базовое значение второго параметра (Charisma=Armor, Lucky=CreteDamage)")]
        public int baseValue2;
        [Tooltip("Множитель второго параметра (дробный)")]
        public float multiplier2;

        public int UsedPoints => choosing.currentValue;
    }
}
