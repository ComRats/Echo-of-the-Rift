using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI.Extensions;

public class Star : MonoBehaviour
{
    [SerializeField] private List<Star> connectedStars;
    [SerializeField] private UILineRenderer linePrefab;

    private Canvas canvas;
    private Camera uiCam;

    private void Awake()
    {
        canvas = GetComponentInParent<Canvas>();
        uiCam = canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera;

        foreach (var star in connectedStars)
        {
            if (star == null) continue;

            var lineObj = Instantiate(linePrefab.gameObject, canvas.transform);
            lineObj.transform.SetParent(transform);
            var lr = lineObj.GetComponent<UILineRenderer>();

            var rt = lr.rectTransform;
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            rt.localPosition = Vector3.zero;
            rt.localScale = Vector3.one;

            Vector2 a = WorldToLocal(transform.position, lr.rectTransform);
            Vector2 b = WorldToLocal(star.transform.position, lr.rectTransform);

            lr.Points = new[] { a, b };
            lr.SetAllDirty();
        }
    }

    private Vector2 WorldToLocal(Vector3 worldPos, RectTransform targetRect)
    {
        Vector2 screen = RectTransformUtility.WorldToScreenPoint(uiCam, worldPos);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(targetRect, screen, uiCam, out var local);
        return local;
    }
}
