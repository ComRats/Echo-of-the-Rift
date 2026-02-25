namespace EchoRift.SaveLoadSystem
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
        
        public static string GetPlayerSceneSave(string sceneName) => $"playerSave_{sceneName}";
        
        public const string INVENTORY_DATA = "inventoryData";
        
        public const string ENEMY_SAVE = "EnemySave";
        public const string CHARACTER_SAVE = "CharacterSave";
        
        public const string AUDIO_SETTINGS = "AudioSettings";
        
        public const string DEBUG_LOGS = "DebugLogs";
    }
}
