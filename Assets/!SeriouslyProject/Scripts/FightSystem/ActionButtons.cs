using EchoRift;
using FightSystem.Character;
using FightSystem.Enemy;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtons : MonoBehaviour
{
    #region Attack Fields
    [FoldoutGroup("Attack")]
    [SerializeField] private Button physicAttack;
    [FoldoutGroup("Attack")]
    [SerializeField] private GameObject physicAttackButtons;
    [FoldoutGroup("Attack")]
    [SerializeField] private Button magicAttack;
    [FoldoutGroup("Attack")]
    [SerializeField] private GameObject magicAttackButtons;
    #endregion 

    [HideInInspector] public Enemy currentEnemy;

    [SerializeField] private FightManager fightManager;
    [SerializeField] private List<ButtonsMethods> buttonsMethods;

    private Action pendingAction;

    private void Start()
    {
        magicAttack.onClick.AddListener(MagicAction);
        physicAttack.onClick.AddListener(PhysicAction);

        Initialize();
    }

    private void Initialize()
    {
        IterateButtons();
    }

    private void IterateButtons()
    {
        for (int i = 0; i < buttonsMethods.Count; i++)
        {
            var bm = buttonsMethods[i];
            if (bm.button == null || string.IsNullOrEmpty(bm.methodName)) continue;

            var methodInfo = typeof(BattleActions).GetMethod(
                bm.methodName,
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            if (methodInfo == null)
            {
                Debug.LogError($"Method '{bm.methodName}' not found in BattleActions.");
                continue;
            }

            bm.button.onClick.RemoveAllListeners();

            var localMethod = methodInfo;
            var localButton = bm.button;

            localButton.onClick.AddListener(() =>
            {
                var activeChar = fightManager.ActiveCharacter;
                if (activeChar == null)
                {
                    Debug.LogWarning("No active character (fightManager.ActiveCharacter == null).");
                    return;
                }

                if (activeChar.Mana < bm.manaCost)
                {
                    Debug.LogWarning($"{activeChar.Name} не хватает маны для {bm.methodName}. Нужно {bm.manaCost}, есть {activeChar.Mana}");
                    return;
                }

                int damage = activeChar.GiveDamage();

                pendingAction = () =>
                {
                    if (currentEnemy == null)
                    {
                        Debug.LogWarning("No target enemy selected.");
                        return;
                    }

                    var ba = new BattleActions(activeChar, damage, currentEnemy);
                    localMethod.Invoke(ba, null);

                    activeChar.Mana -= bm.manaCost;
                    activeChar.UpdateUI();

                    activeChar.IsTurn = false;
                    fightManager.DeleteEnemyOnList(currentEnemy);
                };

                if (currentEnemy != null)
                {
                    pendingAction.Invoke();
                    pendingAction = null;
                }
                else
                {
                    Debug.Log("Ожидается выбор врага...");
                }
            });

        }
    }

    public void OnEnemySelected(Enemy enemy)
    {
        currentEnemy = enemy;

        if (pendingAction != null)
        {
            pendingAction.Invoke();
            pendingAction = null;
        }
    }


    public void MagicAction()
    {
        OpenButtons(magicAttackButtons, physicAttackButtons);
    }

    public void PhysicAction()
    {
        OpenButtons(physicAttackButtons, magicAttackButtons);
    }

    private void OpenButtons(GameObject _buttons1, GameObject _buttons2)
    {
        _buttons1.SetActive(!_buttons1.activeSelf);
        _buttons2.SetActive(false);
    }

    [System.Serializable]
    public class BattleActions
    {
        private Base attacker;
        private Base target;
        private int damage;
        private int manaCost;

        public BattleActions(Base _attacker, int _damage, Base _target)
        {
            attacker = _attacker;
            damage = _damage;
            target = _target;
        }

        public BattleActions(Base _attacker, int _damage, Base _target, int _manaCost)
        {
            attacker = _attacker;
            damage = _damage;
            target = _target;
            manaCost = _manaCost;
        }

        public void SlashAttack()
        {
            Debug.Log($"{attacker.Name} бьёт {target.Name} на {damage} урона (SlashAttack)");
            target.TakeDamage(damage);
        }

        public void RudeBlow()
        {
            Debug.Log($"{attacker.Name} использует RudeBlow");
            target.TakeDamage(damage);
        }

        public void ProudPose()
        {
            Debug.Log($"{attacker.Name} делает ProudPose");

        }

        public void Parry()
        {
            Debug.Log($"{attacker.Name} парирует атаку {target.Name}");

        }



        public void MagicSlashAttack()
        {
            Debug.Log($"{attacker.Name} бьёт {target.Name} на {damage} урона (SlashAttack)");
            target.TakeMagicDamage(damage);
            attacker.Mana -= manaCost;
        }

        public void MagicRudeBlow()
        {
            Debug.Log($"{attacker.Name} использует RudeBlow");
            target.TakeMagicDamage(damage);
        }

        public void MagicProudPose()
        {
            Debug.Log($"{attacker.Name} делает ProudPose");

        }

        public void MagicParry()
        {
            Debug.Log($"{attacker.Name} парирует атаку {target.Name}");

        }

        public void EscapeFight()
        {
            Player.Result = FightResult.Escape;
            //Добавить переменную для сцены и логику
            GlobalLoader.Instance.LoadToScene("PlayerScene", Vector3.zero);
        }
    }

    [System.Serializable]
    public struct ButtonsMethods
    {
        public Button button;
        public string methodName;
        public int manaCost;
    }

}
