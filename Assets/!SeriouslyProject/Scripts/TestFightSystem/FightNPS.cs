using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.Events;

public class FightNPS : MonoBehaviour
{
    [SerializeField] private UnityEvent onFightStart;
    [SerializeField] private UnityEvent onFightEnd;

    public void WinFight(string luaCode, QuestState questTate)
    {
        QuestLog.SetQuestState(luaCode, questTate);
    }
}
