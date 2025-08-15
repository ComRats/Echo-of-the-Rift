using UnityEngine;

public class Fish : MonoBehaviour
{
    public int speed;
    public SpriteRenderer sprite;

    public void Initialization(FishData _randomFish)
    {
        sprite = GetComponent<SpriteRenderer>();
        speed = _randomFish._speed;
        sprite.sprite = _randomFish._sprite;
    }
}