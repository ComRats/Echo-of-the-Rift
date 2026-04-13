#!/usr/bin/env python3
"""Generate EMX/UML2 XMI for FightSystem - correct UML relationship types"""
import uuid

def uid(name):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.v2." + name)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="FightSystem">')
lines.append('')

pkg_id = uid("PKG_FightSystem")
lines.append(f'  <packagedElement xmi:type="uml:Package" xmi:id="{pkg_id}" name="FightSystem">')

# ─── Helpers ──────────────────────────────────────────────────────────────────
# Deferred relationships collected here, written after all classes
deferred = []

def cls(key, name, abstract=False, stereotype=None):
    eid = uid(key)
    abs_str = ' isAbstract="true"' if abstract else ''
    t = "uml:Interface" if stereotype == "interface" else "uml:Class"
    lines.append(f'    <packagedElement xmi:type="{t}" xmi:id="{eid}" name="{name}" visibility="public"{abs_str}>')

def end_cls():
    lines.append('    </packagedElement>')

def attr(owner_key, name, type_str, vis="private"):
    aid = uid(owner_key + "_a_" + name)
    safe = type_str.replace("<","[").replace(">","]")
    lines.append(f'      <ownedAttribute xmi:id="{aid}" name="{name}" visibility="{vis}" type="{safe}"/>')

def op(owner_key, name, ret="void", vis="public", abstract=False):
    oid = uid(owner_key + "_o_" + name)
    abs_str = ' isAbstract="true"' if abstract else ''
    lines.append(f'      <ownedOperation xmi:id="{oid}" name="{name}" visibility="{vis}"{abs_str}/>')

# ── Relationship builders (deferred to package level) ─────────────────────────
def generalization(child_key, parent_key):
    """Inheritance: child extends parent — solid line with hollow triangle"""
    gid = uid("GEN_" + child_key + "_" + parent_key)
    deferred.append(
        f'    <packagedElement xmi:type="uml:Generalization" xmi:id="{gid}"'
        f' specific="{uid(child_key)}" general="{uid(parent_key)}"/>'
    )

def realization(class_key, iface_key):
    """Interface realization: class implements interface — dashed line with hollow triangle"""
    rid = uid("REAL_" + class_key + "_" + iface_key)
    deferred.append(
        f'    <packagedElement xmi:type="uml:InterfaceRealization" xmi:id="{rid}"'
        f' implementingClassifier="{uid(class_key)}" contract="{uid(iface_key)}"/>'
    )

def association(src, tgt, role, aggr="none", mult="1"):
    """Association / Aggregation / Composition — field reference"""
    aid = uid(f"ASSOC_{src}_{tgt}_{role}")
    e1 = uid(aid + "_e1")
    e2 = uid(aid + "_e2")
    label = f"{src}__{role}"
    deferred.append(f'    <packagedElement xmi:type="uml:Association" xmi:id="{aid}" name="{label}">')
    deferred.append(f'      <ownedEnd xmi:id="{e1}" type="{uid(src)}" aggregation="{aggr}" isNavigable="false"/>')
    deferred.append(f'      <ownedEnd xmi:id="{e2}" name="{role}" type="{uid(tgt)}" isNavigable="true" multiplicity="{mult}"/>')
    deferred.append('    </packagedElement>')

def dependency(src, tgt, label="uses"):
    """Dependency — dashed arrow, uses/creates relationship"""
    did = uid(f"DEP_{src}_{tgt}_{label}")
    deferred.append(
        f'    <packagedElement xmi:type="uml:Dependency" xmi:id="{did}"'
        f' name="{label}" client="{uid(src)}" supplier="{uid(tgt)}"/>'
    )

# ══════════════════════════════════════════════════════════════════════════════
# CLASSES
# ══════════════════════════════════════════════════════════════════════════════

# ─── IData (interface) ────────────────────────────────────────────────────────
cls("IData", "IData", stereotype="interface")
for p in ["Name","Description","Sprite","Damage","MagicDamage","Priority",
          "MaxMana","Mana","MaxHealth","Health","Heal","Armor","Lucky",
          "CreteDamage","AttackAnimationName","Level","CurrentXP","MaxXP",
          "XpReward","DamagePerLevel","MaxHealthPerLevel","HealPerLevel",
          "ArmorPerLevel","MaxManaPerLevel","XpRewardPerLevel"]:
    attr("IData", p, "property", "public")
end_cls()

# ─── EntityStats ──────────────────────────────────────────────────────────────
cls("EntityStats", "EntityStats")
for f in ["_name","_description","_damage","_magicDamage","_priority",
          "_maxMana","_mana","_maxHealth","_health","_heal","_armor",
          "_lucky","_creteDamage","_attackAnimationName","_level",
          "_currentXP","_maxXP","_xpReward","_damagePerLevel",
          "_maxHealthPerLevel","_healPerLevel","_armorPerLevel",
          "_maxManaPerLevel","_xpRewardPerLevel"]:
    attr("EntityStats", f, "field", "protected")
op("EntityStats", "CaptureBaseStats")
op("EntityStats", "RecalculateStats")
end_cls()
realization("EntityStats", "IData")

# ─── StatusEffectSO ───────────────────────────────────────────────────────────
cls("StatusEffectSO", "StatusEffectSO")
attr("StatusEffectSO", "effectName", "string", "public")
attr("StatusEffectSO", "damagePerTurn", "int", "public")
attr("StatusEffectSO", "duration", "int", "public")
attr("StatusEffectSO", "tickColor", "Color", "public")
attr("StatusEffectSO", "armorBonus", "int", "public")
attr("StatusEffectSO", "vfxPrefab", "GameObject", "public")
end_cls()

# ─── ActiveStatusEffect ───────────────────────────────────────────────────────
cls("ActiveStatusEffect", "ActiveStatusEffect")
attr("ActiveStatusEffect", "data", "StatusEffectSO", "public")
attr("ActiveStatusEffect", "remainingTurns", "int", "public")
attr("ActiveStatusEffect", "vfxInstance", "GameObject", "public")
end_cls()
# ActiveStatusEffect → StatusEffectSO (composition: owns data)
association("ActiveStatusEffect", "StatusEffectSO", "data", "composite")

# ─── Base ─────────────────────────────────────────────────────────────────────
cls("Base", "Base")
attr("Base", "stats", "EntityStats", "private")
attr("Base", "activeEffects", "List[ActiveStatusEffect]", "public")
attr("Base", "IsBlinking", "bool", "public")
attr("Base", "healthText", "TextMeshProUGUI", "public")
attr("Base", "healthBar", "Slider", "public")
attr("Base", "manaText", "TextMeshProUGUI", "public")
attr("Base", "manaBar", "Slider", "public")
attr("Base", "animator", "Animator", "private")
attr("Base", "textPrefab", "GameObject", "public")
op("Base", "Initialize")
op("Base", "TakeDamage")
op("Base", "TakeMagicDamage")
op("Base", "TakeHeal")
op("Base", "GiveDamage", "int")
op("Base", "GiveHeal", "int")
op("Base", "PlayAnimation")
op("Base", "SetAnimationSpeed")
op("Base", "ApplyStatusEffect")
op("Base", "ProcessStatusEffects")
op("Base", "UpdateUI")
op("Base", "GetXP")
op("Base", "TryDeath")
op("Base", "Blinking")
end_cls()
realization("Base", "IData")
# Base → EntityStats (composition: owns stats)
association("Base", "EntityStats", "stats", "composite")
# Base → ActiveStatusEffect (composition: owns list)
association("Base", "ActiveStatusEffect", "activeEffects", "composite", "0..*")

# ─── CharacterData ────────────────────────────────────────────────────────────
cls("CharacterData", "CharacterData")
attr("CharacterData", "_name", "string", "public")
attr("CharacterData", "_damage", "int", "public")
attr("CharacterData", "_maxHealth", "int", "public")
attr("CharacterData", "_level", "int", "public")
attr("CharacterData", "_abilitySet", "CharacterAbilitySet", "public")
op("CharacterData", "ResetToDefaults")
op("CharacterData", "ValidateAndFixData")
end_cls()
realization("CharacterData", "IData")
# CharacterData → CharacterAbilitySet (composition)
association("CharacterData", "CharacterAbilitySet", "AbilitySet", "composite")

# ─── CharacterDataRuntime ─────────────────────────────────────────────────────
cls("CharacterDataRuntime", "CharacterDataRuntime")
attr("CharacterDataRuntime", "originalData", "CharacterData", "public")
attr("CharacterDataRuntime", "_abilitySet", "CharacterAbilitySet", "public")
attr("CharacterDataRuntime", "spritePath", "string", "public")
op("CharacterDataRuntime", "CreateFromScriptableObject", "CharacterDataRuntime")
op("CharacterDataRuntime", "ValidateAndFixData")
end_cls()
realization("CharacterDataRuntime", "IData")
# CharacterDataRuntime → CharacterData (association: created from)
association("CharacterDataRuntime", "CharacterData", "originalData")
# CharacterDataRuntime → CharacterAbilitySet
association("CharacterDataRuntime", "CharacterAbilitySet", "_abilitySet")

# ─── EnemyData ────────────────────────────────────────────────────────────────
cls("EnemyData", "EnemyData")
attr("EnemyData", "_name", "string", "public")
attr("EnemyData", "_damage", "int", "public")
attr("EnemyData", "_maxHealth", "int", "public")
attr("EnemyData", "_attackAnimationName", "string", "public")
end_cls()
realization("EnemyData", "IData")

# ─── BattleAbility (abstract) ─────────────────────────────────────────────────
cls("BattleAbility", "BattleAbility", abstract=True)
attr("BattleAbility", "AbilityName", "string", "public")
attr("BattleAbility", "ManaCost", "int", "public")
attr("BattleAbility", "targetType", "TargetType", "public")
attr("BattleAbility", "animTrigger", "string", "public")
attr("BattleAbility", "vfxPrefab", "GameObject", "public")
attr("BattleAbility", "Description", "string", "public")
op("BattleAbility", "Execute", abstract=True)
op("BattleAbility", "CanUse", "bool")
op("BattleAbility", "PlayHitAnimation", vis="protected")
op("BattleAbility", "SpawnVFX", vis="protected")
end_cls()

# ─── Concrete Abilities ───────────────────────────────────────────────────────
cls("MeleeAbility", "MeleeAbility")
attr("MeleeAbility", "baseDamageMultiplier", "int", "private")
attr("MeleeAbility", "flatDamageBonus", "int", "private")
attr("MeleeAbility", "hasStatusEffect", "bool", "private")
attr("MeleeAbility", "statusEffect", "StatusEffectSO", "private")
attr("MeleeAbility", "chanceToApply", "float", "private")
op("MeleeAbility", "Execute")
end_cls()
generalization("MeleeAbility", "BattleAbility")
association("MeleeAbility", "StatusEffectSO", "statusEffect")

cls("MagicAbility", "MagicAbility")
attr("MagicAbility", "magicDamage", "int", "private")
attr("MagicAbility", "flatDamageBonus", "int", "private")
attr("MagicAbility", "statusEffect", "StatusEffectSO", "private")
attr("MagicAbility", "chance", "float", "private")
op("MagicAbility", "Execute")
end_cls()
generalization("MagicAbility", "BattleAbility")
association("MagicAbility", "StatusEffectSO", "statusEffect")

cls("HealAbility", "HealAbility")
attr("HealAbility", "baseHealAmount", "int", "private")
attr("HealAbility", "useCharacterHealStat", "bool", "private")
attr("HealAbility", "healMultiplier", "float", "private")
attr("HealAbility", "healOverTimeEffect", "StatusEffectSO", "private")
op("HealAbility", "Execute")
end_cls()
generalization("HealAbility", "BattleAbility")
association("HealAbility", "StatusEffectSO", "healOverTimeEffect")

cls("DefenseAbility", "DefenseAbility")
attr("DefenseAbility", "bonusDefense", "int", "private")
attr("DefenseAbility", "duration", "int", "private")
attr("DefenseAbility", "defenseEffect", "StatusEffectSO", "private")
op("DefenseAbility", "Execute")
end_cls()
generalization("DefenseAbility", "BattleAbility")
association("DefenseAbility", "StatusEffectSO", "defenseEffect")

cls("StatusAbility", "StatusAbility")
attr("StatusAbility", "effect", "StatusEffectSO", "public")
attr("StatusAbility", "damageMultiplier", "float", "public")
op("StatusAbility", "Execute")
end_cls()
generalization("StatusAbility", "BattleAbility")
association("StatusAbility", "StatusEffectSO", "effect")

# ─── CharacterAbility ─────────────────────────────────────────────────────────
cls("CharacterAbility", "CharacterAbility")
attr("CharacterAbility", "ability", "BattleAbility", "public")
attr("CharacterAbility", "requiredLevel", "int", "public")
attr("CharacterAbility", "abilityType", "AbilityType", "public")
attr("CharacterAbility", "abilityIcon", "Sprite", "public")
attr("CharacterAbility", "isActiveByDefault", "bool", "public")
op("CharacterAbility", "IsUnlocked", "bool")
end_cls()
# CharacterAbility → BattleAbility (association)
association("CharacterAbility", "BattleAbility", "ability")

# ─── CharacterAbilitySet ──────────────────────────────────────────────────────
cls("CharacterAbilitySet", "CharacterAbilitySet")
attr("CharacterAbilitySet", "abilities", "List[CharacterAbility]", "public")
attr("CharacterAbilitySet", "activeAbilityIndices", "List[int]", "public")
op("CharacterAbilitySet", "GetUnlockedAbilities", "List[CharacterAbility]")
op("CharacterAbilitySet", "GetActiveAbilities", "List[CharacterAbility]")
op("CharacterAbilitySet", "GetAbilitiesByType", "List[CharacterAbility]")
op("CharacterAbilitySet", "ActivateAbility")
op("CharacterAbilitySet", "DeactivateAbility")
end_cls()
# CharacterAbilitySet → CharacterAbility (composition 0..*)
association("CharacterAbilitySet", "CharacterAbility", "abilities", "composite", "0..*")

# ─── Character ────────────────────────────────────────────────────────────────
cls("Character", "Character")
attr("Character", "IsTurn", "bool", "public")
attr("Character", "characterData", "CharacterData", "private")
attr("Character", "AbilitySet", "CharacterAbilitySet", "public")
attr("Character", "actionButtons", "ActionButtons", "private")
op("Character", "InitializeFromSettings")
end_cls()
generalization("Character", "Base")
association("Character", "CharacterData", "characterData")
association("Character", "CharacterAbilitySet", "AbilitySet")
dependency("Character", "ActionButtons", "uses")

# ─── Enemy ────────────────────────────────────────────────────────────────────
cls("Enemy", "Enemy")
attr("Enemy", "enemyData", "EnemyData", "private")
attr("Enemy", "actionButtons", "ActionButtons", "private")
op("Enemy", "InitializeFromSettings")
op("Enemy", "LocalInizialize")
end_cls()
generalization("Enemy", "Base")
association("Enemy", "EnemyData", "enemyData")
dependency("Enemy", "ActionButtons", "uses")

# ─── EnemiesSettings ──────────────────────────────────────────────────────────
cls("EnemiesSettings", "EnemiesSettings")
attr("EnemiesSettings", "useEnemyData", "bool", "public")
attr("EnemiesSettings", "enemyDataName", "string", "public")
attr("EnemiesSettings", "enemyData", "EnemyData", "public")
attr("EnemiesSettings", "spritePath", "string", "public")
op("EnemiesSettings", "GetSprite", "Sprite")
op("EnemiesSettings", "GetEnemyData", "EnemyData")
end_cls()
realization("EnemiesSettings", "IData")
association("EnemiesSettings", "EnemyData", "enemyData")

# ─── AbilityButton ────────────────────────────────────────────────────────────
cls("AbilityButton", "AbilityButton")
attr("AbilityButton", "ability", "BattleAbility", "private")
attr("AbilityButton", "character", "Base", "private")
attr("AbilityButton", "actionButtons", "ActionButtons", "private")
attr("AbilityButton", "abilityNameText", "TextMeshProUGUI", "private")
attr("AbilityButton", "manaCostText", "TextMeshProUGUI", "private")
op("AbilityButton", "Setup")
op("AbilityButton", "UpdateVisuals")
op("AbilityButton", "GetAbility", "BattleAbility")
end_cls()
association("AbilityButton", "BattleAbility", "ability")
association("AbilityButton", "Base", "character")
dependency("AbilityButton", "ActionButtons", "uses")

# ─── AbilityManager ───────────────────────────────────────────────────────────
cls("AbilityManager", "AbilityManager")
attr("AbilityManager", "fightManager", "FightManager", "private")
attr("AbilityManager", "currentCharacter", "Character", "private")
attr("AbilityManager", "physicalAbilitiesContainer", "Transform", "private")
attr("AbilityManager", "magicAbilitiesContainer", "Transform", "private")
attr("AbilityManager", "defenseAbilitiesContainer", "Transform", "private")
attr("AbilityManager", "supportAbilitiesContainer", "Transform", "private")
op("AbilityManager", "SetupAbilitiesForCharacter")
op("AbilityManager", "CreateAbilityButton", vis="private")
op("AbilityManager", "RefreshAbilities")
end_cls()
association("AbilityManager", "FightManager", "fightManager")
association("AbilityManager", "Character", "currentCharacter")
dependency("AbilityManager", "CharacterAbility", "creates")

# ─── ActionButtons ────────────────────────────────────────────────────────────
cls("ActionButtons", "ActionButtons")
attr("ActionButtons", "currentEnemy", "Enemy", "public")
attr("ActionButtons", "fightManager", "FightManager", "private")
attr("ActionButtons", "sceneLoader", "SceneLoader", "private")
attr("ActionButtons", "abilityManager", "AbilityManager", "private")
attr("ActionButtons", "pendingAbility", "BattleAbility", "private")
attr("ActionButtons", "pendingAttacker", "Base", "private")
attr("ActionButtons", "descriptionText", "TextMeshProUGUI", "private")
op("ActionButtons", "OnEnemySelected")
op("ActionButtons", "OnCharacterSelected")
op("ActionButtons", "SetPendingAbility")
op("ActionButtons", "ShowAbilityDescription")
op("ActionButtons", "ClearDescription")
op("ActionButtons", "EscapeFight")
op("ActionButtons", "OpenInventory")
end_cls()
association("ActionButtons", "FightManager", "fightManager")
association("ActionButtons", "Enemy", "currentEnemy")
association("ActionButtons", "AbilityManager", "abilityManager")
association("ActionButtons", "BattleAbility", "pendingAbility")
association("ActionButtons", "Base", "pendingAttacker")

# ─── FightManager ─────────────────────────────────────────────────────────────
cls("FightManager", "FightManager")
attr("FightManager", "ActiveCharacter", "Character", "public")
attr("FightManager", "enemies", "List[Enemy]", "public")
attr("FightManager", "characters", "List[Character]", "public")
attr("FightManager", "bases", "List[Base]", "private")
attr("FightManager", "abilityManager", "AbilityManager", "private")
attr("FightManager", "battleTeamSync", "BattleTeamSync", "private")
attr("FightManager", "gameSettings", "GameSettings", "private")
op("FightManager", "StartFight", vis="private")
op("FightManager", "EndFight", vis="private")
op("FightManager", "WaitCharacterTurn", vis="private")
op("FightManager", "GetCharacterLowestHP", "Character")
op("FightManager", "DeleteEnemyOnList")
op("FightManager", "StopEnemyBlinking")
op("FightManager", "StartEnemyBlinking")
end_cls()
# FightManager → Character (aggregation 0..*)
association("FightManager", "Character", "characters", "shared", "0..*")
# FightManager → Enemy (aggregation 0..*)
association("FightManager", "Enemy", "enemies", "shared", "0..*")
# FightManager → Base (aggregation 0..*)
association("FightManager", "Base", "bases", "shared", "0..*")
association("FightManager", "AbilityManager", "abilityManager")
association("FightManager", "BattleTeamSync", "battleTeamSync")

# ─── BattleTeamSync ───────────────────────────────────────────────────────────
cls("BattleTeamSync", "BattleTeamSync")
attr("BattleTeamSync", "fightManager", "FightManager", "private")
attr("BattleTeamSync", "initialCharacters", "Dictionary[string,Character]", "private")
op("BattleTeamSync", "OnTeamManagerReady")
op("BattleTeamSync", "LinkTeamManagerWithBattle", vis="private")
op("BattleTeamSync", "SyncTeamAfterBattle")
end_cls()
association("BattleTeamSync", "FightManager", "fightManager")
association("BattleTeamSync", "Character", "initialCharacters", "none", "0..*")
# BattleTeamSync depends on GlobalLoader at runtime
dependency("BattleTeamSync", "GlobalLoader", "uses")

# ─── FightTrigger ─────────────────────────────────────────────────────────────
cls("FightTrigger", "FightTrigger")
attr("FightTrigger", "canTriggered", "bool", "public")
attr("FightTrigger", "sceneLoader", "SceneLoader", "private")
attr("FightTrigger", "enemies", "List[EnemiesSettings]", "private")
op("FightTrigger", "OnTriggerEnter2D", vis="private")
op("FightTrigger", "StartFight", vis="private")
op("FightTrigger", "SaveEnemiesToFile", vis="private")
end_cls()
association("FightTrigger", "EnemiesSettings", "enemies", "composite", "0..*")
dependency("FightTrigger", "GlobalLoader", "uses")

# ─── FightDataLoader ──────────────────────────────────────────────────────────
cls("FightDataLoader", "FightDataLoader")
attr("FightDataLoader", "enemyPrefab", "GameObject", "private")
attr("FightDataLoader", "characterPrefab", "GameObject", "private")
op("FightDataLoader", "LoadFightData", vis="private")
op("FightDataLoader", "LoadCharactersData", vis="private")
end_cls()
dependency("FightDataLoader", "Enemy", "creates")
dependency("FightDataLoader", "Character", "creates")
dependency("FightDataLoader", "GlobalLoader", "uses")

# ─── GlobalLoader (stub — external dependency) ────────────────────────────────
cls("GlobalLoader", "GlobalLoader")
attr("GlobalLoader", "Instance", "GlobalLoader", "public")
attr("GlobalLoader", "playerInstance", "Player", "public")
attr("GlobalLoader", "mainUI", "MainUI", "public")
op("GlobalLoader", "LoadToScene")
op("GlobalLoader", "SavePlayer")
op("GlobalLoader", "Hide")
op("GlobalLoader", "Show")
end_cls()

# ─── Enumerations ─────────────────────────────────────────────────────────────
for ename, elits in [
    ("TargetType",  ["Enemy","Ally","Self","AllEnemies","AllAllies"]),
    ("AbilityType", ["Physical","Magic","Defense","Support"]),
    ("StateEffect", ["None","Fire","Water","Air","Ground"]),
    ("FightResult", ["None","Win","Lose","Escape"]),
]:
    eid = uid(ename)
    lines.append(f'    <packagedElement xmi:type="uml:Enumeration" xmi:id="{eid}" name="{ename}" visibility="public">')
    for lit in elits:
        lines.append(f'      <ownedLiteral xmi:id="{uid(ename+lit)}" name="{lit}"/>')
    lines.append('    </packagedElement>')

# ── Write deferred relationships inside package ───────────────────────────────
for rel in deferred:
    lines.append(rel)

lines.append(f'  </packagedElement>')  # close FightSystem package
lines.append('')
lines.append('</uml:Model>')

content = "\n".join(lines)
with open("EchoRift_FightSystem.xml", "w", encoding="utf-8") as f:
    f.write(content)

import xml.etree.ElementTree as ET
try:
    ET.parse("EchoRift_FightSystem.xml")
    gen_count   = sum(1 for l in lines if 'uml:Generalization"'      in l)
    real_count  = sum(1 for l in lines if 'uml:InterfaceRealization"' in l)
    assoc_count = sum(1 for l in lines if 'uml:Association"'          in l)
    dep_count   = sum(1 for l in lines if 'uml:Dependency"'           in l)
    cls_count   = sum(1 for l in lines if ('uml:Class"' in l or 'uml:Interface"' in l or 'uml:Enumeration"' in l))
    print(f"XML valid!  {len(lines)} lines")
    print(f"  Classes/Interfaces/Enums : {cls_count}")
    print(f"  Generalization           : {gen_count}")
    print(f"  InterfaceRealization     : {real_count}")
    print(f"  Association              : {assoc_count}")
    print(f"  Dependency               : {dep_count}")
except ET.ParseError as e:
    src = content.split('\n')
    ln = e.position[0]
    print("ERROR:", e)
    print("Line", ln, ":", repr(src[ln-1]))

def uid(name):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift." + name)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="FightSystem">')
lines.append('')

pkg_id = uid("PKG_FightSystem")
lines.append(f'  <packagedElement xmi:type="uml:Package" xmi:id="{pkg_id}" name="FightSystem">')

# ─── Helper ───────────────────────────────────────────────────────────────────
def cls(key, name, abstract=False, stereotype=None):
    eid = uid(key)
    abs_str = ' isAbstract="true"' if abstract else ''
    t = "uml:Interface" if stereotype == "interface" else "uml:Class"
    lines.append(f'    <packagedElement xmi:type="{t}" xmi:id="{eid}" name="{name}" visibility="public"{abs_str}>')

def end_cls():
    lines.append('    </packagedElement>')

def attr(owner_key, name, type_str, vis="private"):
    aid = uid(owner_key + "_a_" + name)
    safe_type = type_str.replace("<","[").replace(">","]")
    lines.append(f'      <ownedAttribute xmi:id="{aid}" name="{name}" visibility="{vis}" type="{safe_type}"/>')

def op(owner_key, name, ret="void", vis="public", abstract=False):
    oid = uid(owner_key + "_o_" + name)
    abs_str = ' isAbstract="true"' if abstract else ''
    lines.append(f'      <ownedOperation xmi:id="{oid}" name="{name}" visibility="{vis}"{abs_str}/>')

def generalization(child_key, parent_key):
    gid = uid("GEN_" + child_key + "_" + parent_key)
    lines.append(f'      <generalization xmi:id="{gid}" general="{uid(parent_key)}"/>')

def iface_real(class_key, iface_key):
    rid = uid("REAL_" + class_key + "_" + iface_key)
    lines.append(f'      <interfaceRealization xmi:id="{rid}" supplier="{uid(iface_key)}" client="{uid(class_key)}"/>')

# ─── INTERFACE IData ──────────────────────────────────────────────────────────
cls("IData", "IData", stereotype="interface")
for p in ["Name","Description","Sprite","Damage","MagicDamage","Priority",
          "MaxMana","Mana","MaxHealth","Health","Heal","Armor","Lucky",
          "CreteDamage","AttackAnimationName","Level","CurrentXP","MaxXP",
          "XpReward","DamagePerLevel","MaxHealthPerLevel","HealPerLevel",
          "ArmorPerLevel","MaxManaPerLevel","XpRewardPerLevel"]:
    attr("IData", p, "property", "public")
end_cls()

# ─── EntityStats ──────────────────────────────────────────────────────────────
cls("EntityStats", "EntityStats")
iface_real("EntityStats", "IData")
for f in ["_name","_description","_damage","_magicDamage","_priority",
          "_maxMana","_mana","_maxHealth","_health","_heal","_armor",
          "_lucky","_creteDamage","_attackAnimationName","_level",
          "_currentXP","_maxXP","_xpReward","_damagePerLevel",
          "_maxHealthPerLevel","_healPerLevel","_armorPerLevel",
          "_maxManaPerLevel","_xpRewardPerLevel"]:
    attr("EntityStats", f, "field", "protected")
op("EntityStats", "CaptureBaseStats")
op("EntityStats", "RecalculateStats")
end_cls()

# ─── StatusEffectSO ───────────────────────────────────────────────────────────
cls("StatusEffectSO", "StatusEffectSO")
attr("StatusEffectSO", "effectName", "string", "public")
attr("StatusEffectSO", "damagePerTurn", "int", "public")
attr("StatusEffectSO", "duration", "int", "public")
attr("StatusEffectSO", "tickColor", "Color", "public")
attr("StatusEffectSO", "armorBonus", "int", "public")
attr("StatusEffectSO", "vfxPrefab", "GameObject", "public")
end_cls()

# ─── ActiveStatusEffect ───────────────────────────────────────────────────────
cls("ActiveStatusEffect", "ActiveStatusEffect")
attr("ActiveStatusEffect", "data", "StatusEffectSO", "public")
attr("ActiveStatusEffect", "remainingTurns", "int", "public")
attr("ActiveStatusEffect", "vfxInstance", "GameObject", "public")
end_cls()

# ─── Base ─────────────────────────────────────────────────────────────────────
cls("Base", "Base")
iface_real("Base", "IData")
attr("Base", "stats", "EntityStats", "private")
attr("Base", "activeEffects", "List<ActiveStatusEffect>", "public")
attr("Base", "IsBlinking", "bool", "public")
attr("Base", "healthText", "TextMeshProUGUI", "public")
attr("Base", "healthBar", "Slider", "public")
attr("Base", "manaText", "TextMeshProUGUI", "public")
attr("Base", "manaBar", "Slider", "public")
attr("Base", "animator", "Animator", "private")
attr("Base", "textPrefab", "GameObject", "public")
op("Base", "Initialize")
op("Base", "TakeDamage")
op("Base", "TakeMagicDamage")
op("Base", "TakeHeal")
op("Base", "GiveDamage", "int")
op("Base", "GiveHeal", "int")
op("Base", "PlayAnimation")
op("Base", "SetAnimationSpeed")
op("Base", "ApplyStatusEffect")
op("Base", "ProcessStatusEffects")
op("Base", "UpdateUI")
op("Base", "GetXP")
op("Base", "TryDeath")
op("Base", "Blinking")
end_cls()

# ─── CharacterData ────────────────────────────────────────────────────────────
cls("CharacterData", "CharacterData")
iface_real("CharacterData", "IData")
attr("CharacterData", "_abilitySet", "CharacterAbilitySet", "public")
attr("CharacterData", "_name", "string", "public")
attr("CharacterData", "_damage", "int", "public")
attr("CharacterData", "_maxHealth", "int", "public")
attr("CharacterData", "_level", "int", "public")
op("CharacterData", "ResetToDefaults")
op("CharacterData", "ValidateAndFixData")
end_cls()

# ─── CharacterDataRuntime ─────────────────────────────────────────────────────
cls("CharacterDataRuntime", "CharacterDataRuntime")
iface_real("CharacterDataRuntime", "IData")
attr("CharacterDataRuntime", "originalData", "CharacterData", "public")
attr("CharacterDataRuntime", "_abilitySet", "CharacterAbilitySet", "public")
attr("CharacterDataRuntime", "spritePath", "string", "public")
op("CharacterDataRuntime", "CreateFromScriptableObject", "CharacterDataRuntime")
op("CharacterDataRuntime", "ValidateAndFixData")
end_cls()

# ─── EnemyData ────────────────────────────────────────────────────────────────
cls("EnemyData", "EnemyData")
iface_real("EnemyData", "IData")
attr("EnemyData", "_name", "string", "public")
attr("EnemyData", "_damage", "int", "public")
attr("EnemyData", "_maxHealth", "int", "public")
attr("EnemyData", "_attackAnimationName", "string", "public")
end_cls()

# ─── BattleAbility ────────────────────────────────────────────────────────────
cls("BattleAbility", "BattleAbility", abstract=True)
attr("BattleAbility", "AbilityName", "string", "public")
attr("BattleAbility", "ManaCost", "int", "public")
attr("BattleAbility", "targetType", "TargetType", "public")
attr("BattleAbility", "animTrigger", "string", "public")
attr("BattleAbility", "vfxPrefab", "GameObject", "public")
attr("BattleAbility", "Description", "string", "public")
op("BattleAbility", "Execute", abstract=True)
op("BattleAbility", "CanUse", "bool")
op("BattleAbility", "PlayHitAnimation", vis="protected")
op("BattleAbility", "SpawnVFX", vis="protected")
end_cls()

# ─── Concrete Abilities ───────────────────────────────────────────────────────
cls("MeleeAbility", "MeleeAbility")
generalization("MeleeAbility", "BattleAbility")
attr("MeleeAbility", "baseDamageMultiplier", "int", "private")
attr("MeleeAbility", "flatDamageBonus", "int", "private")
attr("MeleeAbility", "hasStatusEffect", "bool", "private")
attr("MeleeAbility", "statusEffect", "StatusEffectSO", "private")
attr("MeleeAbility", "chanceToApply", "float", "private")
op("MeleeAbility", "Execute")
end_cls()

cls("MagicAbility", "MagicAbility")
generalization("MagicAbility", "BattleAbility")
attr("MagicAbility", "magicDamage", "int", "private")
attr("MagicAbility", "flatDamageBonus", "int", "private")
attr("MagicAbility", "statusEffect", "StatusEffectSO", "private")
attr("MagicAbility", "chance", "float", "private")
op("MagicAbility", "Execute")
end_cls()

cls("HealAbility", "HealAbility")
generalization("HealAbility", "BattleAbility")
attr("HealAbility", "baseHealAmount", "int", "private")
attr("HealAbility", "useCharacterHealStat", "bool", "private")
attr("HealAbility", "healMultiplier", "float", "private")
attr("HealAbility", "healOverTimeEffect", "StatusEffectSO", "private")
op("HealAbility", "Execute")
end_cls()

cls("DefenseAbility", "DefenseAbility")
generalization("DefenseAbility", "BattleAbility")
attr("DefenseAbility", "bonusDefense", "int", "private")
attr("DefenseAbility", "duration", "int", "private")
attr("DefenseAbility", "defenseEffect", "StatusEffectSO", "private")
op("DefenseAbility", "Execute")
end_cls()

cls("StatusAbility", "StatusAbility")
generalization("StatusAbility", "BattleAbility")
attr("StatusAbility", "effect", "StatusEffectSO", "public")
attr("StatusAbility", "damageMultiplier", "float", "public")
op("StatusAbility", "Execute")
end_cls()

# ─── CharacterAbility ─────────────────────────────────────────────────────────
cls("CharacterAbility", "CharacterAbility")
attr("CharacterAbility", "ability", "BattleAbility", "public")
attr("CharacterAbility", "requiredLevel", "int", "public")
attr("CharacterAbility", "abilityType", "AbilityType", "public")
attr("CharacterAbility", "abilityIcon", "Sprite", "public")
attr("CharacterAbility", "isActiveByDefault", "bool", "public")
op("CharacterAbility", "IsUnlocked", "bool")
end_cls()

# ─── CharacterAbilitySet ──────────────────────────────────────────────────────
cls("CharacterAbilitySet", "CharacterAbilitySet")
attr("CharacterAbilitySet", "abilities", "List<CharacterAbility>", "public")
attr("CharacterAbilitySet", "activeAbilityIndices", "List<int>", "public")
op("CharacterAbilitySet", "GetUnlockedAbilities", "List<CharacterAbility>")
op("CharacterAbilitySet", "GetActiveAbilities", "List<CharacterAbility>")
op("CharacterAbilitySet", "GetAbilitiesByType", "List<CharacterAbility>")
op("CharacterAbilitySet", "ActivateAbility")
op("CharacterAbilitySet", "DeactivateAbility")
end_cls()

# ─── Character ────────────────────────────────────────────────────────────────
cls("Character", "Character")
generalization("Character", "Base")
attr("Character", "IsTurn", "bool", "public")
attr("Character", "characterData", "CharacterData", "private")
attr("Character", "AbilitySet", "CharacterAbilitySet", "public")
attr("Character", "actionButtons", "ActionButtons", "private")
op("Character", "InitializeFromSettings")
end_cls()

# ─── Enemy ────────────────────────────────────────────────────────────────────
cls("Enemy", "Enemy")
generalization("Enemy", "Base")
attr("Enemy", "enemyData", "EnemyData", "private")
attr("Enemy", "actionButtons", "ActionButtons", "private")
op("Enemy", "InitializeFromSettings")
op("Enemy", "LocalInizialize")
end_cls()

# ─── EnemiesSettings ──────────────────────────────────────────────────────────
cls("EnemiesSettings", "EnemiesSettings")
iface_real("EnemiesSettings", "IData")
attr("EnemiesSettings", "useEnemyData", "bool", "public")
attr("EnemiesSettings", "enemyDataName", "string", "public")
attr("EnemiesSettings", "enemyData", "EnemyData", "public")
attr("EnemiesSettings", "spritePath", "string", "public")
op("EnemiesSettings", "GetSprite", "Sprite")
op("EnemiesSettings", "GetEnemyData", "EnemyData")
end_cls()

# ─── AbilityButton ────────────────────────────────────────────────────────────
cls("AbilityButton", "AbilityButton")
attr("AbilityButton", "ability", "BattleAbility", "private")
attr("AbilityButton", "character", "Base", "private")
attr("AbilityButton", "actionButtons", "ActionButtons", "private")
attr("AbilityButton", "abilityNameText", "TextMeshProUGUI", "private")
attr("AbilityButton", "manaCostText", "TextMeshProUGUI", "private")
attr("AbilityButton", "abilityIcon", "Image", "private")
op("AbilityButton", "Setup")
op("AbilityButton", "UpdateVisuals")
op("AbilityButton", "GetAbility", "BattleAbility")
end_cls()

# ─── AbilityManager ───────────────────────────────────────────────────────────
cls("AbilityManager", "AbilityManager")
attr("AbilityManager", "fightManager", "FightManager", "private")
attr("AbilityManager", "currentCharacter", "Character", "private")
attr("AbilityManager", "buttonAbilityMap", "Dictionary<Button,BattleAbility>", "private")
attr("AbilityManager", "physicalAbilitiesContainer", "Transform", "private")
attr("AbilityManager", "magicAbilitiesContainer", "Transform", "private")
attr("AbilityManager", "defenseAbilitiesContainer", "Transform", "private")
attr("AbilityManager", "supportAbilitiesContainer", "Transform", "private")
op("AbilityManager", "SetupAbilitiesForCharacter")
op("AbilityManager", "CreateAbilityButton", vis="private")
op("AbilityManager", "RefreshAbilities")
end_cls()

# ─── ActionButtons ────────────────────────────────────────────────────────────
cls("ActionButtons", "ActionButtons")
attr("ActionButtons", "currentEnemy", "Enemy", "public")
attr("ActionButtons", "fightManager", "FightManager", "private")
attr("ActionButtons", "sceneLoader", "SceneLoader", "private")
attr("ActionButtons", "abilityManager", "AbilityManager", "private")
attr("ActionButtons", "pendingAbility", "BattleAbility", "private")
attr("ActionButtons", "pendingAttacker", "Base", "private")
attr("ActionButtons", "descriptionText", "TextMeshProUGUI", "private")
op("ActionButtons", "OnEnemySelected")
op("ActionButtons", "OnCharacterSelected")
op("ActionButtons", "SetPendingAbility")
op("ActionButtons", "ShowAbilityDescription")
op("ActionButtons", "ClearDescription")
op("ActionButtons", "EscapeFight")
op("ActionButtons", "OpenInventory")
end_cls()

# ─── FightManager ─────────────────────────────────────────────────────────────
cls("FightManager", "FightManager")
attr("FightManager", "ActiveCharacter", "Character", "public")
attr("FightManager", "enemies", "List<Enemy>", "public")
attr("FightManager", "characters", "List<Character>", "public")
attr("FightManager", "bases", "List<Base>", "private")
attr("FightManager", "abilityManager", "AbilityManager", "private")
attr("FightManager", "battleTeamSync", "BattleTeamSync", "private")
attr("FightManager", "gameSettings", "GameSettings", "private")
op("FightManager", "StartFight", vis="private")
op("FightManager", "EndFight", vis="private")
op("FightManager", "WaitCharacterTurn", vis="private")
op("FightManager", "GetCharacterLowestHP", "Character")
op("FightManager", "DeleteEnemyOnList")
op("FightManager", "StopEnemyBlinking")
op("FightManager", "StartEnemyBlinking")
end_cls()

# ─── BattleTeamSync ───────────────────────────────────────────────────────────
cls("BattleTeamSync", "BattleTeamSync")
attr("BattleTeamSync", "fightManager", "FightManager", "private")
attr("BattleTeamSync", "initialCharacters", "Dictionary<string,Character>", "private")
op("BattleTeamSync", "OnTeamManagerReady")
op("BattleTeamSync", "LinkTeamManagerWithBattle", vis="private")
op("BattleTeamSync", "SyncTeamAfterBattle")
end_cls()

# ─── FightTrigger ─────────────────────────────────────────────────────────────
cls("FightTrigger", "FightTrigger")
attr("FightTrigger", "canTriggered", "bool", "public")
attr("FightTrigger", "sceneLoader", "SceneLoader", "private")
attr("FightTrigger", "enemies", "List<EnemiesSettings>", "private")
op("FightTrigger", "OnTriggerEnter2D", vis="private")
op("FightTrigger", "StartFight", vis="private")
op("FightTrigger", "SaveEnemiesToFile", vis="private")
end_cls()

# ─── FightDataLoader ──────────────────────────────────────────────────────────
cls("FightDataLoader", "FightDataLoader")
attr("FightDataLoader", "enemyPrefab", "GameObject", "private")
attr("FightDataLoader", "characterPrefab", "GameObject", "private")
op("FightDataLoader", "LoadFightData", vis="private")
op("FightDataLoader", "LoadCharactersData", vis="private")
end_cls()

# ─── Enumerations ─────────────────────────────────────────────────────────────
for ename, elits in [
    ("TargetType", ["Enemy","Ally","Self","AllEnemies","AllAllies"]),
    ("AbilityType", ["Physical","Magic","Defense","Support"]),
    ("StateEffect",  ["None","Fire","Water","Air","Ground"]),
]:
    eid = uid(ename)
    lines.append(f'    <packagedElement xmi:type="uml:Enumeration" xmi:id="{eid}" name="{ename}" visibility="public">')
    for lit in elits:
        lines.append(f'      <ownedLiteral xmi:id="{uid(ename+lit)}" name="{lit}"/>')
    lines.append('    </packagedElement>')

lines.append(f'  </packagedElement>')  # close package
lines.append('')

# ─── ASSOCIATIONS (all from code analysis) ────────────────────────────────────
def assoc(src, tgt, name, aggr="none", tgt_mult="1"):
    aid = uid(f"ASSOC_{src}_{tgt}_{name}")
    assoc_name = f"{src}__{name}"
    lines.append(f'  <packagedElement xmi:type="uml:Association" xmi:id="{aid}" name="{assoc_name}">')
    e1 = uid(aid+"_e1"); e2 = uid(aid+"_e2")
    lines.append(f'    <ownedEnd xmi:id="{e1}" type="{uid(src)}" aggregation="{aggr}" isNavigable="false"/>')
    lines.append(f'    <ownedEnd xmi:id="{e2}" name="{name}" type="{uid(tgt)}" isNavigable="true" multiplicity="{tgt_mult}"/>')
    lines.append('  </packagedElement>')

# Base → EntityStats (composition: Base owns stats)
assoc("Base", "EntityStats", "stats", "composite")
# Base → ActiveStatusEffect (composition: Base owns list)
assoc("Base", "ActiveStatusEffect", "activeEffects", "composite", "0..*")
# ActiveStatusEffect → StatusEffectSO (association)
assoc("ActiveStatusEffect", "StatusEffectSO", "data")

# Character → CharacterData
assoc("Character", "CharacterData", "characterData")
# Character → CharacterAbilitySet
assoc("Character", "CharacterAbilitySet", "AbilitySet")
# Character → ActionButtons (uses)
assoc("Character", "ActionButtons", "actionButtons")

# Enemy → EnemyData
assoc("Enemy", "EnemyData", "enemyData")
# Enemy → ActionButtons (uses)
assoc("Enemy", "ActionButtons", "actionButtons")

# CharacterData → CharacterAbilitySet (composition)
assoc("CharacterData", "CharacterAbilitySet", "AbilitySet", "composite")
# CharacterDataRuntime → CharacterData
assoc("CharacterDataRuntime", "CharacterData", "originalData")
# CharacterDataRuntime → CharacterAbilitySet
assoc("CharacterDataRuntime", "CharacterAbilitySet", "_abilitySet")

# CharacterAbilitySet → CharacterAbility (composition 0..*)
assoc("CharacterAbilitySet", "CharacterAbility", "abilities", "composite", "0..*")
# CharacterAbility → BattleAbility
assoc("CharacterAbility", "BattleAbility", "ability")

# Concrete abilities → StatusEffectSO
assoc("MeleeAbility", "StatusEffectSO", "statusEffect")
assoc("MagicAbility", "StatusEffectSO", "statusEffect")
assoc("HealAbility", "StatusEffectSO", "healOverTimeEffect")
assoc("DefenseAbility", "StatusEffectSO", "defenseEffect")
assoc("StatusAbility", "StatusEffectSO", "effect")

# EnemiesSettings → EnemyData
assoc("EnemiesSettings", "EnemyData", "enemyData")

# AbilityButton → BattleAbility
assoc("AbilityButton", "BattleAbility", "ability")
# AbilityButton → Base (character reference)
assoc("AbilityButton", "Base", "character")
# AbilityButton → ActionButtons
assoc("AbilityButton", "ActionButtons", "actionButtons")

# AbilityManager → FightManager
assoc("AbilityManager", "FightManager", "fightManager")
# AbilityManager → Character (currentCharacter)
assoc("AbilityManager", "Character", "currentCharacter")

# ActionButtons → FightManager
assoc("ActionButtons", "FightManager", "fightManager")
# ActionButtons → Enemy (currentEnemy)
assoc("ActionButtons", "Enemy", "currentEnemy")
# ActionButtons → AbilityManager
assoc("ActionButtons", "AbilityManager", "abilityManager")
# ActionButtons → BattleAbility (pendingAbility)
assoc("ActionButtons", "BattleAbility", "pendingAbility")
# ActionButtons → Base (pendingAttacker)
assoc("ActionButtons", "Base", "pendingAttacker")

# FightManager → Character (aggregation 0..*)
assoc("FightManager", "Character", "characters", "shared", "0..*")
# FightManager → Enemy (aggregation 0..*)
assoc("FightManager", "Enemy", "enemies", "shared", "0..*")
# FightManager → Base (aggregation 0..*)
assoc("FightManager", "Base", "bases", "shared", "0..*")
# FightManager → AbilityManager
assoc("FightManager", "AbilityManager", "abilityManager")
# FightManager → BattleTeamSync
assoc("FightManager", "BattleTeamSync", "battleTeamSync")

# BattleTeamSync → FightManager
assoc("BattleTeamSync", "FightManager", "fightManager")
# BattleTeamSync → Character (dictionary values)
assoc("BattleTeamSync", "Character", "initialCharacters", "none", "0..*")

# FightTrigger → EnemiesSettings (composition 0..*)
assoc("FightTrigger", "EnemiesSettings", "enemies", "composite", "0..*")

# FightDataLoader → Enemy (creates)
assoc("FightDataLoader", "Enemy", "creates")
# FightDataLoader → Character (creates)
assoc("FightDataLoader", "Character", "creates")

lines.append('</uml:Model>')

content = "\n".join(lines)
with open("EchoRift_FightSystem.xml", "w", encoding="utf-8") as f:
    f.write(content)

import xml.etree.ElementTree as ET
try:
    ET.parse("EchoRift_FightSystem.xml")
    print(f"XML valid! {len(lines)} lines, {len(content)} chars")
    print("Classes:", sum(1 for l in lines if 'uml:Class' in l or 'uml:Interface' in l))
    print("Associations:", sum(1 for l in lines if 'uml:Association' in l))
    print("Generalizations:", sum(1 for l in lines if '<generalization' in l))
except ET.ParseError as e:
    print("ERROR:", e)
    src = content.split('\n')
    ln = e.position[0]
    print("Line", ln, ":", repr(src[ln-1]))
