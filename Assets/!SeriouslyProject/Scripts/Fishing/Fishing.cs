using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EchoRift;
using Zenject;
using AudioManager.Locator;

public class Fishing : MonoBehaviour
{
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

    [Header("Рыба")]
    [SerializeField] private List<string> fishList;

    public bool IsFishing { get; private set; } = false;
    private bool isMinigameActive = false;
    private ClickBarUI clickBar;
    private KeyCode fishingKey = KeyCode.F;

    private void Start()
    {
        fishingUI = _mainUI.fishingUI;
        clickBar = fishingUI.clickBar;
        inventoryManager = _mainUI.inventoryManager;
        playerMovement = _player.movement;
        fishingKey = _settings.useButton;
    }

    public void StartFishingProcess(FishingTrigger trigger)
    {
        if (!IsFishing && playerMovement != null)
        {
            currentFishingTrigger = trigger;
            _mainUI.canOpenUI = false; // Блокируем UI во время рыбалки
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
        float waitTime = Random.Range(minWaitTime, maxWaitTime);

        // Ожидание поклевки
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

        // Поклевка!
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

            // Запускаем мини-игру
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
        isMinigameActive = true;
        bool minigameCompleted = false;
        bool minigameFailed = false;

        clickBar.Setup(minigameStartFill, minigameDrainSpeed,
            () => { minigameCompleted = true; },
            () => { minigameFailed = true; });

        // Ждём завершения мини-игры
        while (!minigameCompleted && !minigameFailed)
        {
            if (Input.GetMouseButtonDown(0))
            {
                clickBar.AddProgress(minigameClickPower);
            }
            yield return null;
        }

        isMinigameActive = false;

        if (minigameCompleted)
        {
            Debug.Log("Рыба поймана!");
            string caughtFish = CatchRandomFish();
            var itemData = inventoryManager.FindItemDataByName(caughtFish);
            fishingUI?.ShowCatchResult(itemData?.itemGameName ?? caughtFish);
            ServiceLocator.GetService().PlayOneShot("CollectItem1");
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
        if (fishList == null || fishList.Count == 0)
        {
            Debug.LogWarning("Список рыбы пуст!");
            return "Неизвестная рыба";
        }

        string randomFish = fishList[Random.Range(0, fishList.Count)];
        inventoryManager?.AddItem(randomFish);
        Debug.Log($"Поймана рыба: {randomFish}!");
        return randomFish;
    }

    public void EndFishing()
    {
        IsFishing = false;
        isMinigameActive = false;
        _mainUI.canOpenUI = true; // Разблокируем UI после рыбалки
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