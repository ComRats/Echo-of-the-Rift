using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverHandler : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private string descriptionText;
    [SerializeField] private string descriptionText1;

    private TextMeshProUGUI description;
    private PointsManager points;
    private Choosing choosing;

    private void Start()
    {
        choosing = GetComponent<Choosing>();
        points = GetComponentInParent<PointsManager>();
        description = choosing.descriptionText;

        description.text = descriptionText + (points.maxPoints - points.usedPoints).ToString() + descriptionText1;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        choosing.UpdateUI();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        description.text = descriptionText + (points.maxPoints - points.usedPoints).ToString() + descriptionText1;
    }
}
