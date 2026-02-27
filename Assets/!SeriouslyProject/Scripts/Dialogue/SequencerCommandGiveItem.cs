using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    /// <summary>
    /// Sequencer команда для выдачи предмета игроку
    /// Использование: GiveItem(ItemGameName, Amount)
    /// Пример: GiveItem(apple, 5) - выдаст 5 яблок
    /// Пример: GiveItem(sword) - выдаст 1 меч
    /// </summary>
    public class SequencerCommandGiveItem : SequencerCommand
    {
        public void Awake()
        {
            string itemGameName = GetParameter(0);
            int amount = GetParameterAsInt(1, 1);

            if (string.IsNullOrEmpty(itemGameName))
            {
                Debug.LogError("[SequencerCommandGiveItem] Не указано имя предмета!");
                Stop();
                return;
            }

            MainUI mainUI = GlobalLoader.Instance.mainUI;
            InventoryManager inventoryManager = mainUI.inventoryManager;

            if (inventoryManager == null)
            {
                Debug.LogError("[SequencerCommandGiveItem] InventoryManager не найден в MainUI!");
                Stop();
                return;
            }

            inventoryManager.AddItem(itemGameName, amount);

            Stop();
        }
    }
}
