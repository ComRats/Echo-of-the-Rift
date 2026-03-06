using FightSystem.Data;
using UnityEngine;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    /// <summary>
    /// Удаляет союзника из команды игрока
    /// Использование: RemoveCompanion(CharacterDataName)
    /// Пример: RemoveCompanion(Knight)
    /// </summary>
    public class SequencerCommandRemoveCompanion : SequencerCommand
    {
        public void Awake()
        {
            string characterDataName = GetParameter(0);

            if (string.IsNullOrEmpty(characterDataName))
            {
                Debug.LogError("[RemoveCompanion] Не указано имя CharacterData!");
                Stop();
                return;
            }

            if (GlobalLoader.Instance == null || GlobalLoader.Instance.playerInstance == null)
            {
                Debug.LogError("[RemoveCompanion] GlobalLoader или playerInstance null!");
                Stop();
                return;
            }

            Team team = GlobalLoader.Instance.playerInstance.team;

            if (team == null)
            {
                Debug.LogError("[RemoveCompanion] Team не найден!");
                Stop();
                return;
            }

            CharactersSettings characterToRemove = team.characters
                .Find(c => c.characterDataName == characterDataName);

            if (characterToRemove == null)
            {
                Debug.LogWarning($"[RemoveCompanion] {characterDataName} не найден в команде");
                Stop();
                return;
            }

            team.characters.Remove(characterToRemove);

            MainUI mainUI = GlobalLoader.Instance.mainUI;

            mainUI.teamManager.InitializeTeam();

            Debug.Log($"[RemoveCompanion] {characterDataName} покинул команду");

            Stop();
        }
    }
}