﻿using EchoRift;
using TMPro;
using UnityEngine;
using Zenject;

public class UploadTarget : MonoBehaviour
{
    [SerializeField] private SceneLoader nextSceneLoader;
    [SerializeField] private PointsManager points;
    [SerializeField] private TextMeshProUGUI descriptionStats;
    [SerializeField] private CustomInputField inputField;

    [Header("Настройки поля имени")]
    [SerializeField] private int maxNameLength = 20;
    [SerializeField] private bool focusOnEnable = true;

    [Inject] private Player playerInstance;
    [Inject] private MainUI mainUiInstance;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        if (mainUiInstance != null)
            mainUiInstance.canOpenUI = false;
        else
        {
            GlobalLoader.Instance.mainUI.canOpenUI = false;
        }
    }

    private void OnEnable()
    {
        inputField.characterLimit = maxNameLength;
        inputField.richText = false;
        inputField.onValueChanged.AddListener(OnNameChanged);
        inputField.onSubmit.AddListener(_ => NextScene());

        if (focusOnEnable)
        {
            inputField.ActivateInputField();
            inputField.Select();
        }
    }

    private void OnDisable()
    {
        inputField.onValueChanged?.RemoveListener(OnNameChanged);
        inputField.onSubmit.RemoveListener(_ => NextScene());
    }

    private void OnNameChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
            descriptionStats.text = string.Empty;
    }

    public void NextScene()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
        {
            descriptionStats.text = "Введите имя персонажа!";
            inputField.ActivateInputField();
            return;
        }

        if (points.usedPoints >= points.maxPoints)
        {
            points.AddPointsToPlayer();
            descriptionStats.text = "Загрузка...";

            GlobalLoader.Instance.RefreshPlayerDataFromCharacterData();

            RestoreValues();

            playerInstance.dialogActor.SaveNameForDialogueActor(inputField.text);
            playerInstance.dialogActor.ApplyName();
            nextSceneLoader._onSceneActivated.AddListener(() => 
            {
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
