using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string descriptionText;
    [SerializeField] private string descriptionText1;

    private TextMeshProUGUI description;
    private PointsManager points;
    private IUpdatableUI uiHandler;

    private void Start()
    {
        uiHandler = GetComponent<IUpdatableUI>();
        points = GetComponentInParent<PointsManager>();

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
