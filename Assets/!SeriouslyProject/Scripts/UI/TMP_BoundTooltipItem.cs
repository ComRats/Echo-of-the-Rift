using UnityEngine;
using TMPro;

namespace CustomUI.Tooltips
{
    [AddComponentMenu("Custom UI/Tooltips/TMP Bound Tooltip Item")]
    public class TMP_BoundTooltipItem : MonoBehaviour
    {
        public bool IsActive => gameObject.activeSelf;

        public TextMeshProUGUI TooltipText;
        public Vector3 ToolTipOffset;

        private static TMP_BoundTooltipItem instance;
        public static TMP_BoundTooltipItem Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = FindFirstObjectByType<TMP_BoundTooltipItem>(FindObjectsInactive.Include);
                }
                return instance;
            }
        }

        void Awake()
        {
            instance = this;
            if (!TooltipText) TooltipText = GetComponentInChildren<TextMeshProUGUI>();
            HideTooltip();
        }

        public void ShowTooltip(string text, Vector3 pos)
        {
            if (TooltipText != null && TooltipText.text != text)
                TooltipText.text = text;

            transform.position = pos + ToolTipOffset;
            gameObject.SetActive(true);
        }

        public void HideTooltip()
        {
            gameObject.SetActive(false);
        }
    }
}