using UnityEngine;

public interface IColliderDebugDrawable2D
{
    /// <summary>Возвращает целевой 2D-коллайдер (по умолчанию берётся с объекта).</summary>
    Collider2D GetCollider2D() => (this as Component)?.GetComponent<Collider2D>();

    /// <summary>Цвет линий визуализации.</summary>
    Color GizmoColor => Color.green;

    /// <summary>Показывать ли в билде (только в Development Build).</summary>
    bool ShowInDevelopmentBuild => true;

    /// <summary>Условие, нужно ли рисовать в текущем контексте.</summary>
    bool ShouldDrawGizmos()
    {
#if UNITY_EDITOR
        return true;
#else
        return Debug.isDebugBuild && ShowInDevelopmentBuild;
#endif
    }

    /// <summary>Отрисовывает границы коллайдера в редакторе и дев-билде.</summary>
    void OnDrawColliderGizmos2D()
    {
        if (!ShouldDrawGizmos())
            return;

        var col = GetCollider2D();
        if (col == null)
            return;

        Gizmos.color = GizmoColor;
        Gizmos.matrix = col.transform.localToWorldMatrix;

        switch (col)
        {
            case BoxCollider2D box:
                Gizmos.DrawWireCube(box.offset, box.size);
                break;

            case CircleCollider2D circle:
                DrawWireCircle(circle.offset, circle.radius);
                break;

            case CapsuleCollider2D capsule:
                DrawWireCapsule2D(capsule.offset, capsule.size, capsule.direction);
                break;

            case PolygonCollider2D poly:
                DrawWirePolygon(poly);
                break;
        }
    }

    private static void DrawWireCircle(Vector2 center, float radius, int segments = 32)
    {
        Vector3 prev = Vector3.zero;
        for (int i = 0; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2f / segments;
            Vector3 point = new Vector3(Mathf.Cos(angle), Mathf.Sin(angle), 0) * radius + (Vector3)center;
            if (i > 0)
                Gizmos.DrawLine(prev, point);
            prev = point;
        }
    }

    private static void DrawWireCapsule2D(Vector2 center, Vector2 size, CapsuleDirection2D direction)
    {
        float radius, height;
        if (direction == CapsuleDirection2D.Vertical)
        {
            radius = size.x / 2f;
            height = size.y - radius * 2f;
        }
        else
        {
            radius = size.y / 2f;
            height = size.x - radius * 2f;
        }

        Vector2 up = (direction == CapsuleDirection2D.Vertical) ? Vector2.up : Vector2.right;
        Vector2 right = (direction == CapsuleDirection2D.Vertical) ? Vector2.right : Vector2.up;
        Vector2 offset = up * height / 2f;

        // Прямоугольная часть
        Gizmos.DrawLine(center + right * radius + offset, center + right * radius - offset);
        Gizmos.DrawLine(center - right * radius + offset, center - right * radius - offset);

        // Круги сверху и снизу
        DrawWireCircle(center + offset, radius);
        DrawWireCircle(center - offset, radius);
    }

    private static void DrawWirePolygon(PolygonCollider2D poly)
    {
        int pathCount = poly.pathCount;
        for (int p = 0; p < pathCount; p++)
        {
            var points = poly.GetPath(p);
            if (points.Length < 2)
                continue;

            for (int i = 0; i < points.Length; i++)
            {
                Vector3 a = points[i];
                Vector3 b = points[(i + 1) % points.Length];
                Gizmos.DrawLine(a, b);
            }
        }
    }
}
