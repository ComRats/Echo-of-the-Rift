using EchoRift;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

public class BaseTrigger : MonoBehaviour
{
    [Header("Quest Settings")]
    public QuestChange[] quests;

    [Header("Trigger Identity")]
    [SerializeField] private string triggerID;

    private void OnValidate()
    {
        if (string.IsNullOrEmpty(triggerID))
            triggerID = System.Guid.NewGuid().ToString();
    }

    private FightResult result;

    public void EventApply()
    {
        // Проверяем что бой был инициирован именно этим триггером
        if (Player.LastFightTriggerID != triggerID)
            return;

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
        Player.LastFightTriggerID = string.Empty;
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
                    Debug.LogWarning($"[BaseTrigger] ApplyQuestChanges: result = {result}, пропускаем '{quest.questCode}'");
                    continue;
            }

            var before = QuestLog.GetQuestState(quest.questCode);
            QuestLog.SetQuestState(quest.questCode, stateToApply);
            var after = QuestLog.GetQuestState(quest.questCode);
            Debug.Log($"[BaseTrigger] Квест '{quest.questCode}': {before} → {after} (result={result}, target={stateToApply})");
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
