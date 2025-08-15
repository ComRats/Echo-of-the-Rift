using UnityEngine;
using UnityEngine.UI;

public class HoodMovement : MonoBehaviour
{
    [Header("HoodProgress")]
    [SerializeField] private Image imageProgress;
    [SerializeField] public float progress;
    [SerializeField] private float speedOfProgress;
    [SerializeField] private float hoodSpeed = 10f;

    private Rigidbody2D rigidBody;
    private bool isTriggered = false;

    public void StartFishing()
    {
        rigidBody = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            Moving();
        }
        if (!isTriggered)
        {
            ProgressMinus();
        }
    }

    private void Moving()
    {
        rigidBody.AddForce(Vector2.up * hoodSpeed, ForceMode2D.Force);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Fish"))
        {
            isTriggered = true;
            ProgressAdd();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Fish"))
        {
            isTriggered = false;
        }
    }

    private void ProgressAdd()
    {
        if (progress < 1)
        {
            progress += speedOfProgress * speedOfProgress / 10 * Time.deltaTime;
            imageProgress.fillAmount = progress;
        }
    }
    private void ProgressMinus()
    {
        if (progress > 0)
        {
            progress -= speedOfProgress * speedOfProgress / 6 * Time.deltaTime;
            imageProgress.fillAmount = progress;
        }
    }
}
