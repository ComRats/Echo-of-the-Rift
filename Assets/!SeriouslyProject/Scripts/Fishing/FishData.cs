using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "ScriptableObjects/fishes")]
public class FishData : ScriptableObject
{
    public Sprite _sprite;
    public int _speed;
}
