using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SelectableTab : MonoBehaviour
{
    public int index;
    public GameObject objectToOpen;

    protected bool isSelected = false;
    protected Animator animator;
}
