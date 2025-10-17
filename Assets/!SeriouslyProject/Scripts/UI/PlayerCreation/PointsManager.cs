using FightSystem.Data;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{
    public int maxPoints = 10;
    public int usedPoints = 0;

    [SerializeField] private List<PointsData> pointsData = new();

    private CharacterData characterData;

    private void Start()
    {
        characterData = Resources.Load<CharacterData>("CharacterData/Human");
    }

    public void AddPointsToPlayer()
    {
        if (characterData != null)
        {
            foreach (var data in pointsData)
            {
                Debug.LogError(data.usedPoints);
                switch (data.description)
                {
                    case "Power":
                        characterData.Damage = 5 + data.usedPoints + (int)(0.5f * data.usedPoints);
                        characterData.Health = 10 + data.usedPoints + (int)(0.5f * data.usedPoints);
                        characterData.MaxHealth = 10 + data.usedPoints + (int)(0.5f * data.usedPoints);
                        break;
                    case "Intellect":
                        characterData.Mana = 5 + data.usedPoints + (int)(0.5f * data.usedPoints);
                        characterData.MaxMana = 5 + data.usedPoints + (int)(0.5f * data.usedPoints);
                        //magic damage
                        break;
                    case "Agility":
                        characterData.Priority = 2 + data.usedPoints + (int)(0.2f * data.usedPoints);
                        characterData.Armor = 1 + (int)(0.4f * data.usedPoints);
                        break;
                    case "Lucky":
                        characterData.Lucky = 2 + data.usedPoints + (int)(0.2f * data.usedPoints);
                        characterData.CreteChance = 2 + data.usedPoints + (int)(0.2f * data.usedPoints);
                        break;
                    default:

                        break;
                }
            }
        }
        else
        {
            Debug.LogError("Не удалось загрузить Human.asset из папки Resources.");
        }
    }

    [System.Serializable]
    public class PointsData
    {
        public Choosing choosing;
        public string description;
        public int usedPoints => choosing.currentValue;
    }
}
