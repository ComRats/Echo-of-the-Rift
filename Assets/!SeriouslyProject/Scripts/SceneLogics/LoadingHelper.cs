using TMPro;
using UnityEngine;

public class LoadingHelper : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI textHelper;
    [SerializeField] private HelpersTexts helper;

    private void Start()
    {
        textHelper.text = helper.GetRandomHelp();
    }

}
