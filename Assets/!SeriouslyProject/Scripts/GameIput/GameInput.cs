using UnityEngine;

public class GameInput : MonoBehaviour
{
    private PlayerInputAction playerInputActions;

    public static GameInput Instance { get; private set; }


    private void Awake()
    {
        Instance = this;
        playerInputActions = new PlayerInputAction();
        playerInputActions.Enable();
    }


    public Vector2 GetMovementVector()
    {
        Vector2 inputVector = playerInputActions.Player.Move.ReadValue<Vector2>();

        return inputVector;
    }
}
