using UnityEngine;
using DG.Tweening;
using System.Collections;

public class Dice : MonoBehaviour
{
    [SerializeField] private Sprite[] diceSprites;
    private SpriteRenderer sr;
    public int CurrentSide { get; private set; }

    private void Awake()
    {
        sr = GetComponent<SpriteRenderer>();
    }
        

    public IEnumerator RollAnimation()
    {
        transform.DOPunchScale(Vector3.one * 0.3f, 0.5f);
        transform.DORotate(new Vector3(0, 0, 360), 0.5f, RotateMode.LocalAxisAdd);

        for (int i = 0; i < 10; i++)
        {
            sr.sprite = diceSprites[Random.Range(0, 6)];
            yield return new WaitForSeconds(0.05f);
        }
        
        CurrentSide = Random.Range(1, 7);
        sr.sprite = diceSprites[CurrentSide - 1];
    }
}