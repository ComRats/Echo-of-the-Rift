using System.Collections.Generic;
using FightSystem.Character;
using System.Collections;
using FightSystem.Enemy;
using UnityEngine;
using System.Linq;
using TMPro;
using UnityEngine.SceneManagement;
using EchoRift;
using Sirenix.OdinInspector;
using Zenject;

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

    [Title("Ability System")]
    [SerializeField] private AbilityManager abilityManager;
    [SerializeField] private BattleTeamSync battleTeamSync;

    [Inject] private GameSettings gameSettings;

    private int allEnemyXP;
    private int allCharacterXP;
    private int characterStartCount;
    private int enemiesStartCount;
    private int pendingEnemyDeletions = 0;

#if UNITY_EDITOR
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
#endif

    private void Start()
    {
        InitializationLists();
        Initialization();
        EquipmentManager.Instance.SetBattleState(true);
        StartCoroutine(StartFight());
    }

    private IEnumerator StartFight()
    {
        for (int i = 0; i < bases.Count; i++)
        {
            var currentBase = bases[i];
            if (currentBase == null || currentBase.Health <= 0) continue;

            currentBase.ProcessStatusEffects();
            yield return new WaitForSeconds(0.5f);

            if (currentBase.Health <= 0) continue;

            if (currentBase is Enemy enemy)
            {
                float enemyDelay = gameSettings != null ? gameSettings.enemyTurnDelay : damageDelay;
                float enemySpeed = gameSettings != null ? gameSettings.enemyTurnSpeed : 1f;

                yield return new WaitForSeconds(enemyDelay);

                Character target = GetCharacterLowestHP();

                if (target == null) break;

                enemy.SetAnimationSpeed(enemySpeed);

                target.TakeDamage(enemy.GiveDamage());
                target.PlayAnimation(enemy.AttackAnimationName);

                enemy.SetAnimationSpeed(1f);

                DeleteCharacterOnList(GetCharacterLowestHP());
            }
            else if (currentBase is Character character)
            {
                yield return StartCoroutine(WaitCharacterTurn(character));
            }
        }

        while (pendingEnemyDeletions > 0)
        {
            yield return null;
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
                basic.GetXP(allEnemyXP / characterStartCount);
                Debug.Log(basic.name + " ������� " + (allEnemyXP / characterStartCount) + " XP");
            }

            Player.Result = FightResult.Win;

            battleTeamSync.SyncTeamAfterBattle();

            EquipmentManager.Instance.SetBattleState(false);

            yield return new WaitForSecondsRealtime(1f);
            GlobalLoader.Instance.LoadToScene();
        }
        else if (enemies.All(e => e.Health > 0) && characters.All(c => c.Health == 0))
        {
            Debug.Log("You are LOSE!");

            foreach (var basic in bases)
            {
                basic.GetXP(allCharacterXP / enemiesStartCount);
                Debug.Log(basic.name + " ������� " + (allEnemyXP / enemiesStartCount) + " XP");
            }

            Player.Result = FightResult.Lose;

            battleTeamSync.SyncTeamAfterBattle();

            EquipmentManager.Instance.SetBattleState(false);

            yield return new WaitForSecondsRealtime(1f);
            GlobalLoader.Instance.LoadToScene();

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
    }

    private void Initialization()
    {
        foreach (var enemy in enemies)
        {
            allEnemyXP += enemy.XpReward;
        }
        foreach (var character in characters)
        {
            allCharacterXP += character.XpReward;
        }

        characterStartCount = characters.Count;
        enemiesStartCount = enemies.Count;
    }

    private IEnumerator WaitCharacterTurn(Character _character)
    {
        _character.IsTurn = true;
        ActiveCharacter = _character;

        if (abilityManager != null)
        {
            abilityManager.SetupAbilitiesForCharacter(_character);
        }

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
        return characters.OrderBy(character => character.Health).FirstOrDefault();
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

            pendingEnemyDeletions++;

            StartCoroutine(DeleteEnemyGameObjectWithDelay(enemy));
        }
    }

    private IEnumerator DeleteEnemyGameObjectWithDelay(Enemy enemy)
    {
        yield return new WaitForSeconds(1.5f);

        if (enemy != null)
        {
            Destroy(enemy.gameObject);
        }

        pendingEnemyDeletions--;
    }
}