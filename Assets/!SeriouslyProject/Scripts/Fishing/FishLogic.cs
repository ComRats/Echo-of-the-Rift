using UnityEngine;

public class FishLogic : MonoBehaviour
{
    [Header("Fish")]
    [SerializeField] private Fish fish;
    [SerializeField] private RectTransform fishTransform;
    [Header("Fishing")]
    [SerializeField] private GameObject objToAcive;
    [Header("MovingBorder")]
    [SerializeField] private RectTransform maxY;
    [SerializeField] private RectTransform minY;
    [Header("GetHoodProgress")]
    [SerializeField] private HoodMovement hoodMovement;
    [SerializeField] private FishingSystem fishingSystem;

    public bool IsFishingWin { get; set; } = false;
    public bool IsFishingLose { get; set; } = false;

    private float randomPosition;
    private float progress;
    private bool isFishingActive = false;

    public void StartFishing()
    {
        Initialization();
        isFishingActive = true;
    }

    private void Update()
    {
        if (isFishingActive)
        {
            MoveFish();
            TryEndFishing();
        }
    }

    private void MoveFish()
    {
        if (Mathf.Abs(fishTransform.position.y - randomPosition) > 0.01f)
        {
            Vector3 position = fishTransform.position;
            position.y = Mathf.Lerp(position.y, randomPosition, Time.deltaTime * fish.speed);
            position.y = Mathf.Clamp(position.y, minY.position.y, maxY.position.y);
            fishTransform.position = position;
        }
        else
        {           
            SetRandomPosition();
        }
    }

    private void TryEndFishing()
    {
        progress = hoodMovement.progress;

        if (progress >= 1f)
        {
            IsFishingWin = true;
            EndFishing();
        }
        if (progress <= 0.05f)
        {
            IsFishingLose = true;
            EndFishing();
        }
    }

    private void EndFishing()
    {
        hoodMovement.progress = 0.4f;
        isFishingActive = false;
        fishingSystem.TryGetRandomFish();
        objToAcive.SetActive(false);
    }

    private void SetRandomPosition()
    {
        float minYValue = minY.position.y;
        float maxYValue = maxY.position.y;

        randomPosition = Random.Range(minYValue, maxYValue);
    }

    private void Initialization()
    {
        progress = hoodMovement.progress;
        objToAcive.SetActive(true);
        SetRandomPosition();
    }
}