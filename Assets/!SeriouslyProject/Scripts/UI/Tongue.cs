using UnityEngine;
using UnityEngine.UI;

public class Tongue : SelectableTab
{
    public bool IsSelected
    {
        get => isSelected;
        set
        {
            if (isSelected == value) return;

            isSelected = value;
            objectToOpen.SetActive(isSelected);

            if (isSelected)
                PlayAnimationTrigger("Pressed");
            else
                PlayAnimationTrigger("Normal");
        }
    }

    public void Init(System.Action<int> onClickCallback)
    {
        Button button = GetComponent<Button>();
        animator = GetComponent<Animator>();

        button.onClick.AddListener(() => onClickCallback(index));
    }

    public void PlayAnimationTrigger(string animation)
    {
        animator.SetTrigger(animation);
    }
}
