using UnityEngine;

namespace EchoRift.EchoRiftSaveLoadSystem
{
    /// <summary>
    /// Централизованное хранилище имен файлов сохранений.
    /// Использование констант предотвращает опечатки при обращении к файлам.
    /// </summary>
    public static class SaveFileNames
    {
        public const string GAME_DIRECTORY = "GameProcess";
        
        public const string GLOBAL_SAVE = "globalSave";
        
        public const string PLAYER_DATA = "playerData";
        public const string PLAYER_NAME = "PlayerName";
        public const string TEAM_DATA = "teamData";
        
        public static string GetPlayerSceneSave(string sceneName) => $"playerSave_{sceneName}";
        
        public const string INVENTORY_DATA = "inventoryData";
        
        public const string ENEMY_SAVE = "EnemySave";
        public const string CHARACTER_SAVE = "CharacterSave";

        public const string DIALOGUE_STATE = "dialogueState";

        public const string SETTINGS = "GameSettings";

        public const string DEBUG_LOGS = "DebugLogs";

        [System.Serializable]
        public class GlobalSettingsData
        {
            public float musicVolume = 1f;
            public float sfxVolume = 1f;

            public float enemyTurnDelay = 1.5f;
            public float enemyTurnSpeed = 1f;
            public float loadingSceneSpeed = 1f;

            public KeyCode openInventoryKey = KeyCode.E;
            public KeyCode openPauseMenuKey = KeyCode.Escape;
            public KeyCode useButton = KeyCode.F;
            public KeyCode questWindowKey = KeyCode.J;
        }
    }
}
