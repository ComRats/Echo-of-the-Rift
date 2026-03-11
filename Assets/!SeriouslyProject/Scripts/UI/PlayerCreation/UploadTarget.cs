﻿using EchoRift;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

public class UploadTarget : MonoBehaviour
{
    [SerializeField] private SceneLoader nextSceneLoader;
    [SerializeField] private PointsManager points;
    [SerializeField] private TextMeshProUGUI descriptionStats;
    [SerializeField] private TMP_InputField inputField;

    [Inject] private Player playerInstance;
    [Inject] private MainUI mainUiInstance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public void NextScene()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            descriptionStats.text = "Введите имя персонажа!";
            return;
        }

        if (points.usedPoints >= points.maxPoints)
        {
            points.AddPointsToPlayer();
            descriptionStats.text = "Загрузка...";

            RestoreValues();

            playerInstance.dialogActor.SaveNameForDialogueActor(inputField.text);
            playerInstance.dialogActor.ApplyName();
            nextSceneLoader._onSceneActivated.AddListener(() => 
            {
                Debug.Log("OnSceneActivated");
                FindObjectOfType<TimeLineLogic>().StartConversationDelay();
                playerInstance.movement.CanMoveTrue();
            });

            nextSceneLoader.LoadAsync();
        }
        else
        {
            descriptionStats.text = $"Распределите оставшиеся очки: ({points.maxPoints - points.usedPoints})";
        }
    }


    private void RestoreValues()
    {
        playerInstance ??= Object.FindObjectOfType<Player>(true);
        mainUiInstance ??= Object.FindObjectOfType<MainUI>(true);

        if (playerInstance != null)
            playerInstance.movement.canMove = true;

        if (mainUiInstance != null)
        {
            mainUiInstance.canOpenUI = true;
            mainUiInstance.gameObject.SetActive(true);
        }

        GameTimer.ResumeGame();
    }
}
