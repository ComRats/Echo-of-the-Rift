using UnityEngine;

public class Movement : MonoBehaviour, ISceneLoader
{
    [SerializeField] private float playerSpeed;

    public bool canMove = true;

    private Animator animator;
    private Vector2 direction;
    private new Rigidbody2D rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
