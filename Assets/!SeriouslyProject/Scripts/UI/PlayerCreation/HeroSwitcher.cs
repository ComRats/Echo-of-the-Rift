using System.Collections.Generic;
using UnityEngine;

public class HeroSwitcher : MonoBehaviour
{
    [SerializeField] private List<GameObject> heroes;
    private int currentIndex = 0;

    private void Start()
    {
        ShowHero(currentIndex);
    }

    public void NextHero()
    {
        currentIndex++;
        if (currentIndex >= heroes.Count)
            currentIndex = 0;

        ShowHero(currentIndex);
    }

    public void PrevHero()
    {
        currentIndex--;
        if (currentIndex < 0)
            currentIndex = heroes.Count - 1;

        ShowHero(currentIndex);
    }

    private void ShowHero(int index)
    {
        for (int i = 0; i < heroes.Count; i++)
            heroes[i].SetActive(i == index);
    }
}