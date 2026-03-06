using FightSystem.Data;
using UnityEngine;
using Zenject;

namespace PixelCrushers.DialogueSystem.SequencerCommands
{
    /// <summary>
    /// Добавляет союзника в команду игрока
    /// Использование: RecruitCompanion(CharacterDataName)
    /// Пример: RecruitCompanion(Knight)
    /// </summary>
    public class SequencerCommandRecruitCompanion : SequencerCommand
    {
        [Inject] private MainUI mainUI;

        public void Awake()
        {
            string characterDataName = GetParameter(0);

            if (string.IsNullOrEmpty(characterDataName))
            {
                Debug.LogError("[SequencerCommandRecruitCompanion] Не указано имя CharacterData!");
                Stop();
                return;
            }

            Team team = GlobalLoader.Instance.playerInstance.team;

            if (team == null)
            {
                Debug.LogError("[SequencerCommandRecruitCompanion] Team не найден!");
                Stop();
                return;
            }

            CharacterData data = Resources.Load<CharacterData>("CharacterData/" + characterDataName);

            if (data == null)
            {
                Debug.LogError($"[SequencerCommandRecruitCompanion] CharacterData {characterDataName} не найден!");
                Stop();
                return;
            }

            // Проверяем чтобы персонаж не добавился дважды
            if (team.characters.Exists(c => c.characterDataName == characterDataName))
            {
                Debug.Log($"[RecruitCompanion] {characterDataName} уже в команде");
                Stop();
                return;
            }

            CharactersSettings newCharacter = new CharactersSettings
            {
                useCharacterData = true,
                characterData = data,
                characterDataName = characterDataName
            };

            team.characters.Add(newCharacter);

            MainUI mainUI = GlobalLoader.Instance.mainUI;
            mainUI.teamManager.InitializeTeam();

            Debug.Log($"[RecruitCompanion] {characterDataName} присоединился к команде!");

            Stop();
        }
    }
}