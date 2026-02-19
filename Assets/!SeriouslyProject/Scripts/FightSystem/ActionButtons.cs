using EchoRift;
using EchoRift.SaveLoadSystem;
using FightSystem.Enemy;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static GlobalLoader;

public class ActionButtons : MonoBehaviour
{
    #region UI References
    [FoldoutGroup("UI")][SerializeField] private GameObject physicAttackButtons;
    [FoldoutGroup("UI")][SerializeField] private GameObject magicAttackButtons;
    [FoldoutGroup("UI")][SerializeField] private Button physicAttackToggleBtn;
    [FoldoutGroup("UI")][SerializeField] private Button magicAttackToggleBtn;
    #endregion 

    [Title("UI Feedback")]
    [SerializeField] private TextMeshProUGUI descriptionText;

    [HideInInspector] public Enemy currentEnemy;

    [SerializeField] private FightManager fightManager;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private AbilityManager abilityManager;
    [Inject] private MainUI mainUI;

    private Action pendingAction;
    private BattleAbility pendingAbility;
    private Base pendingAttacker;

    private void Start()
    {
        magicAttackToggleBtn.onClick.AddListener(() => OpenButtons(magicAttackButtons, physicAttackButtons));
        physicAttackToggleBtn.onClick.AddListener(() => OpenButtons(physicAttackButtons, magicAttackButtons));
    }

    public void OnEnemySelected(Enemy enemy)
    {
        currentEnemy = enemy;
        ExecutePendingAction();
    }

    /// <summary>
    /// Метод для установки ожидающей способности из AbilityManager
    /// </summary>
    public void SetPendingAbility(BattleAbility ability, Base attacker)
    {
        if (!ability.CanUse(attacker))
        {
            return;
        }

        pendingAbility = ability;
        pendingAttacker = attacker;

        // Проверяем тип цели способности
        switch (ability.targetType)
        {
            case TargetType.Self:
                // Применяем способность на себя сразу
                ExecuteAbilityOnTarget(attacker);
                break;

            case TargetType.Ally:
                // Нужно выбрать союзника (включая себя)
                Debug.Log("Выберите союзника для способности: " + ability.AbilityName);
                StartCharacterSelection();
                break;

            case TargetType.Enemy:
                // Нужно выбрать врага
                pendingAction = () =>
                {
                    if (currentEnemy == null)
                    {
                        Debug.LogWarning("No target enemy selected.");
                        return;
                    }

                    ExecuteAbilityOnTarget(currentEnemy);
                };

                if (currentEnemy != null)
                {
                    ExecutePendingAction();
                }
                else
                {
                    Debug.Log("Выберите врага для способности: " + ability.AbilityName);
                }
                break;

            case TargetType.AllEnemies:
                // Применяем на всех врагов
                ExecuteAbilityOnAllEnemies();
                break;

            case TargetType.AllAllies:
                // Применяем на всех союзников
                ExecuteAbilityOnAllAllies();
                break;
        }
    }

    private void ExecuteAbilityOnTarget(Base target)
    {
        if (pendingAbility == null || pendingAttacker == null)
        {
            return;
        }

        pendingAbility.Execute(pendingAttacker, target);

        if (pendingAttacker is FightSystem.Character.Character character)
        {
            character.UpdateUI();
            character.IsTurn = false;
        }

        // Удаляем врага из списка, если он умер
        if (target is Enemy enemy)
        {
            fightManager.DeleteEnemyOnList(enemy);
        }

        pendingAbility = null;
        pendingAttacker = null;
    }

    private void ExecuteAbilityOnAllEnemies()
    {
        if (pendingAbility == null || pendingAttacker == null)
        {
            return;
        }

        foreach (var enemy in fightManager.enemies.ToList())
        {
            pendingAbility.Execute(pendingAttacker, enemy);
            fightManager.DeleteEnemyOnList(enemy);
        }

        if (pendingAttacker is FightSystem.Character.Character character)
        {
            character.UpdateUI();
            character.IsTurn = false;
        }

        pendingAbility = null;
        pendingAttacker = null;
    }

    private void ExecuteAbilityOnAllAllies()
    {
        if (pendingAbility == null || pendingAttacker == null)
        {
            return;
        }

        foreach (var ally in fightManager.characters)
        {
            pendingAbility.Execute(pendingAttacker, ally);
        }

        if (pendingAttacker is FightSystem.Character.Character character)
        {
            character.UpdateUI();
            character.IsTurn = false;
        }

        pendingAbility = null;
        pendingAttacker = null;
    }

    private void StartCharacterSelection()
    {
        // Включаем подсветку персонажей для выбора
        foreach (var character in fightManager.characters)
        {
            character.IsBlinking = true;
        }
        fightManager.StopEnemyBlinking();
    }

    public void OnCharacterSelected(FightSystem.Character.Character character)
    {
        if (pendingAbility != null && pendingAttacker != null)
        {
            ExecuteAbilityOnTarget(character);
            
            // Выключаем подсветку персонажей
            foreach (var c in fightManager.characters)
            {
                c.IsBlinking = false;
            }
        }
    }

    private void ExecutePendingAction()
    {
        if (pendingAction != null)
        {
            pendingAction.Invoke();
            pendingAction = null;
        }
    }

    private void OpenButtons(GameObject toOpen, GameObject toClose)
    {
        toOpen.SetActive(!toOpen.activeSelf);
        toClose.SetActive(false);
    }

    public void EscapeFight()
    {
        Player.Result = FightResult.Escape;
        var data = SaveLoadSystem.Load<GlobalData>("globalSave", GAME_DIRECTORY);
        sceneLoader.LoadAsync(data.SceneIndex);
    }

    public void OpenInventory()
    {
        mainUI.playerUI.enabled = !mainUI.playerUI.enabled;
        mainUI.canvas.enabled = !mainUI.canvas.enabled;
        mainUI.playerUI.ToggleInventoryOnFight();
    }
}