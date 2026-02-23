using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string descriptionText;
    [SerializeField] private string descriptionText1;
    [SerializeField] private PointsManager points;

    private TextMeshProUGUI description;
    private IUpdatableUI uiHandler;

    private void Start()
    {
        uiHandler = GetComponent<IUpdatableUI>();

        if (uiHandler is Choosing)
        {
            description = uiHandler.DescriptionText;
            description.text = descriptionText + (points.maxPoints - points.usedPoints).ToString() + descriptionText1;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        uiHandler.UpdateUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (uiHandler is Choosing)
            description.text = descriptionText + (points.maxPoints - points.usedPoints).ToString() + descriptionText1;
        else
            uiHandler.UpdateUI();
    }
}
