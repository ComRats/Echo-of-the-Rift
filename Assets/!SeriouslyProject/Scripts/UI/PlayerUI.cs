using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private List<Tongue> tongues;
    private int selectedTongueIndex = 0;

    private void Start()
    {
        selectedTongueIndex = GlobalLoader.Instance.SelectedTongueIndex;
    }

    public void OpenPlayerUI()
    {
        for (int i = 0; i < tongues.Count; i++)
        {
            tongues[i].index = i;
            tongues[i].Init(OnTongueSelected);
        }

        if (tongues.Count > 0)
        {
            SelectTongue(selectedTongueIndex);
            tongues[selectedTongueIndex].PlayAnimationTrigger("Pressed");
        }
    }

    private void OnTongueSelected(int selectedIndex)
    {
        SelectTongue(selectedIndex);
        selectedTongueIndex = selectedIndex;
        GlobalLoader.Instance.SelectedTongueIndex = selectedIndex;
    }


    private void SelectTongue(int selectedIndex)
    {
        for (int i = 0; i < tongues.Count; i++)
        {
            tongues[i].IsSelected = (i == selectedIndex);
        }
    }
}
