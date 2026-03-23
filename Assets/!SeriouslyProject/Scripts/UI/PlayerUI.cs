using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    [SerializeField] private List<Tongue> tongues;
    [SerializeField] private GameObject playerPanel;
    [SerializeField] private GameObject overlayPanel;

    // Индекс язычка квестов (соответствует позиции в списке tongues)
    [SerializeField] private int questTongueIndex = 3;

    public System.Action onQuestTongueSelected;
    public System.Action onQuestTongueDeselected;

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

        SelectTongue(selectedIndex);

        if (selectedIndex == questTongueIndex)
        {
            onQuestTongueSelected?.Invoke();
        }
        else if (wasQuestSelected)
        {
            onQuestTongueDeselected?.Invoke();
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
