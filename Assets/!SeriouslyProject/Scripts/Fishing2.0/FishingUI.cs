using TMPro;
using UnityEngine;

namespace Fishing2
{
    public class FishingUI : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fishingStatusText;

        public void ShowWaitingForBite()
        {
            if (fishingStatusText != null)
            {
                fishingStatusText.text = "Ожидание поклевки...\nНажмите f что бы вытянуть удочку";
            }
        }

        public void ShowBite()
        {
            if (fishingStatusText != null)
            {
                fishingStatusText.text = "Клюёт!\nНажмите f что бы вытянуть удочку";
            }
        }

        public void HideText()
        {
            if (fishingStatusText != null)
            {
                fishingStatusText.text = "";
            }
        }
    }
}