using Cinemachine;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private string colliderTag = "cameraBorder";
    private CinemachineVirtualCamera _virtCam;
    private CinemachineConfiner _confiner;

    private void Awake()
    {
        _virtCam = GetComponent<CinemachineVirtualCamera>();
        _confiner = GetComponent<CinemachineConfiner>();
    }

    public void Initialize()
    {
        if (_virtCam == null || _confiner == null) return;

        GameObject borderObj = GameObject.FindGameObjectWithTag(colliderTag);
        if (borderObj != null)
        {
            _confiner.m_BoundingShape2D = borderObj.GetComponent<PolygonCollider2D>();
            _confiner.InvalidatePathCache();
        }

        transform.position = transform.parent.position;

        _virtCam.OnTargetObjectWarped(_virtCam.Follow, transform.parent.position - transform.position);
        _virtCam.ForceCameraPosition(transform.parent.position, Quaternion.identity);
        _virtCam.InternalUpdateCameraState(Vector3.up, -1);
    }
}