using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif

[ExecuteAlways]
public class ContentScaler : MonoBehaviour
{
    [Title("Ссылки")]
    [SerializeField, Required] private RectTransform content;
    [SerializeField, Required] private RectTransform layoutContainer;

    [Title("Отладка")]
    [SerializeField, ReadOnly] private float spacing;
    [SerializeField, ReadOnly] private int elementCount;
    [SerializeField, ReadOnly] private float totalHeight;

    private int lastChildCount = -1;

    private void LateUpdate()
    {
        if (Application.isPlaying)
        {
            if (layoutContainer.childCount != lastChildCount)
            {
                UpdateContentHeight();
                lastChildCount = layoutContainer.childCount;
            }
        }
    }

    [Button("Обновить высоту контента")]
    public void UpdateContentHeight()
    {
        VerticalLayoutGroup layoutGroup = layoutContainer.GetComponent<VerticalLayoutGroup>();

        spacing = layoutGroup.spacing;
        totalHeight = 0f;
        elementCount = 0;

        foreach (RectTransform child in layoutContainer)
        {
            if (child.gameObject.activeSelf)
            {
                totalHeight += child.rect.height;
                elementCount++;
            }
        }

        if (elementCount > 1)
            totalHeight += spacing * (elementCount - 1);

        totalHeight += layoutGroup.padding.top + layoutGroup.padding.bottom;

        Vector2 size = content.sizeDelta;
        size.y = totalHeight;
        content.sizeDelta = size;

#if UNITY_EDITOR
        if (!Application.isPlaying)
            EditorUtility.SetDirty(content);
#endif
    }

#if UNITY_EDITOR
    private void OnValidate()
    {
        UpdateContentHeight();
    }
#endif
}
