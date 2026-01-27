using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EchoRift;

namespace Fishing2
{
    public class Fishing : MonoBehaviour
    {
        [Header("Компоненты")]
        //private Inventory playerInventory;
        private FishingUI fishingUI;
        private TestMovement playerMovement;
        private FishingTrigger currentFishingTrigger;

        //глобал лоадер.инстанс.mainUI.Inventory
        //глобал лоадер.инстанс.mainUI.FishingUI
        //глобал лоадер.инстанс.player.TestMovement


        [Header("Настройки")]
        [SerializeField] private float minWaitTime = 5f;
        [SerializeField] private float maxWaitTime = 15f;
        [SerializeField] private float biteWindow = 1f;

        //[Header("Рыба")]
        //[SerializeField] private List<Item> fishPrefabs;

        public bool IsFishing { get; private set; } = false;

        private void Start()
        {
            fishingUI = FindObjectOfType<FishingUI>();
        }

        public void StartFishingProcess(FishingTrigger trigger)
        {
            if (!IsFishing)
            {
                currentFishingTrigger = trigger;
                StartCoroutine(FishingCoroutine());
            }
        }

        private IEnumerator FishingCoroutine()
        {
            IsFishing = true;
            
            yield return null;

            if (currentFishingTrigger != null)
            {
                currentFishingTrigger.enabled = false;
            }

            if (playerMovement == null)
            {
                playerMovement = FindObjectOfType<TestMovement>();
            }
            if (playerMovement != null)
            {
                playerMovement.enabled = false;
            }

            Debug.Log("Ожидание поклевки...");
            if (fishingUI != null) fishingUI.ShowWaitingForBite();

            float waitTimer = 0f;
            float waitTime = Random.Range(minWaitTime, maxWaitTime);
            bool interrupted = false;

            while (waitTimer < waitTime)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    Debug.Log("Рыбалка прервана.");
                    interrupted = true;
                    break;
                }
                waitTimer += Time.deltaTime;
                yield return null;
            }

            if (interrupted)
            {
                EndFishing();
                yield break;
            }

            Debug.Log("Клюет!");
            if (fishingUI != null) fishingUI.ShowBite();
            GameMassage.ButtonMassage(gameObject, true, null, Vector3.zero);

            float biteTimer = 0;
            bool buttonPressed = false;
            while (biteTimer < biteWindow)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    buttonPressed = true;
                    break;
                }
                biteTimer += Time.deltaTime;
                yield return null;
            }

            if (fishingUI != null) fishingUI.HideText();
            GameMassage.ButtonMassage(gameObject, false, null, Vector3.zero);

            if (buttonPressed)
            {
                Debug.Log("Рыба поймана!");
                CatchRandomFish();
            }
            else
            {
                Debug.Log("Упустил!");
            }

            EndFishing();
        }

        private void CatchRandomFish()
        {
            // if (playerInventory == null)
            // {
            //     playerInventory = FindObjectOfType<Inventory>(true);
            //     if (playerInventory == null)
            //     {
            //         Debug.LogWarning("Инвентарь не найден. Невозможно добавить рыбу.");
            //         return;
            //     }
            // }
            
            // if (fishPrefabs != null && fishPrefabs.Count > 0)
            // {
            //     Item randomFish = fishPrefabs[Random.Range(0, fishPrefabs.Count)];
            //     playerInventory.AddItem(randomFish);
            //     Debug.Log($"Поймана рыба: {randomFish.name}!");
            // }
            // else
            // {
            //     Debug.LogWarning("Список рыб пуст. Невозможно добавить рыбу.");
            // }
        }

        public void EndFishing()
        {
            IsFishing = false;
            if (fishingUI != null) fishingUI.HideText();
            StopAllCoroutines();

            if (currentFishingTrigger != null)
            {
                currentFishingTrigger.enabled = true;
            }

            if (playerMovement != null)
            {
                playerMovement.enabled = true;
            }
            Debug.Log("Рыбалка окончена.");
        }
    }
}