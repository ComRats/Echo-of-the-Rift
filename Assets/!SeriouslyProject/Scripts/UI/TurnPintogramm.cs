using FightSystem.Character;
using System.Collections.Generic;
using UnityEngine;

public class TurnPintogramm : MonoBehaviour
{
    [SerializeField] GameObject pintogramm;
    private FightManager fightManager;

    private List<Character> characters = new();

    private void Start()
    {
        fightManager = GetComponentInParent<FightManager>();
    }

    private void FixedUpdate()
    {
        characters = fightManager.characters;
        foreach (Character character in characters)
        {
            if (character.IsTurn)
            {
                pintogramm.transform.position = character.transform.position;
            }
        }
    }
}
