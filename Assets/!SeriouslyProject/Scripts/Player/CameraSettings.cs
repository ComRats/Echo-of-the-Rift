using Cinemachine;
using EchoRift;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private string colliderTag = "cameraBorder";

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        CinemachineConfiner cam = GetComponent<CinemachineConfiner>();

        if (cam == null)
            cam = GetComponent<CinemachineConfiner>();

        if (cam == null)
        {
            Debug.LogWarning("CinemachineConfiner не найден!");
            return;
        }

        GameObject borderObj = GameObject.FindGameObjectWithTag(colliderTag);

        if (borderObj == null)
        {
            //Debug.LogWarning("Объект с тегом " + colliderTag + " не найден!");
            return;
        }

        PolygonCollider2D cameraBorder = borderObj.GetComponent<PolygonCollider2D>();
        if (cameraBorder == null)
        {
            Debug.LogError("Объект " + borderObj.name + " найден, но у него нет PolygonCollider2D!");
            return;
        }

        cam.m_BoundingShape2D = cameraBorder;
        cam.InvalidatePathCache();

        Debug.Log("Конфайнер успешно привязан к " + borderObj.name);
    }

}
