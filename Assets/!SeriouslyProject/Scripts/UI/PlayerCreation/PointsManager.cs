using FightSystem.Data;
using System.Collections.Generic;
using UnityEngine;

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
    public int maxPoints = 10;
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

    #region Points control
    public bool CanAddPoint()
    {
        return usedPoints < maxPoints;
    }

    public void AddPoint()
    {
        usedPoints++;
    }

    public void RemovePoint()
    {
        usedPoints = Mathf.Max(0, usedPoints - 1);
    }
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
                    characterData.Damage = 5 + v + (int)(0.5f * v);
                    break;

                case StatType.Intellect:
                    characterData.MagicDamage = 5 + v + (int)(0.5f * v);
                    break;

                case StatType.Charisma:
                    characterData.Priority = 2 + v + (int)(0.2f * v);
                    characterData.Armor = 1 + (int)(0.4f * v);
                    break;

                case StatType.Lucky:
                    characterData.Lucky = 2 + v + (int)(0.2f * v);
                    characterData.CreteDamage = 2 + v + (int)(0.2f * v);
                    break;

                case StatType.HP:
                    characterData.Health =
                    characterData.MaxHealth = 10 + v + (int)(0.5f * v);
                    break;

                case StatType.MP:
                    characterData.Mana =
                    characterData.MaxMana = 5 + v + (int)(0.5f * v);
                    break;
            }
        }
    }
    #endregion

    #region UI description
    public string GetDescription(StatType type, int value)
    {
        return type switch
        {
            StatType.Power => $"Увеличивает физический урон на {(int)(value * 1.5f)}",
            StatType.Intellect => $"Увеличивает магический урон на {(int)(value * 1.5f)}",
            StatType.Charisma => "Повышает приоритет и броню",
            StatType.Lucky => "Увеличивает шанс и крит урон",
            StatType.HP => $"Увеличивает здоровье на {value * 2}",
            StatType.MP => $"Увеличивает ману на {value * 2}",
            _ => ""
        };
    }
    #endregion

    [System.Serializable]
    public class PointsData
    {
        public StatType statType;
        public Choosing choosing;

        public int UsedPoints => choosing.currentValue;
    }
}
