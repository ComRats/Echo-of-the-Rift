using System;
using UnityEngine;
using PixelCrushers;
using EchoRift.EchoRiftSaveLoadSystem;
using static EchoRift.EchoRiftSaveLoadSystem.SaveFileNames;

namespace EchoRift.Dialogue
{
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
        }

        public static void Load()
        {
            if (!SaveLoadSystem.Exists(DIALOGUE_STATE, GAME_DIRECTORY))
            {
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
        }

        public static void Delete()
        {
            SaveLoadSystem.Delete(DIALOGUE_STATE);
        }

        public static bool Exists()
        {
            return SaveLoadSystem.Exists(DIALOGUE_STATE, GAME_DIRECTORY);
        }
    }
}
