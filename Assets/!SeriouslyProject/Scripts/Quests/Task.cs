using System.Collections.Generic;
using FightSystem.Enemy;
using UnityEngine;

[CreateAssetMenu(fileName = "NewTask", menuName = "Quest/Task")]
public class Task : ScriptableObject
{
    enum State 
    { 
        KillSomebody, 
        TalkToSomebody,
        VisitPlace
    }

    [SerializeField] State currenTask;
}
public class KillSomebody
{
    public Enemy enemyToKill;
    public int killCount;

    public void CompleteTask()
    {
        //Сделать у Enemy при Death вызов проверки выполнения задания
        if (killCount == 0)
        {
            OnTaskCompleted();
        }
    }

    public void FailTask()
    {
        //Сделать у Enemy при Death вызов проверки выполнения задания
        if (killCount == 0 /*&& вышел из локации или не дрался с кем-то*/)
        {
            OnTaskFailed();
        }
    }

    public void OnTaskCompleted()
    {
        //Выдать награду я хз
    }

    public void OnTaskFailed()
    {
        //Грусни смайлик
    }
}

public class TalkingToSomebody
{
    public ITalkable NPS;
}
