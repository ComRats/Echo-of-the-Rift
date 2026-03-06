using FightSystem.Character;
using FightSystem.Data;
using System.Collections.Generic;
using UnityEngine;

public class TeamManager : MonoBehaviour
{
    [SerializeField] private GameObject teamMemberPrefab;
    [SerializeField] private Transform teamMembersContainer;
    [SerializeField] private bool debugMode = false;

    private Team team;
    private List<TeamMember> teamMembers = new List<TeamMember>();

    private void Start()
    {
        team = GlobalLoader.Instance.playerInstance.team;
        InitializeTeam();
    }

    public void InitializeTeam()
    {
        ClearTeamUI();

        if (team == null || team.characters == null)
        {
            Debug.LogWarning("[TeamManager] Team or characters list is null");
            return;
        }

        foreach (var characterSettings in team.characters)
        {
            CreateTeamMemberUI(characterSettings);
        }
    }

    private void CreateTeamMemberUI(CharactersSettings characterSettings)
    {
        if (teamMemberPrefab == null || teamMembersContainer == null)
        {
            Debug.LogError("[TeamManager] TeamMemberPrefab or Container is not assigned");
            return;
        }

        // Проверка данных персонажа
        if (characterSettings == null)
        {
            Debug.LogWarning("[TeamManager] CharacterSettings is null, skipping");
            return;
        }

        // Валидация и исправление данных ScriptableObject
        if (characterSettings.useCharacterData && characterSettings.RuntimeData != null)
        {
            characterSettings.RuntimeData.ValidateAndFixData();
        }

        // Отладочная информация
        if (debugMode)
        {
            Debug.Log($"[TeamManager] Creating UI for: {characterSettings.Name}");
            Debug.Log($"  - UseCharacterData: {characterSettings.useCharacterData}");
            Debug.Log($"  - CharacterData: {characterSettings.characterData}");
            Debug.Log($"  - HP: {characterSettings.Health}/{characterSettings.MaxHealth}");
            Debug.Log($"  - XP: {characterSettings.CurrentXP}/{characterSettings.MaxXP}");
            Debug.Log($"  - Level: {characterSettings.Level}");
            Debug.Log($"  - Sprite: {characterSettings.Sprite}");
            
            if (characterSettings.useCharacterData && characterSettings.characterData != null)
            {
                Debug.Log($"  - CharacterData.Health: {characterSettings.characterData.Health}");
                Debug.Log($"  - CharacterData.MaxHealth: {characterSettings.characterData.MaxHealth}");
                Debug.Log($"  - CharacterData.CurrentXP: {characterSettings.characterData.CurrentXP}");
                Debug.Log($"  - CharacterData.MaxXP: {characterSettings.characterData.MaxXP}");
            }
        }

        GameObject memberObj = Instantiate(teamMemberPrefab, teamMembersContainer);
        TeamMember member = memberObj.GetComponent<TeamMember>();

        if (member != null)
        {
            member.Initialize(characterSettings, false);
            teamMembers.Add(member);
        }
        else
        {
            Debug.LogError("[TeamManager] TeamMember component not found on prefab");
        }
    }

    public void AddTeamMember(CharacterData data)
    {
        team.AddCharacter(data);

        CreateTeamMemberUI(team.characters[team.characters.Count - 1]);
    }

    private void ClearTeamUI()
    {
        foreach (var member in teamMembers)
        {
            if (member != null)
                Destroy(member.gameObject);
        }
        teamMembers.Clear();
    }

    public void UpdateTeamUI()
    {
        foreach (var member in teamMembers)
        {
            if (member != null)
                member.UpdateUI();
        }
    }

    public void LinkBattleCharacters(List<Character> battleCharacters)
    {
        for (int i = 0; i < Mathf.Min(teamMembers.Count, battleCharacters.Count); i++)
        {
            if (teamMembers[i] != null && battleCharacters[i] != null)
            {
                teamMembers[i].SetCharacter(battleCharacters[i]);
            }
        }
    }

    public void SyncFromBattle()
    {
        foreach (var member in teamMembers)
        {
            if (member != null)
                member.SyncFromBattle();
        }
    }

    public void SaveTeam()
    {
        if (GlobalLoader.Instance != null)
        {
            GlobalLoader.Instance.SavePlayer();
        }
    }
}
