using UnityEngine;
using UnityEngine.UI;

public class TestDoDelete : MonoBehaviour
{
    [SerializeField] private ScreenFader screenFader;
    [SerializeField] private Button leftButton;
    [SerializeField] private Button rightButton;

    private void Start()
    {
        //leftButton.onClick.AddListener(screenFader.FadeIn);
        //rightButton.onClick.AddListener(screenFader.FadeOut);
    }
}
