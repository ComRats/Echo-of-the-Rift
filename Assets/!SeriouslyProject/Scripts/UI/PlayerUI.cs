using EchoRift.UI;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public MobGuide mobGuide;

    [SerializeField] private List<Tongue> tongues;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject overlayPanel;

    [SerializeField] private int questTongueIndex = 3;
    [SerializeField] private int guideTongueIndex = 2;

    public System.Action onQuestTongueSelected;
    public System.Action onQuestTongueDeselected;

    public System.Action onGuideTongueSelected;

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
        bool wasQuestSelected = tongues.Count > questTongueIndex && tongues[questTongueIndex].IsSelected;
        bool wasGuideSelected = tongues.Count > guideTongueIndex && tongues[guideTongueIndex].IsSelected;

        SelectTongue(selectedIndex);

        if (selectedIndex == questTongueIndex)
        {
            onQuestTongueSelected?.Invoke();
        }
        else if (wasQuestSelected)
        {
            onQuestTongueDeselected?.Invoke();
        }

        if (selectedIndex == guideTongueIndex)
        {
            onGuideTongueSelected?.Invoke();
        }
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
