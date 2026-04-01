using EchoRift;
using EchoRift.EchoRiftSaveLoadSystem;
using static EchoRift.EchoRiftSaveLoadSystem.SaveFileNames;
using FightSystem.Enemy;
using Sirenix.OdinInspector;
using System;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static GlobalLoader;
using PixelCrushers.DialogueSystem;

public class ActionButtons : MonoBehaviour
{
    #region UI References
    [FoldoutGroup("UI")][SerializeField] private GameObject physicAttackButtons;
    [FoldoutGroup("UI")][SerializeField] private GameObject magicAttackButtons;
    [FoldoutGroup("UI")][SerializeField] private Button physicAttackToggleBtn;
    [FoldoutGroup("UI")][SerializeField] private Button magicAttackToggleBtn;
    #endregion 

    [Title("UI Feedback")]
    [SerializeField] private TextMeshProUGUI descriptionText;
    [SerializeField] private TextMeshProTypewriterEffect typewriterEffect;
    [SerializeField] private bool useTypewriterEffect = true;
    [SerializeField] private float typewriterSpeed = 100f;
    
    [Title("Text Highlighting")]
    [SerializeField] private Color damageColor = new Color(1f, 0.3f, 0.3f); // Красный для урона
    [SerializeField] private Color healColor = new Color(0.3f, 1f, 0.3f); // Зеленый для хила
    [SerializeField] private Color defenseColor = new Color(0.3f, 0.6f, 1f); // Синий для защиты
    [SerializeField] private Color manaColor = new Color(0.4f, 0.4f, 1f); // Фиолетовый для маны
    [SerializeField] private float highlightSizeMultiplier = 1.2f; // Увеличение размера выделенного текста

    [HideInInspector] public Enemy currentEnemy;

    [SerializeField] private FightManager fightManager;
    [SerializeField] private SceneLoader sceneLoader;
    [SerializeField] private AbilityManager abilityManager;

    [Inject] private MainUI mainUI;

    private Action pendingAction;
    private BattleAbility pendingAbility;
    private Base pendingAttacker;

    private void Start()
    {
        magicAttackToggleBtn.onClick.AddListener(() => OpenButtons(magicAttackButtons, physicAttackButtons));
        physicAttackToggleBtn.onClick.AddListener(() => OpenButtons(physicAttackButtons, magicAttackButtons));

        typewriterEffect.charactersPerSecond = typewriterSpeed;
        typewriterEffect.playOnEnable = false;
    }

    public void OnEnemySelected(Enemy enemy)
    {
        currentEnemy = enemy;

        // Выполняем ожидающее действие только если оно есть
        if (pendingAction != null)
        {
            ExecutePendingAction();
        }
    }

    /// <summary>
    /// Метод для установки ожидающей способности из AbilityManager
    /// </summary>
    public void SetPendingAbility(BattleAbility ability, Base attacker)
    {
        if (!ability.CanUse(attacker))
        {
            return;
        }

        if (attacker is FightSystem.Character.Character character)
        {
            if (!character.IsTurn)
            {
                Debug.Log("Не ваш ход!");
                return;
            }
        }

        pendingAbility = ability;
        pendingAttacker = attacker;

        // Отображаем описание способности
        ShowAbilityDescription(ability, attacker);

        switch (ability.targetType)
        {
            case TargetType.Self:
                ExecuteAbilityOnTarget(attacker);
                break;

            case TargetType.Ally:
                Debug.Log("Выберите союзника для способности: " + ability.AbilityName);
                StartCharacterSelection();
                break;

            case TargetType.Enemy:
                pendingAction = () =>
                {
                    if (currentEnemy == null)
                    {
                        Debug.LogWarning("No target enemy selected.");
                        return;
                    }

                    ExecuteAbilityOnTarget(currentEnemy);
                };

                if (currentEnemy != null)
                {
                    ExecutePendingAction();
                }
                else
                {
                    Debug.Log("Выберите врага для способности: " + ability.AbilityName);
                }
                break;

            case TargetType.AllEnemies:
                ExecuteAbilityOnAllEnemies();
                break;

            case TargetType.AllAllies:
                ExecuteAbilityOnAllAllies();
                break;
        }
    }

    /// <summary>
    /// Отображает описание способности в UI
    /// </summary>
    public void ShowAbilityDescription(BattleAbility ability, Base attacker = null)
    {
        if (descriptionText == null) return;

        string textToShow;
        if (!string.IsNullOrEmpty(ability.Description))
        {
            textToShow = ProcessHighlightedText(ability.Description);
        }
        else
        {
            textToShow = ability.AbilityName;
        }

        // Добавляем строку со статистикой способности
        string statsLine = GenerateAbilityStats(ability, attacker);
        if (!string.IsNullOrEmpty(statsLine))
        {
            textToShow += "\n" + statsLine;
        }

        if (useTypewriterEffect && typewriterEffect != null)
        {
            typewriterEffect.Stop();

            descriptionText.text = textToShow;
            typewriterEffect.StartTyping(textToShow);
        }
        else
        {
            descriptionText.text = textToShow;
        }
    }

    /// <summary>
    /// Генерирует строку со статистикой способности
    /// </summary>
    private string GenerateAbilityStats(BattleAbility ability, Base attacker = null)
    {
        System.Text.StringBuilder stats = new System.Text.StringBuilder();

        // Стоимость маны
        if (ability.ManaCost > 0)
        {
            stats.Append($"[mana]{ability.ManaCost} маны[/mana]");
        }

        // Специфичные параметры в зависимости от типа способности
        if (ability is MeleeAbility meleeAbility)
        {
            var multiplierField = meleeAbility.GetType().GetField("baseDamageMultiplier",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var flatBonusField = meleeAbility.GetType().GetField("flatDamageBonus",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (multiplierField != null)
            {
                int multiplier = (int)multiplierField.GetValue(meleeAbility);
                int flatBonus = flatBonusField != null ? (int)flatBonusField.GetValue(meleeAbility) : 0;

                if (stats.Length > 0) stats.Append(" | ");

                if (attacker != null)
                {
                    // Итоговый урон = базовый урон атакующего * множитель + плоский бонус
                    int finalDamage = attacker.Damage * multiplier + flatBonus;
                    stats.Append($"[dmg]{finalDamage} урона[/dmg]");
                }
                else
                {
                    stats.Append($"[dmg]x{multiplier} урона[/dmg]");
                }
            }

            var hasEffect = meleeAbility.GetType().GetField("hasStatusEffect",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (hasEffect != null && (bool)hasEffect.GetValue(meleeAbility))
            {
                var effectField = meleeAbility.GetType().GetField("statusEffect",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
                var chanceField = meleeAbility.GetType().GetField("chanceToApply",
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

                if (effectField != null && chanceField != null)
                {
                    var effect = effectField.GetValue(meleeAbility) as StatusEffectSO;
                    float chance = (float)chanceField.GetValue(meleeAbility);
                    if (effect != null)
                    {
                        if (stats.Length > 0) stats.Append(" | ");
                        stats.Append($"<color=#FF8800>{effect.effectName} ({chance:F0}%)</color>");
                    }
                }
            }
        }
        else if (ability is MagicAbility magicAbility)
        {
            var damageField = magicAbility.GetType().GetField("magicDamage",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var flatBonusField = magicAbility.GetType().GetField("flatDamageBonus",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (damageField != null)
            {
                int damage = (int)damageField.GetValue(magicAbility);
                int flatBonus = flatBonusField != null ? (int)flatBonusField.GetValue(magicAbility) : 0;
                if (stats.Length > 0) stats.Append(" | ");
                stats.Append($"[dmg]{damage + flatBonus} маг. урона[/dmg]");
            }

            var effectField = magicAbility.GetType().GetField("statusEffect",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (effectField != null)
            {
                var effect = effectField.GetValue(magicAbility) as StatusEffectSO;
                if (effect != null)
                {
                    if (stats.Length > 0) stats.Append(" | ");
                    stats.Append($"<color=#FF8800>{effect.effectName}</color>");
                }
            }
        }
        else if (ability is HealAbility healAbility)
        {
            var baseHealField = healAbility.GetType().GetField("baseHealAmount",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var useCharStatField = healAbility.GetType().GetField("useCharacterHealStat",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var multiplierField = healAbility.GetType().GetField("healMultiplier",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (baseHealField != null && useCharStatField != null && multiplierField != null)
            {
                int baseHeal = (int)baseHealField.GetValue(healAbility);
                bool useCharStat = (bool)useCharStatField.GetValue(healAbility);
                float multiplier = (float)multiplierField.GetValue(healAbility);

                if (stats.Length > 0) stats.Append(" | ");
                if (useCharStat && attacker != null)
                {
                    int finalHeal = Mathf.RoundToInt(attacker.GiveHeal() * multiplier);
                    stats.Append($"[heal]+{finalHeal} HP[/heal]");
                }
                else if (useCharStat)
                {
                    stats.Append($"[heal]Лечение x{multiplier:F1}[/heal]");
                }
                else
                {
                    stats.Append($"[heal]+{baseHeal} HP[/heal]");
                }
            }

            var effectField = healAbility.GetType().GetField("healOverTimeEffect",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            if (effectField != null)
            {
                var effect = effectField.GetValue(healAbility) as StatusEffectSO;
                if (effect != null)
                {
                    if (stats.Length > 0) stats.Append(" | ");
                    stats.Append($"<color=#88FF88>{effect.effectName}</color>");
                }
            }
        }
        else if (ability is DefenseAbility defenseAbility)
        {
            var bonusField = defenseAbility.GetType().GetField("bonusDefense",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            var durationField = defenseAbility.GetType().GetField("duration",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (bonusField != null && durationField != null)
            {
                int bonus = (int)bonusField.GetValue(defenseAbility);
                int duration = (int)durationField.GetValue(defenseAbility);

                if (stats.Length > 0) stats.Append(" | ");
                stats.Append($"[def]+{bonus} защиты ({duration} ход.)[/def]");
            }
        }
        else if (ability is StatusAbility statusAbility)
        {
            var effectField = statusAbility.GetType().GetField("effect",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var multiplierField = statusAbility.GetType().GetField("damageMultiplier",
                System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            if (multiplierField != null)
            {
                float multiplier = (float)multiplierField.GetValue(statusAbility);
                if (stats.Length > 0) stats.Append(" | ");

                if (attacker != null)
                {
                    int finalDamage = Mathf.RoundToInt(attacker.Damage * multiplier);
                    stats.Append($"[dmg]{finalDamage} урона[/dmg]");
                }
                else
                {
                    stats.Append($"[dmg]x{multiplier:F1} урона[/dmg]");
                }
            }

            if (effectField != null)
            {
                var effect = effectField.GetValue(statusAbility) as StatusEffectSO;
                if (effect != null)
                {
                    if (stats.Length > 0) stats.Append(" | ");
                    stats.Append($"<color=#FF8800>{effect.effectName}</color>");
                }
            }
        }

        return ProcessHighlightedText(stats.ToString());
    }

    /// <summary>
    /// Обрабатывает текст и применяет выделение к ключевым словам
    /// Использование: [dmg]50[/dmg], [heal]30[/heal], [def]20[/def], [mana]15[/mana]
    /// </summary>
    private string ProcessHighlightedText(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;

        // Конвертируем цвета в hex для TextMeshPro
        string damageHex = ColorUtility.ToHtmlStringRGB(damageColor);
        string healHex = ColorUtility.ToHtmlStringRGB(healColor);
        string defenseHex = ColorUtility.ToHtmlStringRGB(defenseColor);
        string manaHex = ColorUtility.ToHtmlStringRGB(manaColor);

        int sizePercent = Mathf.RoundToInt(highlightSizeMultiplier * 100);

        // Заменяем кастомные теги на Rich Text теги TextMeshPro
        text = text.Replace("[dmg]", $"<size={sizePercent}%><color=#{damageHex}><b>");
        text = text.Replace("[/dmg]", "</b></color></size>");

        text = text.Replace("[heal]", $"<size={sizePercent}%><color=#{healHex}><b>");
        text = text.Replace("[/heal]", "</b></color></size>");

        text = text.Replace("[def]", $"<size={sizePercent}%><color=#{defenseHex}><b>");
        text = text.Replace("[/def]", "</b></color></size>");

        text = text.Replace("[mana]", $"<size={sizePercent}%><color=#{manaHex}><b>");
        text = text.Replace("[/mana]", "</b></color></size>");

        return text;
    }

    /// <summary>
    /// Очищает текст описания
    /// </summary>
    public void ClearDescription()
    {
        if (descriptionText != null)
        {
            if (typewriterEffect != null)
            {
                typewriterEffect.Stop();
            }
            descriptionText.text = "";
        }
    }

    private void ExecuteAbilityOnTarget(Base target)
    {
        if (pendingAbility == null || pendingAttacker == null)
        {
            return;
        }

        if (pendingAttacker is FightSystem.Character.Character character)
        {
            if (!character.IsTurn)
            {
                Debug.LogWarning("Ход уже завершен!");
                pendingAbility = null;
                pendingAttacker = null;
                pendingAction = null;
                ClearDescription();
                return;
            }
        }

        pendingAbility.Execute(pendingAttacker, target);

        if (pendingAttacker is FightSystem.Character.Character chr)
        {
            chr.UpdateUI();
            chr.IsTurn = false;
        }

        // Удаляем врага из списка, если он умер
        if (target is Enemy enemy)
        {
            fightManager.DeleteEnemyOnList(enemy);
        }

        pendingAbility = null;
        pendingAttacker = null;
        pendingAction = null;
        ClearDescription();
    }

    private void ExecuteAbilityOnAllEnemies()
    {
        if (pendingAbility == null || pendingAttacker == null)
        {
            return;
        }

        // Проверяем, что персонаж еще не завершил свой ход
        if (pendingAttacker is FightSystem.Character.Character character)
        {
            if (!character.IsTurn)
            {
                Debug.LogWarning("Ход уже завершен!");
                pendingAbility = null;
                pendingAttacker = null;
                pendingAction = null;
                ClearDescription();
                return;
            }
        }

        foreach (var enemy in fightManager.enemies.ToList())
        {
            pendingAbility.Execute(pendingAttacker, enemy);
            fightManager.DeleteEnemyOnList(enemy);
        }

        if (pendingAttacker is FightSystem.Character.Character chr)
        {
            chr.UpdateUI();
            chr.IsTurn = false;
        }

        pendingAbility = null;
        pendingAttacker = null;
        pendingAction = null;
        ClearDescription();
    }

    private void ExecuteAbilityOnAllAllies()
    {
        if (pendingAbility == null || pendingAttacker == null)
        {
            return;
        }

        // Проверяем, что персонаж еще не завершил свой ход
        if (pendingAttacker is FightSystem.Character.Character character)
        {
            if (!character.IsTurn)
            {
                Debug.LogWarning("Ход уже завершен!");
                pendingAbility = null;
                pendingAttacker = null;
                pendingAction = null;
                ClearDescription();
                return;
            }
        }

        foreach (var ally in fightManager.characters)
        {
            pendingAbility.Execute(pendingAttacker, ally);
        }

        if (pendingAttacker is FightSystem.Character.Character chr)
        {
            chr.UpdateUI();
            chr.IsTurn = false;
        }

        pendingAbility = null;
        pendingAttacker = null;
        pendingAction = null;
        ClearDescription();
    }

    private void StartCharacterSelection()
    {
        // Включаем подсветку персонажей для выбора
        foreach (var character in fightManager.characters)
        {
            character.IsBlinking = true;
        }
        fightManager.StopEnemyBlinking();
    }

    public void OnCharacterSelected(FightSystem.Character.Character character)
    {
        if (pendingAbility != null && pendingAttacker != null)
        {
            ExecuteAbilityOnTarget(character);

            // Выключаем подсветку персонажей
            foreach (var c in fightManager.characters)
            {
                c.IsBlinking = false;
            }
        }
    }

    private void ExecutePendingAction()
    {
        if (pendingAction != null)
        {
            pendingAction.Invoke();
            pendingAction = null;
        }
    }

    private void OpenButtons(GameObject toOpen, GameObject toClose)
    {
        toOpen.SetActive(!toOpen.activeSelf);
        toClose.SetActive(false);
    }

    public void EscapeFight()
    {
        Player.Result = FightResult.Escape;
        
        // Синхронизируем команду перед выходом из боя
        var battleSync = FindObjectOfType<BattleTeamSync>();
        if (battleSync != null)
        {
            battleSync.SyncTeamAfterBattle();
        }
        else
        {
            Debug.LogWarning("[ActionButtons] BattleTeamSync not found, team data not synchronized");
        }
        
        var data = SaveLoadSystem.Load<GlobalData>(GLOBAL_SAVE, GAME_DIRECTORY);
        sceneLoader.LoadAsync(data.SceneIndex);
    }

    public void OpenInventory()
    {
        mainUI.playerUI.enabled = !mainUI.playerUI.enabled;
        mainUI.canvas.enabled = !mainUI.canvas.enabled;
        mainUI.playerUI.ToggleInventoryOnFight();
    }
}