using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButtons : MonoBehaviour
{
    [SerializeField] private Button magicAttack;
    [SerializeField] private Button physicAttack;


    private void Start()
    {
        magicAttack.onClick.AddListener(MagicAction);
        physicAttack.onClick.AddListener(PhysicAction);
    }

    public void MagicAction()
    {
        OpenButtons();
    }

    public void PhysicAction()
    {
        OpenButtons();
    }

    private void OpenButtons()
    {

    }
}
