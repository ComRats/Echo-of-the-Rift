using System.Collections.Generic;
using FightSystem.Character;
using System.Collections;
using FightSystem.Enemy;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;

public class FightManager : MonoBehaviour
{
    public Character ActiveCharacter { get; private set; }

    [SerializeField] private float damageDelay = 1;
    [SerializeField] private TextMeshProUGUI fightTurn;
    [SerializeField] private ContextMenu contextMenu;
    [SerializeField] private ContextText contextText;

    public List<Enemy> enemies = new();
    public List<Character> characters = new();

    [SerializeField] private List<Base> bases = new();

    private int allEnemyXP;

    //WARNING DELETE!!!!!!!!
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void Start()
    {
        InitializationLists();

        StartCoroutine(StartFight());
    }

    private IEnumerator StartFight()
    {
        for (int i = 0; i < bases.Count; i++)
        {
            if (bases[i] is Enemy enemy && enemy.Health > 0)
            {
                yield return new WaitForSeconds(damageDelay);
                GetCharacterLowestHP().TakeDamage(enemy.Damage);

                DeleteCharacterOnList(GetCharacterLowestHP());
            }
            else if (bases[i] is Character character && character.Health > 0)
            {
                yield return StartCoroutine(WaitCharacterTurn(character));
            }
        }
        contextText.ChangeTurnText();
        yield return StartCoroutine(EndFight());
    }

    private IEnumerator EndFight()
    {
        if (characters.All(c => c.Health > 0) && enemies.All(e => e.Health == 0))
        {
            Debug.Log("You are WiN!");
            
            foreach (var basic in bases)
            {
                basic.GetXP(allEnemyXP / characters.Count);
                Debug.Log(basic.name + " получил " + (allEnemyXP / characters.Count) + " XP");
            }
        }
        else if (enemies.All(e => e.Health > 0) && characters.All(c => c.Health == 0))
        {
            Debug.Log("You are LOSE!");

            foreach (var basic in bases)
            {
                basic.GetXP(allEnemyXP / enemies.Count);
                Debug.Log(basic.name + " получил " + (allEnemyXP / characters.Count) + " XP");
            }
        }
        else if (enemies.All(e => e.Health > 0) && characters.All(c => c.Health > 0))
        {
            yield return StartCoroutine(StartFight());
        }
    }

    private void ContinueFight()
    {
    }

    private void InitializationLists()
    {
        enemies.AddRange(GetComponentsInChildren<Enemy>());
        characters.AddRange(GetComponentsInChildren<Character>());

        enemies = enemies.OrderByDescending(enemy => enemy.Priority).ToList();
        characters = characters.OrderByDescending(character => character.Priority).ToList();

        bases = enemies
            .Cast<Base>()
            .Concat(characters.Cast<Base>())
            .OrderByDescending(item => item.Priority)
            .ToList();

        foreach (var enemy in enemies)
        {
            allEnemyXP += enemy.XpReward;
        }
    }

    private IEnumerator WaitCharacterTurn(Character _character)
    {
        _character.IsTurn = true;
        ActiveCharacter = _character;

        StartEnemyBlinking();

        while (_character.IsTurn)
        {
            yield return null;
        }

        ActiveCharacter = null;
        StopEnemyBlinking();
    }

    public Character GetCharacterLowestHP()
    {
        return characters.OrderBy(character => character.Health).FirstOrDefault(); ;
    }

    private Enemy GetEnemyHighestPriority()
    {
        return enemies.OrderByDescending(enemy => enemy.Priority).FirstOrDefault();
    }

    public void StopEnemyBlinking()
    {
        enemies.ForEach(enemy => enemy.IsBlinking = false);
    }

    public void StartEnemyBlinking()
    {
        enemies.ForEach(enemy => enemy.IsBlinking = true);
    }

    private void DeleteCharacterOnList(Character character)
    {
        if (character.Health <= 0)
        {
            bases.Remove(character);
            characters.Remove(character);
            Destroy(character.gameObject);
        }
    }

    public void DeleteEnemyOnList(Enemy enemy)
    {
        if (enemy.Health <= 0)
        {
            enemies.Remove(enemy);
            bases.Remove(enemy);
            Destroy(enemy.gameObject);
        }
    }
}