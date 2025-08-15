using System.Collections.Generic;
using UnityEngine;

public class Bobber : MonoBehaviour
{
    [SerializeField] private FishingSystem fishingSystem;
    [SerializeField] private TestMovement movement;

    [HideInInspector] public FishingPlace fishingPlace;
    [HideInInspector] public List<FishData> fishes;
    [HideInInspector] public FishData randomFishes;

    private new Rigidbody2D rigidbody;
    private BobberThrow bobberThrow;

    private void Start()
    {
        movement = GetComponentInParent<TestMovement>();
        rigidbody = GetComponent<Rigidbody2D>();
        bobberThrow = FindFirstObjectByType<BobberThrow>();
        fishingSystem = FindFirstObjectByType<FishingSystem>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out FishingPlace _fishingPlace))
        {
            movement.canMove = false;
            rigidbody.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezePositionY;

            fishes = _fishingPlace.fishes;

            randomFishes = GetRandomFish(fishes);

            fishingSystem.StartFishing();
            fishingSystem.isDownKeyCode = false;
        }
    }

    private void Update()
    {
        if (IsOutOfScreen())
        {
            fishingSystem.newBobber = null;
            Destroy(gameObject);
        }
    }

    private bool IsOutOfScreen()
    {
        Vector3 screenPoint = Camera.main.WorldToViewportPoint(transform.position);
        return screenPoint.x < 0 || screenPoint.x > 1 || screenPoint.y < 0 || screenPoint.y > 1;
    }

    private FishData GetRandomFish(List<FishData> _fishes)
    {
        int rndFish = Random.Range(0, _fishes.Count);
        return _fishes[rndFish];
    }

}
