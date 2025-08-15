using UnityEngine;

public class Quest : MonoBehaviour
{
    public int id = 0;
    public new string name = "Quest1";
    [TextArea]
    public string description = "Quest1 description";
    public bool isCompletetd = false;
    public Task task;
}
