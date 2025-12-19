using Cinemachine;
using EchoRift;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private string colliderTag = "cameraBorder";

    private void OnEnable()
    {
        Initialize();
    }

    public void Initialize()
    {
        //Debug.LogWarning("CameraInit");
        CinemachineConfiner cam = GetComponent<CinemachineConfiner>();
        CinemachineVirtualCamera virtCam = GetComponent<CinemachineVirtualCamera>();

        if (cam == null && virtCam != null)
        {
            Debug.LogWarning("CinemachineConfiner не найден!");
            Debug.LogWarning("CinemachineVirtualCamera не найден!");
            return;
        }

        virtCam.ForceCameraPosition(transform.parent.position, Quaternion.identity);
        cam.InvalidatePathCache();

        GameObject borderObj = GameObject.FindGameObjectWithTag(colliderTag);
        if (borderObj != null)
        {
            PolygonCollider2D cameraBorder = borderObj.GetComponent<PolygonCollider2D>();
            if (cameraBorder != null)
            {
                cam.m_BoundingShape2D = cameraBorder;
                cam.InvalidatePathCache();
                //Debug.Log("Конфайнер успешно привязан к " + borderObj.name);
            }
        }
    }
}