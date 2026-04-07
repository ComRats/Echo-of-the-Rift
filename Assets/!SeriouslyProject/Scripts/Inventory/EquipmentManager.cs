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
        var inventoryManager = FindObjectOfType<InventoryManager>();
        if (inventoryManager != null)
            RecalculateEquipmentBonuses(inventoryManager.equipmentSlots, inventoryManager);
    }

    public void SetBattleState(bool inBattle)
    {
        IsInBattle = inBattle;
    }

    public void RecalculateEquipmentBonuses(InventorySlot[] equipmentSlots, InventoryManager inventoryManager)
    {
        CharacterDataRuntime runtime = GetPlayerRuntime();
        if (runtime == null) return;

        int baseHealth = runtime._health;
        int baseMana   = runtime._mana;
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

        int oldMaxHealth = runtime._maxHealth - newMaxHealth;
        int oldMaxMana   = runtime._maxMana   - newMaxMana;

        if (baseHealth >= oldMaxHealth)
            runtime._health = runtime._maxHealth;
        else
            runtime._health = Mathf.Clamp(baseHealth, 1, runtime._maxHealth);

        if (baseMana >= oldMaxMana)
            runtime._mana = runtime._maxMana;
        else
            runtime._mana = Mathf.Clamp(baseMana, 0, runtime._maxMana);

        appliedDamage      = newDamage;
        appliedMagicDamage = newMagicDamage;
        appliedArmor       = newArmor;
        appliedMaxHealth   = newMaxHealth;
        appliedMaxMana     = newMaxMana;
        appliedHeal        = newHeal;
        appliedLucky       = newLucky;

        GlobalLoader.Instance?.mainUI?.teamManager?.UpdateTeamUI();
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

        appliedDamage = appliedMagicDamage = appliedArmor = 0;
        appliedMaxHealth = appliedMaxMana = appliedHeal = appliedLucky = 0;
    }

    public (int damage, int magicDamage, int armor, int maxHealth, int maxMana, int heal, int lucky) GetBaseStats(CharacterDataRuntime runtime)
    {
        return (
            runtime._damage      - appliedDamage,
            runtime._magicDamage - appliedMagicDamage,
            runtime._armor       - appliedArmor,
            runtime._maxHealth   - appliedMaxHealth,
            runtime._maxMana     - appliedMaxMana,
            runtime._heal        - appliedHeal,
            runtime._lucky       - appliedLucky
        );
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
