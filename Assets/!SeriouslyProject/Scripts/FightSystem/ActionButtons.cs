using EchoRift;
using EchoRift.SaveLoadSystem;
using FightSystem.Enemy;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
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

        pendingAction = () =>
        {
            if (currentEnemy == null)
            {
                Debug.LogWarning("No target enemy selected.");
                return;
            }

            pendingAbility.Execute(pendingAttacker, currentEnemy);

            if (pendingAttacker is FightSystem.Character.Character character)
            {
                character.UpdateUI();
                character.IsTurn = false;
            }

            fightManager.DeleteEnemyOnList(currentEnemy);
            
            pendingAbility = null;
            pendingAttacker = null;
        };

        if (currentEnemy != null)
        {
            ExecutePendingAction();
        }
        else
        {
            Debug.Log("Выберите цель для способности: " + ability.AbilityName);
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