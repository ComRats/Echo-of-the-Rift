using UnityEngine;

public class TestMovement : MonoBehaviour, ISceneLoader
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
        if (canMove)
            Moving();
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
        rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;
    }

    public void UnFrezeMoving()
    {
        rigidbody.constraints = RigidbodyConstraints2D.None;
    }
}
