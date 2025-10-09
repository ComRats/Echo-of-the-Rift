using UnityEngine;

public class Player : MonoBehaviour
{
    public PressableButtons pressableButtons;
    public CameraSettings cameraSettings;
    public TestMovement movement;

    public static FightResult Result = FightResult.None;
    public Vector3 startPosition;

}
