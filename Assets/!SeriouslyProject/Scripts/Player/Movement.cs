using AudioManager.Core;
using AudioManager.Locator;
using UnityEngine;

public class Movement : MonoBehaviour
{
    [SerializeField] private float playerSpeed;
    [SerializeField] private new Rigidbody2D rigidbody;
    [SerializeField] private Animator animator;

    public bool canMove = true;

    private Vector2 direction;
    private IAudioManager service;
    private int _skipFrames;

    private void OnEnable()
    {
        // Пропускаем первые 2 кадра после включения — Input ещё не готов
        _skipFrames = 2;
    }

    private void Start()
    {
        service = ServiceLocator.GetService();
    }

    private void Update()
    {
        if (_skipFrames > 0)
        {
            _skipFrames--;
            return;
        }

        if (canMove && !GameTimer.IsPaused)
            Moving();
        else
        {
            direction = Vector2.zero;
            animator.SetFloat("Speed", 0f);
        }
    }

    private void Moving()
    {
        direction.x = Input.GetAxisRaw("Horizontal");
        direction.y = Input.GetAxisRaw("Vertical");

        if (direction.sqrMagnitude > 0.01f)
        {
            direction.Normalize();
        }

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Speed", direction.sqrMagnitude);

        HandleStepSound(direction.sqrMagnitude);
    }

    private void HandleStepSound(float speed)
    {
        bool isMoving = Input.GetAxisRaw("Horizontal") != 0 || Input.GetAxisRaw("Vertical") != 0;
        
        if (speed < 0.01f || !isMoving)
        {
            service.Stop("Step");
            return;
        }

        float progress;
        service.GetProgress("Step", out progress);

        if (float.IsNaN(progress) || progress >= 0.98f || progress <= 0f)
        {
            service.Play("Step");
        }
    }

    private void FixedUpdate()
    {
        rigidbody.MovePosition(rigidbody.position + direction * playerSpeed * Time.fixedDeltaTime);
    }

    public void SetPlayerPosition(Vector3 nextPos)
    {
        gameObject.transform.position = nextPos;
    }

    public void CanMoveTrue()
    {
        canMove = true;
    }

    public void CanMoveFalse()
    {
        canMove = false;
        service?.Stop("Step");
    }
}
