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

    [Title("Abilities Configuration")]
    [SerializeField] private List<AbilityBinding> abilityBindings;

    [HideInInspector] public Enemy currentEnemy;

    [SerializeField] private FightManager fightManager;
    [SerializeField] private SceneLoader sceneLoader;
    [Inject] private MainUI mainUI;

    private Action pendingAction;
    //добавить динамическое создание кнопок по листу кнопок и абилок
    //хил и защита отдельные классы

    private void Start()
    {
        magicAttackToggleBtn.onClick.AddListener(() => OpenButtons(magicAttackButtons, physicAttackButtons));
        physicAttackToggleBtn.onClick.AddListener(() => OpenButtons(physicAttackButtons, magicAttackButtons));

        InitializeButtons();
    }

    private void InitializeButtons()
    {
        foreach (var binding in abilityBindings)
        {
            if (binding.button == null || binding.ability == null) continue;

            TextMeshProUGUI buttonText = binding.button.GetComponentInChildren<TextMeshProUGUI>();
            buttonText.text = binding.ability.AbilityName;

            binding.button.onClick.RemoveAllListeners();

            var localAbility = binding.ability;
            binding.button.onClick.AddListener(() => OnAbilityClicked(localAbility));
        }
    }

    private void OnAbilityClicked(BattleAbility ability)
    {
        var activeChar = fightManager.ActiveCharacter;

        if (activeChar == null) return;

        if (!ability.CanUse(activeChar))
        {
            return;
        }

        pendingAction = () =>
        {
            if (currentEnemy == null)
            {
                Debug.LogWarning("No target enemy selected.");
                return;
            }

            ability.Execute(activeChar, currentEnemy);

            activeChar.UpdateUI();
            activeChar.IsTurn = false;

            fightManager.DeleteEnemyOnList(currentEnemy);
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

    public void OnEnemySelected(Enemy enemy)
    {
        currentEnemy = enemy;
        ExecutePendingAction();
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

    [System.Serializable]
    public struct AbilityBinding
    {
        public Button button;
        public BattleAbility ability;
    }
}