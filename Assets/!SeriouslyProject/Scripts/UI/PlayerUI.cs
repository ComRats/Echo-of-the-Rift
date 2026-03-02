using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private List<Tongue> tongues;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject overlayPanel;

    public void OpenPlayerUI(int tongue = 0)
    {
        for (int i = 0; i < tongues.Count; i++)
        {
            tongues[i].index = i;
            tongues[i].Init(OnTongueSelected);
        }

        if (tongues.Count > 0)
        {
            SelectTongue(tongue);
        }
    }

    private void OnTongueSelected(int selectedIndex)
    {
        SelectTongue(selectedIndex);
    }

    public void ToggleInventoryOnFight()
    {
        overlayPanel.SetActive(!overlayPanel.activeSelf);
        playerPanel.SetActive(!playerPanel.activeSelf);
    }


    private void SelectTongue(int selectedIndex)
    {
        for (int i = 0; i < tongues.Count; i++)
        {
            tongues[i].IsSelected = (i == selectedIndex);
        }
    }
}
