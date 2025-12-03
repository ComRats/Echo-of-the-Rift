using Cinemachine;
using EchoRift;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private string colliderTag = "cameraBorder";

    private CinemachineVirtualCamera _vCam;

    private void Awake()
    {
        _vCam = GetComponent<CinemachineVirtualCamera>();
    }

    public void Initialize()
    {
        CinemachineConfiner cam = GetComponent<CinemachineConfiner>();

        if (cam == null)
        {
            Debug.LogWarning("CinemachineConfiner не найден!");
            return;
        }

        GameObject borderObj = GameObject.FindGameObjectWithTag(colliderTag);
        if (borderObj != null)
        {
            PolygonCollider2D cameraBorder = borderObj.GetComponent<PolygonCollider2D>();
            if (cameraBorder != null)
            {
                cam.m_BoundingShape2D = cameraBorder;
                cam.InvalidatePathCache();
                Debug.Log("Конфайнер успешно привязан к " + borderObj.name);
            }
        }

        SnapCameraToTarget();
    }

    private void SnapCameraToTarget()
    {
        if (_vCam == null) _vCam = GetComponent<CinemachineVirtualCamera>();

        if (_vCam != null && _vCam.Follow != null)
        {
            _vCam.PreviousStateIsValid = false;

            Vector3 targetPos = _vCam.Follow.position;
            targetPos.z = transform.position.z;
            transform.position = targetPos;
        }
        else
        {
            Debug.LogWarning("Virtual Camera или Follow target не установлены!");
        }
    }
}