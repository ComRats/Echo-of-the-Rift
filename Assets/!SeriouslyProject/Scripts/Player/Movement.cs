using AudioManager.Core;
using AudioManager.Locator;
using UnityEngine;

public class Movement : MonoBehaviour, ISceneLoader
{
    [SerializeField] private float playerSpeed;

    public bool canMove = true;

    private Animator animator;
    private Vector2 direction;
    private new Rigidbody2D rigidbody;
    private IAudioManager service;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        service = ServiceLocator.GetService();
    }

    private void Update()
    {
        canMove = !GameTimer.IsPaused;

        if (canMove)
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

        animator.SetFloat("Horizontal", direction.x);
        animator.SetFloat("Vertical", direction.y);
        animator.SetFloat("Speed", direction.sqrMagnitude);

        HandleStepSound(direction.sqrMagnitude);
    }

    private void HandleStepSound(float speed)
    {
        if (speed < 0.01f)
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

    public void FrezeMoving()
    {
        canMove = false;
    }

    public void UnFrezeMoving()
    {
        canMove = true;
    }

    public void SetPlayerPosition(Vector3 nextPos)
    {
        gameObject.transform.position = nextPos;
    }
}
