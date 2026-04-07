using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EchoRift;
using Zenject;
using AudioManager.Locator;

public class Fishing : MonoBehaviour
{
    [System.Serializable]
    private class LakeFishConfig
    {
        public string fishName;
        [Min(1)] public int amount = 1;
        [Range(1, 100)] public int rarity = 1;
    }

    private class RuntimeFishState
    {
        public string fishName;
        public int remaining;
        public int rarity;

        public float CatchWeight => 1f / Mathf.Max(1, rarity);
    }

    private InventoryManager inventoryManager;
    private FishingUI fishingUI;
    private Movement playerMovement;
    private FishingTrigger currentFishingTrigger;
    private Coroutine fishingCoroutine;

    [Inject] private MainUI _mainUI;
    [Inject] private Player _player;
    [Inject] private GameSettings _settings;

    [Header("Настройки")]
    [SerializeField] private float minWaitTime = 5f;
    [SerializeField] private float maxWaitTime = 15f;
    [SerializeField] private float biteWindow = 1f;

    [Header("Мини-игра")]
    [SerializeField, Range(0, 1)] private float minigameStartFill = 0.15f;
    [SerializeField] private float minigameClickPower = 0.12f;
    [SerializeField] private float minigameDrainSpeed = 0.08f;

    [Header("VFX")]
    [SerializeField] private ParticleSystem catchVFX;
    [SerializeField] private Transform[] catchVFXPoints;

    [Header("Рыба")]
    [SerializeField] private List<LakeFishConfig> lakeFishConfigs;

    [Header("Динамика ожидания")]
    [SerializeField, Min(1f)] private float waitMultiplierWhenScarce = 2f;

    public bool IsFishing { get; private set; } = false;
    private ClickBarUI clickBar;
    private KeyCode fishingKey;
    private List<RuntimeFishState> remainingFish;
    private int initialFishCount;
    private int currentFishCount;

    public int RemainingFishCount => currentFishCount;
    public bool HasFishRemaining => RemainingFishCount > 0;

    private void Start()
    {
        fishingUI = _mainUI.fishingUI;
        clickBar = fishingUI.clickBar;
        inventoryManager = _mainUI.inventoryManager;
        playerMovement = _player.movement;
        fishingKey = _settings.useButton;
        InitializeFishPool();
    }

    public void StartFishingProcess(FishingTrigger trigger)
    {
        if (!HasFishRemaining)
        {
            fishingUI?.ShowMinigameHint("В этом месте больше нет рыбы", 2f);
            return;
        }

        if (!IsFishing && playerMovement != null)
        {
            currentFishingTrigger = trigger;
            _mainUI.canOpenUI = false;
            fishingCoroutine = StartCoroutine(FishingCoroutine());
        }
    }

    private IEnumerator FishingCoroutine()
    {
        IsFishing = true;
        playerMovement.enabled = false;

        Debug.Log("Ожидание поклевки...");
        ServiceLocator.GetService().PlayOneShot("WaterSplash");
        fishingUI?.ShowWaitingForBite();

        yield return null;

        float waitTimer = 0f;
        float waitTime = CalculateWaitTime();

        while (waitTimer < waitTime)
        {
            if (Input.GetKeyDown(fishingKey))
            {
                Debug.Log("Рыбалка прервана.");
                EndFishing();
                yield break;
            }
            waitTimer += Time.deltaTime;
            yield return null;
        }

        Debug.Log("Клюёт!");
        fishingUI?.ShowBite();

        float biteTimer = 0;
        bool buttonPressed = false;

        while (biteTimer < biteWindow)
        {
            if (Input.GetKeyDown(fishingKey))
            {
                buttonPressed = true;
                break;
            }
            biteTimer += Time.deltaTime;
            yield return null;
        }

        if (buttonPressed)
        {
            Debug.Log("Рыба на крючке! Начинается мини-игра!");
            fishingUI?.ShowMinigameHint("Кликайте мышью, чтобы удержать рыбу!");

            yield return StartMinigame();
        }
        else
        {
            Debug.Log("Упустил!");
            fishingUI?.ShowMissed();
            yield return new WaitForSeconds(2f);
        }

        EndFishing();
    }

    private IEnumerator StartMinigame()
    {
        bool minigameCompleted = false;
        bool minigameFailed = false;

        clickBar.Setup(minigameStartFill, minigameDrainSpeed,
            () => { minigameCompleted = true; },
            () => { minigameFailed = true; });

        while (!minigameCompleted && !minigameFailed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickBar.AddProgress(minigameClickPower);
            }
            yield return null;
        }

        if (minigameCompleted)
        {
            Debug.Log("Рыба поймана!");
            string caughtFish = CatchRandomFish();
            var itemData = inventoryManager.FindItemDataByName(caughtFish);
            fishingUI?.ShowCatchResult(itemData?.itemGameName ?? caughtFish);
            ServiceLocator.GetService().PlayOneShot("CollectItem1");
            ServiceLocator.GetService().PlayOneShot("WaterSplash");
            PlayCatchVFX();
        }
        else
        {
            Debug.Log("Рыба сорвалась!");
            fishingUI?.ShowFishEscaped();
        }

        yield return new WaitForSeconds(2f);
    }

    private string CatchRandomFish()
    {
        if (!HasFishRemaining)
        {
            Debug.LogWarning("Список рыбы пуст!");
            return "Неизвестная рыба";
        }

        var selectedFish = GetRandomFishByRarity();
        if (selectedFish == null)
        {
            Debug.LogWarning("Не удалось выбрать рыбу для поимки.");
            return "Неизвестная рыба";
        }

        string randomFish = selectedFish.fishName;
        selectedFish.remaining = Mathf.Max(0, selectedFish.remaining - 1);
        currentFishCount = Mathf.Max(0, currentFishCount - 1);
        inventoryManager?.AddItem(randomFish);
        Debug.Log($"Поймана рыба: {randomFish}!");
        return randomFish;
    }

    private void InitializeFishPool()
    {
        remainingFish = new List<RuntimeFishState>();
        initialFishCount = 0;
        currentFishCount = 0;

        if (lakeFishConfigs == null)
        {
            return;
        }

        for (int i = 0; i < lakeFishConfigs.Count; i++)
        {
            var config = lakeFishConfigs[i];
            if (config == null || string.IsNullOrWhiteSpace(config.fishName) || config.amount <= 0)
                continue;

            remainingFish.Add(new RuntimeFishState
            {
                fishName = config.fishName,
                remaining = config.amount,
                rarity = Mathf.Max(1, config.rarity)
            });

            initialFishCount += config.amount;
        }

        currentFishCount = initialFishCount;
    }

    private RuntimeFishState GetRandomFishByRarity()
    {
        if (remainingFish == null || remainingFish.Count == 0)
            return null;

        float totalWeight = 0f;
        for (int i = 0; i < remainingFish.Count; i++)
        {
            if (remainingFish[i].remaining > 0)
                totalWeight += remainingFish[i].CatchWeight;
        }

        if (totalWeight <= 0f)
            return null;

        float roll = Random.Range(0f, totalWeight);
        float cumulative = 0f;

        for (int i = 0; i < remainingFish.Count; i++)
        {
            var fish = remainingFish[i];
            if (fish.remaining <= 0)
                continue;

            cumulative += fish.CatchWeight;
            if (roll <= cumulative)
                return fish;
        }

        for (int i = 0; i < remainingFish.Count; i++)
        {
            if (remainingFish[i].remaining > 0)
                return remainingFish[i];
        }

        return null;
    }

    private float CalculateWaitTime()
    {
        if (initialFishCount <= 0)
            return maxWaitTime;

        float remainingRatio = Mathf.Clamp01((float)RemainingFishCount / initialFishCount);
        float scarcity = 1f - remainingRatio;
        float waitMultiplier = Mathf.Lerp(1f, waitMultiplierWhenScarce, scarcity);

        float dynamicMinWait = minWaitTime * waitMultiplier;
        float dynamicMaxWait = maxWaitTime * waitMultiplier;

        return Random.Range(dynamicMinWait, dynamicMaxWait);
    }

    private void PlayCatchVFX()
    {
        if (catchVFX == null) return;

        if (catchVFXPoints != null && catchVFXPoints.Length > 0)
        {
            var point = catchVFXPoints[Random.Range(0, catchVFXPoints.Length)];
            catchVFX.transform.position = point.position;
        }

        if (!catchVFX.gameObject.activeInHierarchy)
            catchVFX.gameObject.SetActive(true);

        catchVFX.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        catchVFX.Play(true);
    }

    public void EndFishing()
    {
        IsFishing = false;
        _mainUI.canOpenUI = true;
        fishingUI?.HideText();
        clickBar?.Hide();

        if (fishingCoroutine != null)
        {
            StopCoroutine(fishingCoroutine);
            fishingCoroutine = null;
        }

        if (playerMovement != null)
            playerMovement.enabled = true;

        if (currentFishingTrigger != null)
            currentFishingTrigger.ShowButtonAfterFishing();

        Debug.Log("Рыбалка окончена.");
    }
}
