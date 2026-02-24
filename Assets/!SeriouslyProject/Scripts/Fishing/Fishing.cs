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

    [Header("Управление")]
    [SerializeField] private KeyCode fishingKey = KeyCode.F;

    [Header("Настройки")]
    [SerializeField] private float minWaitTime = 5f;
    [SerializeField] private float maxWaitTime = 15f;
    [SerializeField] private float biteWindow = 1f;

    [Header("Рыба")]
    [SerializeField] private List<string> fishList;

    public bool IsFishing { get; private set; } = false;

    private void Start()
    {
        fishingUI = _mainUI.fishingUI;
        inventoryManager = _mainUI.inventoryManager;
        playerMovement = FindObjectOfType<Movement>(); // замени на Inject player.movement
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
            Debug.Log("Рыба поймана!");
            string caughtFish = CatchRandomFish();
            fishingUI?.ShowCatchResult(caughtFish);
        }
        else
        {
            Debug.Log("Упустил!");
            fishingUI?.ShowMissed();
        }

        yield return new WaitForSeconds(2f);

        EndFishing();
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
        _mainUI.canOpenUI = true; // Разблокируем UI после рыбалки
        fishingUI?.HideText();

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