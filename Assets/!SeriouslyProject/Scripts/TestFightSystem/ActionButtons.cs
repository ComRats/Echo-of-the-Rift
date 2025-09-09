using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtons : MonoBehaviour
{
    [SerializeField] private Button physicAttack;
    [SerializeField] private GameObject physicAttackButtons;
    [SerializeField] private Button magicAttack;
    [SerializeField] private GameObject magicAttackButtons;

    private void Start()
    {
        magicAttack.onClick.AddListener(MagicAction);
        physicAttack.onClick.AddListener(PhysicAction);
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
}
