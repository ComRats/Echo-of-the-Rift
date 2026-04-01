using System;
using UnityEngine;
using PixelCrushers;
using EchoRift.EchoRiftSaveLoadSystem;
using static EchoRift.EchoRiftSaveLoadSystem.SaveFileNames;

namespace EchoRift.Dialogue
{
    /// <summary>
    /// Унифицированное сохранение/загрузка состояния Dialogue System (Pixel Crushers)
    /// через тот же SaveLoadSystem что и все остальные данные игры.
    /// Файл: GameProcess/dialogueState.json
    /// </summary>
    public static class DialogueSaveManager
    {
        [Serializable]
        private class DialogueStateData
        {
            public string serializedState;
        }

        public static void Save()
        {
            var savedGameData = SaveSystem.RecordSavedGameData();
            var data = new DialogueStateData
            {
                serializedState = SaveSystem.Serialize(savedGameData)
            };
            SaveLoadSystem.Save(DIALOGUE_STATE, data, GAME_DIRECTORY);
            Debug.Log("[DialogueSaveManager] Saved.");
        }

        public static void Load()
        {
            if (!SaveLoadSystem.Exists(DIALOGUE_STATE, GAME_DIRECTORY))
            {
                Debug.Log("[DialogueSaveManager] No save found, skipping.");
                return;
            }

            var data = SaveLoadSystem.Load<DialogueStateData>(DIALOGUE_STATE, GAME_DIRECTORY);
            if (string.IsNullOrEmpty(data.serializedState))
            {
                Debug.LogWarning("[DialogueSaveManager] Save file is empty.");
                return;
            }

            var savedGameData = SaveSystem.Deserialize<SavedGameData>(data.serializedState);
            SaveSystem.ApplySavedGameData(savedGameData);
            Debug.Log("[DialogueSaveManager] Loaded.");
        }

        public static void Delete()
        {
            SaveLoadSystem.Delete(DIALOGUE_STATE);
            Debug.Log("[DialogueSaveManager] Deleted.");
        }

        public static bool Exists()
        {
            return SaveLoadSystem.Exists(DIALOGUE_STATE, GAME_DIRECTORY);
        }
    }
}
