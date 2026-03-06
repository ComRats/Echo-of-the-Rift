using FightSystem.Character;
using System.Collections.Generic;
using UnityEngine;
using EchoRift;

/// <summary>
/// Синхронизирует данные персонажей между боевой системой и командой игрока
/// </summary>
public class BattleTeamSync : MonoBehaviour
{
    [SerializeField] private FightManager fightManager;

    private bool isSynced = false;
    private Dictionary<string, Character> initialCharacters = new Dictionary<string, Character>();

    private void Start()
    {
        if (fightManager == null)
            fightManager = GetComponent<FightManager>();

        // Сохраняем ссылки на всех персонажей в начале боя
        if (fightManager != null && fightManager.characters != null)
        {
            foreach (var character in fightManager.characters)
            {
                if (character != null)
                {
                    initialCharacters[character.Name] = character;
                    Debug.Log($"[BattleTeamSync] Registered character: {character.Name}");
                }
            }
        }
    }

    public void SyncTeamAfterBattle()
    {
        if (isSynced)
        {
            Debug.Log("[BattleTeamSync] Already synced, skipping");
            return;
        }

        if (GlobalLoader.Instance == null || GlobalLoader.Instance.playerInstance == null)
        {
            Debug.LogWarning("[BattleTeamSync] GlobalLoader or Player not found");
            return;
        }

        var team = GlobalLoader.Instance.playerInstance.GetComponent<Team>();
        if (team == null)
        {
            Debug.LogWarning("[BattleTeamSync] Team component not found on player");
            return;
        }

        Debug.Log($"[BattleTeamSync] Syncing {team.characters.Count} team members");

        // Синхронизируем данные из боевых персонажей в настройки команды
        foreach (var settings in team.characters)
        {
            if (settings == null) continue;

            // Ищем персонажа по имени в сохраненных ссылках
            if (initialCharacters.TryGetValue(settings.Name, out var character))
            {
                Debug.Log($"[BattleTeamSync] Syncing {settings.Name}: Level {character.Level}, HP {character.Health}/{character.MaxHealth}, XP {character.CurrentXP}/{character.MaxXP}");

                // Если используется ScriptableObject, обновляем runtime копию
                if (settings.useCharacterData && settings.RuntimeData != null)
                {
                    settings.RuntimeData.Health = character.Health;
                    settings.RuntimeData.Mana = character.Mana;
                    settings.RuntimeData.Level = character.Level;
                    settings.RuntimeData.CurrentXP = character.CurrentXP;
                    settings.RuntimeData.MaxXP = character.MaxXP;
                    settings.RuntimeData.Damage = character.Damage;
                    settings.RuntimeData.MaxHealth = character.MaxHealth;
                    settings.RuntimeData.Heal = character.Heal;
                    settings.RuntimeData.Armor = character.Armor;
                    settings.RuntimeData.MaxMana = character.MaxMana;
                    settings.RuntimeData.XpReward = character.XpReward;
                    
                    Debug.Log($"[BattleTeamSync] RuntimeData updated for {settings.Name}: Level {settings.RuntimeData.Level}, XP {settings.RuntimeData.CurrentXP}/{settings.RuntimeData.MaxXP}");
                }
                else
                {
                    // Для ручных настроек используем сеттеры
                    settings.Health = character.Health;
                    settings.MaxHealth = character.MaxHealth;
                    settings.Mana = character.Mana;
                    settings.MaxMana = character.MaxMana;
                    settings.Level = character.Level;
                    settings.CurrentXP = character.CurrentXP;
                    settings.MaxXP = character.MaxXP;
                    settings.Damage = character.Damage;
                    settings.Heal = character.Heal;
                    settings.Armor = character.Armor;
                    settings.XpReward = character.XpReward;
                    
                    Debug.Log($"[BattleTeamSync] Direct settings updated for {settings.Name}: Level {settings.Level}, XP {settings.CurrentXP}/{settings.MaxXP}");
                }
            }
            else
            {
                Debug.LogWarning($"[BattleTeamSync] Character {settings.Name} not found in battle");
            }
        }

        // Сохраняем обновленные данные команды
        var teamData = team.CreateSaveData();
        
        // Логируем данные перед сохранением
        foreach (var charData in teamData.charactersData)
        {
            Debug.Log($"[BattleTeamSync] Saving to file: {charData.Name} - Level {charData.Level}, XP {charData.CurrentXP}/{charData.MaxXP}, HP {charData.Health}/{charData.MaxHealth}");
        }
        
        EchoRift.SaveLoadSystem.SaveLoadSystem.Save(
            EchoRift.SaveLoadSystem.SaveFileNames.TEAM_DATA, 
            teamData, 
            EchoRift.SaveLoadSystem.SaveFileNames.GAME_DIRECTORY
        );

        isSynced = true;
        Debug.Log("[BattleTeamSync] Team data synchronized and saved");
    }

    private void OnDestroy()
    {
        // Автоматически синхронизируем при выходе из боя если еще не синхронизировали
        if (!isSynced && (Player.Result == FightResult.Win || Player.Result == FightResult.Lose || Player.Result == FightResult.Escape))
        {
            SyncTeamAfterBattle();
        }
    }
}
