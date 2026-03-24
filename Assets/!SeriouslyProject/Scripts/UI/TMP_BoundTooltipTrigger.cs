using UnityEngine;
using UnityEngine.EventSystems;

namespace CustomUI.Tooltips
{
    [AddComponentMenu("Custom UI/Tooltips/TMP Bound Tooltip Trigger")]
    public class TMP_BoundTooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler
    {
        [TextAreaAttribute]
        public string text;
        public bool useMousePosition = false;
        public Vector3 offset;

        public void OnPointerEnter(PointerEventData eventData)
        {
            if (useMousePosition)
                StartHover(new Vector3(eventData.position.x, eventData.position.y, 0f));
            else
                StartHover(transform.position + offset);
        }

        public void OnSelect(BaseEventData eventData)
        {
            StartHover(transform.position);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StopHover();
        }

        public void OnDeselect(BaseEventData eventData)
        {
            StopHover();
        }

        void StartHover(Vector3 position)
        {
            if (TMP_BoundTooltipItem.Instance != null)
                TMP_BoundTooltipItem.Instance.ShowTooltip(text, position);
            else
                Debug.LogError("Не найден TMP_BoundTooltipItem на сцене!");
        }

        void StopHover()
        {
            if (TMP_BoundTooltipItem.Instance != null)
                TMP_BoundTooltipItem.Instance.HideTooltip();
        }
    }
}