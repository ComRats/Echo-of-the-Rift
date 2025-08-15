using UnityEngine;

public class FishingSystem : MonoBehaviour
{
    [Header("Fishing")]
    [SerializeField] private HoodMovement fishingHood;
    [SerializeField] private FishLogic fishLogic;
    [SerializeField] private Fish fish;

    [Header("Player")]
    [SerializeField] private TestPlayer player;
    [SerializeField] private TestMovement movement;

    [Header("Bobber")]
    [SerializeField] private GameObject bobberObject;
    [SerializeField] private BobberThrow bobberThrow;
    [SerializeField] private Bobber bobber;

    [HideInInspector] public GameObject newBobber;
    [HideInInspector] public bool isDownKeyCode = false;

    private void Update()
    {
        if (Input.GetKeyDown(player.keyCode) && newBobber == null)
        {
            newBobber = Instantiate(bobberObject, transform);
            bobberThrow.SetTransformBobber(newBobber);
            bobber = newBobber.GetComponent<Bobber>();
            isDownKeyCode = true;
        }
        if (isDownKeyCode && newBobber != null)
            bobberThrow.DrawLine();
        else if(newBobber == null)
            bobberThrow.ResetLine();
    }

    public void TryGetRandomFish()
    {
        if (fishLogic.IsFishingWin)
        {
            Debug.Log($"Вы поймали рыбу: {bobber.randomFishes.name}");
            fishLogic.IsFishingWin = false;
            bobber.fishingPlace = null;
            Destroy(newBobber);
            movement.canMove = true;
            bobberThrow.ResetLine();
        }
        else if (fishLogic.IsFishingLose)
        {
            Debug.Log("Вы не поймали рыбу");
            fishLogic.IsFishingLose = false;
            bobber.fishingPlace = null;
            Destroy(newBobber);
            movement.canMove = true;
            bobberThrow.ResetLine();
        }
    }

    public void StartFishing()
    {
        ResetFishingState();
        fishLogic.StartFishing();
        fish.Initialization(bobber.randomFishes);
        fishingHood.StartFishing();
    }

    public void ResetFishingState()
    {
        fishLogic.IsFishingWin = false;
        fishLogic.IsFishingLose = false;
    }
}
