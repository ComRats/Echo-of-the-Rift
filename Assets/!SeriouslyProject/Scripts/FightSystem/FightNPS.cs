using EchoRift;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public struct QuestChange
{
    public string questCode;
    public QuestState questState;
}

public enum FightResult
{
    None,
    Win,
    Lose,
    Escape
}

public class FightNPS : MonoBehaviour
{
    [Header("Quest Settings")]
    [SerializeField] private QuestChange[] quests;

    [SerializeField] private UnityEvent onFightWin;
    [SerializeField] private UnityEvent onFightLose;
    [SerializeField] private UnityEvent onFightEscape;

    private void Start()
    {
        switch (Player.Result)
        {
            case FightResult.Win:
                onFightWin?.Invoke();
                break;

            case FightResult.Lose:
                onFightLose?.Invoke();
                break;

            case FightResult.Escape:
                onFightEscape?.Invoke();
                break;
        }

        Player.Result = FightResult.None;
    }

    public void ApplyQuestChanges()
    {
        foreach (var quest in quests)
        {
            QuestLog.SetQuestState(quest.questCode, quest.questState);
            Debug.Log($" вест {quest.questCode} изменЄн на {quest.questState}");
        }
    }
}
