using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TeamMember: MonoBehaviour
{
    [SerializeField] private int teamMemberIndex; // 0 = игрок, 1 = первый тиммейт, 2 = второй и т.д.
    [SerializeField] private Slider xpBar;
    [SerializeField] private TextMeshProUGUI xpText;
    [SerializeField] private TextMeshProUGUI levelText;
    [SerializeField] private Slider healthBar;
    [SerializeField] private TextMeshProUGUI healthText;

    private Base character;
    private bool isInBattle = false;

    private void Start()
    {
        TryFindCharacterInScene();
        
        if (!isInBattle)
        {
            // Вне боя - берем данные из сохранения
            UpdateUIFromSaveData();
        }
    }

    private void TryFindCharacterInScene()
    {
        // Пытаемся найти персонажа на сцене боя по индексу
        var fightManager = FindObjectOfType<FightManager>();
        if (fightManager != null && fightManager.characters.Count > teamMemberIndex)
        {
            character = fightManager.characters[teamMemberIndex];
            
            if (character != null)
            {
                isInBattle = true;
                character.OnXPChanged += UpdateXPUI;
                character.OnHealthChanged += UpdateHealthUI;
                
                UpdateXPUI(character.CurrentXP, character.MaxXP);
                UpdateHealthUI(character.Health, character.MaxHealth);
            }
        }
    }

    private void OnDestroy()
    {
        if (character != null)
        {
            character.OnXPChanged -= UpdateXPUI;
            character.OnHealthChanged -= UpdateHealthUI;
        }
    }

    private void UpdateUIFromSaveData()
    {
        if (GlobalLoader.Instance?.playerInstance?.playerSaver == null) return;

        // Для игрока (индекс 0) берем данные из playerSaver
        if (teamMemberIndex == 0)
        {
            var data = GlobalLoader.Instance.playerInstance.playerSaver;
            
            UpdateXPUI(data.CurrentXP, data.MaxXP);
            UpdateHealthUI(data.Health, data.MaxHealth);
            levelText.text = $"Level {data.Level}";
        }
        // Для тиммейтов нужно будет добавить систему сохранения команды
        // TODO: Добавить сохранение данных тиммейтов
    }

    private void UpdateXPUI(int current, int max)
    {
        if (isInBattle && character != null)
        {
            levelText.text = $"Level {character.Level}";
        }
        
        xpText.text = $"{current}/{max}";
        xpBar.value = current;
        xpBar.maxValue = max;
    }

    private void UpdateHealthUI(int current, int max)
    {
        healthText.text = $"{current}/{max}";
        healthBar.value = current;
        healthBar.maxValue = max;
    }
}
