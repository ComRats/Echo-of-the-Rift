using FightSystem.Character;
using System.Collections.Generic;
using UnityEngine;

public class TurnPintogramm : MonoBehaviour
{
    [SerializeField] private GameObject pintogramm;
    [SerializeField] private FightManager fightManager;

    private List<Character> characters = new();

    private void Update()
    {
        characters = fightManager.characters;

        bool anyTurn = false;
        foreach (Character character in characters)
        {
            if (character != null && character.IsTurn)
            {
                pintogramm.transform.position = character.transform.position + new Vector3(0f, character.transform.position.y / 2, 0f);
                anyTurn = true;
                break;
            }
        }

        pintogramm.SetActive(anyTurn);
    }
}
