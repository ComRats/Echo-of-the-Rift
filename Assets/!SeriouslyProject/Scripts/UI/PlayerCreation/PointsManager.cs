using FightSystem.Data;
using System.Collections.Generic;
using UnityEngine;

public class PointsManager : MonoBehaviour
{//НУЖНО СДЕЛАТЬ ЧТОБЫ ПРИ НАЖАТИИ НА КНОПКУ "ДАЛЕЕ" ПРОИСХОДИЛО СОХРАНЕНИЕ СТАТОВ В ПЕРСОНАЖА
    public int maxPoints = 10;
    public int usedPoints = 0;

    [SerializeField] private List<PointsData> pointsData = new();

    public void AddPointsToPlayer()
    {
        CharacterData characterData = Resources.Load<CharacterData>("CharacterData/Human");

        if (characterData != null)
        {
            foreach (var data in pointsData)
            {
                switch (data.description)
                {
                    case "Power":
                        characterData.Damage += data.usedPoints + (int)(0.3f * data.usedPoints);
                        characterData.Health += data.usedPoints + (int)(0.5f * data.usedPoints);
                        characterData.MaxHealth += data.usedPoints + (int)(0.5f * data.usedPoints);
                        break;
                    case "Intellect":
                        characterData.Mana += data.usedPoints + (int)(0.5f * data.usedPoints);
                        characterData.MaxMana += data.usedPoints + (int)(0.5f * data.usedPoints);
                        characterData.Damage += data.usedPoints+ (int)(0.3f * data.usedPoints);
                        break;
                    case "Agility":
                        characterData.Priority += data.usedPoints + (int)(0.3f * data.usedPoints);
                        characterData.Armor += data.usedPoints + (int)(0.3f * data.usedPoints);
                        break;
                    case "Lucky":
                        characterData.Lucky += data.usedPoints + (int)(0.3f * data.usedPoints);
                        characterData.CreteChance += data.usedPoints + (int)(0.3f * data.usedPoints);
                        break;
                    default:
                        // Логика для обработки неизвестного типа
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
