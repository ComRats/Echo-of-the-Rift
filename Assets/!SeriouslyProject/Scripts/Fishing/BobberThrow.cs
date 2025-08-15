using UnityEngine;

public class BobberThrow : MonoBehaviour
{
    private Transform attachPoint;
    private LineRenderer lineRenderer;

    private void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    public void SetTransformBobber(GameObject _gameObject)
    {
        attachPoint = _gameObject.GetComponent<Transform>();
    }

    public void DrawLine()
    {
        lineRenderer.SetPosition(0, attachPoint.position);
        lineRenderer.SetPosition(1, transform.position);
    }

    public void ResetLine()
    {
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, transform.position);
    }

}
