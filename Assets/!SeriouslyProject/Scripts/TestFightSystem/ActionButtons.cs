using FightSystem.Character;
using FightSystem.Enemy;
using Sirenix.OdinInspector;
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
    private List<Character> characters;
    private BattleActions battleActions;

    [SerializeField] private FightManager fightManager;
    [SerializeField] private List<ButtonsMethods> buttonsMethods;

    private void Start()
    {
        magicAttack.onClick.AddListener(MagicAction);
        physicAttack.onClick.AddListener(PhysicAction);

        Initialize();
    }

    private void Initialize()
    {
        characters = fightManager.characters;

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

            var localMethod = methodInfo;

            bm.button.onClick.AddListener(() =>
            {
                foreach (var character in characters)
                {
                    int damage = character.Damage;
                    var ba = new BattleActions(character, damage);
                    localMethod.Invoke(ba, null);
                }
            });
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
        private Base baseChar;
        private int damage;

        public BattleActions(Base _baseChar, int _damage)
        {
            baseChar = _baseChar;
            damage = _damage;
        }

        public void SlashAttack()
        {
            Debug.Log("SlashAttack");
            baseChar.TakeDamage(damage);
        }

        public void RudeBlow()
        {
            baseChar.TakeDamage(damage);

        }

        public void ProudPose()
        {
            baseChar.TakeDamage(damage);

        }

        public void Parry()
        {
            baseChar.TakeDamage(damage);

        }
    }

    [System.Serializable]
    public struct ButtonsMethods
    {
        public Button button;     
        public string methodName;
    }
}
