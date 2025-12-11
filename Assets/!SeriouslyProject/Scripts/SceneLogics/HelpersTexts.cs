using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New HelpersTexts", menuName = "ScriptableObjects/Helpers")]
public class HelpersTexts : ScriptableObject
{
    public List<string> helps;

    public string GetRandomHelp()
    {
        return helps[Random.Range(0, helps.Count)];
    }
}
