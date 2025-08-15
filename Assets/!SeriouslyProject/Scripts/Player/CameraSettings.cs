using Cinemachine;
using UnityEngine;

public class CameraSettings : MonoBehaviour
{
    [SerializeField] private string colliderTag = "cameraBorder";

    private void Start()
    {
        CinemachineConfiner cam = GetComponent<CinemachineConfiner>();
        PolygonCollider2D cameraBorder = GameObject.FindGameObjectWithTag(colliderTag).GetComponent<PolygonCollider2D>();
        cam.m_BoundingShape2D = cameraBorder;
    }
}
