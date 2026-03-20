using FightSystem.Data;
using UnityEngine;

public class EquipmentManager : MonoBehaviour
{
    public static EquipmentManager Instance { get; private set; }

    private const string PlayerCharacterDataName = "Human";

    public bool IsInBattle { get; private set; }

    private int appliedDamage;
    private int appliedMagicDamage;
    private int appliedArmor;
    private int appliedMaxHealth;
    private int appliedMaxMana;
    private int appliedHeal;
    private int appliedLucky;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        // Пересчитываем бонусы при старте — на случай если инвентарь загрузился раньше нас
        var inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
            RecalculateEquipmentBonuses(inventoryManager.equipmentSlots, inventoryManager);
    }

    public void SetBattleState(bool inBattle)
    {
        IsInBattle = inBattle;
    }

    /// <summary>
    /// Пересчитывает все бонусы от текущей экипировки.
    /// </summary>
    public void RecalculateEquipmentBonuses(InventorySlot[] equipmentSlots, InventoryManager inventoryManager)
    {
        CharacterDataRuntime runtime = GetPlayerRuntime();
        if (runtime == null) return;

        RemoveAppliedBonuses(runtime);

        int newDamage = 0, newMagicDamage = 0, newArmor = 0;
        int newMaxHealth = 0, newMaxMana = 0, newHeal = 0, newLucky = 0;

        foreach (var slot in equipmentSlots)
        {
            var draggable = slot.GetComponentInChildren<DraggableItem>();
            if (draggable == null || draggable.itemData == null) continue;

            ItemData item = draggable.itemData;
            if (!item.IsEquipable()) continue;

            newDamage      += item.bonusDamage;
            newMagicDamage += item.bonusMagicDamage;
            newArmor       += item.bonusArmor;
            newMaxHealth   += item.bonusMaxHealth;
            newMaxMana     += item.bonusMaxMana;
            newHeal        += item.bonusHeal;
            newLucky       += item.bonusLucky;
        }

        runtime._damage      += newDamage;
        runtime._magicDamage += newMagicDamage;
        runtime._armor       += newArmor;
        runtime._maxHealth   += newMaxHealth;
        runtime._maxMana     += newMaxMana;
        runtime._heal        += newHeal;
        runtime._lucky       += newLucky;

        // Корректируем текущие HP/Mana пропорционально изменению максимума
        if (newMaxHealth != 0)
            runtime._health = Mathf.Clamp(runtime._health + newMaxHealth, 1, runtime._maxHealth);
        if (newMaxMana != 0)
            runtime._mana = Mathf.Clamp(runtime._mana + newMaxMana, 0, runtime._maxMana);

        appliedDamage      = newDamage;
        appliedMagicDamage = newMagicDamage;
        appliedArmor       = newArmor;
        appliedMaxHealth   = newMaxHealth;
        appliedMaxMana     = newMaxMana;
        appliedHeal        = newHeal;
        appliedLucky       = newLucky;

        // Обновляем UI команды чтобы отобразить новые значения
        GlobalLoader.Instance?.mainUI?.teamManager?.UpdateTeamUI();

        Debug.Log($"[EquipmentManager] Бонусы применены: DMG+{newDamage} ARM+{newArmor} HP+{newMaxHealth}");
    }

    private void RemoveAppliedBonuses(CharacterDataRuntime runtime)
    {
        runtime._damage      -= appliedDamage;
        runtime._magicDamage -= appliedMagicDamage;
        runtime._armor       -= appliedArmor;
        runtime._maxHealth   -= appliedMaxHealth;
        runtime._maxMana     -= appliedMaxMana;
        runtime._heal        -= appliedHeal;
        runtime._lucky       -= appliedLucky;

        // Клэмпим HP/Mana чтобы не выйти за новый максимум
        runtime._health = Mathf.Clamp(runtime._health, 1, Mathf.Max(1, runtime._maxHealth));
        runtime._mana   = Mathf.Clamp(runtime._mana,   0, Mathf.Max(0, runtime._maxMana));

        appliedDamage = appliedMagicDamage = appliedArmor = 0;
        appliedMaxHealth = appliedMaxMana = appliedHeal = appliedLucky = 0;
    }

    private CharacterDataRuntime GetPlayerRuntime()
    {
        if (GlobalLoader.Instance == null || GlobalLoader.Instance.playerInstance == null)
            return null;

        var team = GlobalLoader.Instance.playerInstance.GetComponent<Team>();
        if (team == null || team.characters.Count == 0) return null;

        foreach (var character in team.characters)
        {
            if (character.characterDataName == PlayerCharacterDataName)
                return character.RuntimeData;
        }

        return team.characters[0].RuntimeData;
    }
}
