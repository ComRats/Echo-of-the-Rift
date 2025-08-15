using UnityEngine;

public class AnimationFuncton : MonoBehaviour
{
    public void SetTrigger()
    {
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger("DropDown");
    }
}
