using UnityEngine;
using Cinemachine;

public class CameraDebugger : MonoBehaviour
{
    private CinemachineVirtualCamera _vcam;
    private bool _isTracking = false;

    private void Awake()
    {
        _vcam = GetComponent<CinemachineVirtualCamera>();
    }

    // Вызывай это из SceneLoaderTrigger перед загрузкой
    public void StartTracking() => _isTracking = true;

    // Вызывай это из SceneLoaderBridge после FadeOut
    public void StopTracking() => _isTracking = false;

    private void Update()
    {
        if (!_isTracking || _vcam == null) return;

        // Логируем позицию каждый кадр, чтобы поймать "пролет"
        if (DebugRecorder.Instance != null)
        {
            DebugRecorder.Instance.Log("CameraTick",
                $"Pos: {transform.position}, " +
                $"Follow: {(_vcam.Follow != null ? _vcam.Follow.position.ToString() : "null")}");
        }
    }
}