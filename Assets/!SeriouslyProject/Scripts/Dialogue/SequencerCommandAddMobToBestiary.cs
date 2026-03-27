using UnityEngine;
using PixelCrushers.DialogueSystem;
using EchoRift.UI;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    public class SequencerCommandAddMobToBestiary : SequencerCommand
    {
        public void Awake()
        {
            string objectName = GetParameter(0);

            if (string.IsNullOrEmpty(objectName))
            {
                Debug.LogError("[AddMobToBestiary] Не указано имя объекта!");
                Stop();
                return;
            }

            GameObject target = GameObject.Find(objectName);

            if (target == null)
            {
                Debug.LogError($"[AddMobToBestiary] Объект '{objectName}' не найден!");
                Stop();
                return;
            }

            IMobProvider mobProvider = target.GetComponent<IMobProvider>();

            if (mobProvider == null || mobProvider.MobData == null)
            {
                Debug.LogError("[AddMobToBestiary] На объекте нет IMobProvider или MobData!");
                Stop();
                return;
            }

            MainUI mainUI = GlobalLoader.Instance.mainUI;

            if (mainUI == null || mainUI.playerUI == null)
            {
                Debug.LogError("[AddMobToBestiary] UI не найден!");
                Stop();
                return;
            }

            MobGuide guide = mainUI.playerUI.mobGuide;

            if (guide == null)
            {
                Debug.LogError("[AddMobToBestiary] mobGuide == null!");
                Stop();
                return;
            }
            guide.AddMob(mobProvider.MobData);

            Stop();
        }
    }
}