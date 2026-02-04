using EchoRift;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

public class BaseTrigger : MonoBehaviour
{
    [Header("Quest Settings")]
    public QuestChange[] quests;

    private FightResult result;

    public void EventApply()
    {
        result = Player.Result;

        foreach (var quest in quests)
        {
            switch (result)
            {
                case FightResult.Win:
                    quest.onFightWin?.Invoke();
                    break;

                case FightResult.Lose:
                    quest.onFightLose?.Invoke();
                    break;

                case FightResult.Escape:
                    quest.onFightEscape?.Invoke();
                    break;
            }

        }
        Player.Result = FightResult.None;
    }

    public void ApplyQuestChanges()
    {
        foreach (var quest in quests)
        {
            QuestState stateToApply;

            switch (result)
            {
                case FightResult.Win:
                    stateToApply = quest.questState_Win;
                    break;

                case FightResult.Lose:
                    stateToApply = quest.questState_Lose;
                    break;

                case FightResult.Escape:
                    stateToApply = quest.questState_Escape;
                    break;

                default:
                    continue;
            }

            QuestLog.SetQuestState(quest.questCode, stateToApply);
            Debug.Log($" вест {quest.questCode} изменЄн на {stateToApply}");
        }
    }
}

[System.Serializable]
public struct QuestChange
{
    public string questCode;

    public QuestState questState_Win;
    public UnityEvent onFightWin;

    public QuestState questState_Lose;
    public UnityEvent onFightLose;

    public QuestState questState_Escape;
    public UnityEvent onFightEscape;

}

public enum FightResult
{
    None,
    Win,
    Lose,
    Escape
}
