#!/usr/bin/env python3
"""Generate UML2 XMI Component Diagram for FightSystem module (EA-compatible)"""
import uuid

def uid(n):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.fight.comp." + n)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="FightSystem_Component">')
lines.append('')

deferred = []

def pkg(key, name):
    lines.append(f'  <packagedElement xmi:type="uml:Package" xmi:id="{uid(key)}" name="{name}">')

def end_pkg():
    lines.append('  </packagedElement>')

def component(key, name, indent="    "):
    lines.append(f'{indent}<packagedElement xmi:type="uml:Component" xmi:id="{uid(key)}" name="{name}" visibility="public">')

def end_component(indent="    "):
    lines.append(f'{indent}</packagedElement>')

def provided(comp_key, iface_key, indent="      "):
    rid = uid(f"PROV_{comp_key}_{iface_key}")
    lines.append(f'{indent}<interfaceRealization xmi:id="{rid}" contract="{uid(iface_key)}" implementingClassifier="{uid(comp_key)}"/>')

def interface(key, name, ops=None, indent="    "):
    lines.append(f'{indent}<packagedElement xmi:type="uml:Interface" xmi:id="{uid(key)}" name="{name}" visibility="public">')
    if ops:
        for op in ops:
            lines.append(f'{indent}  <ownedOperation xmi:id="{uid(key+"_op_"+op)}" name="{op}" visibility="public" isAbstract="true"/>')
    lines.append(f'{indent}</packagedElement>')

def usage(src_key, tgt_key):
    deferred.append(
        f'    <packagedElement xmi:type="uml:Usage" xmi:id="{uid("USE_"+src_key+"_"+tgt_key)}"'
        f' client="{uid(src_key)}" supplier="{uid(tgt_key)}"/>'
    )

def dependency(src_key, tgt_key, label="uses"):
    deferred.append(
        f'    <packagedElement xmi:type="uml:Dependency" xmi:id="{uid("DEP_"+src_key+"_"+tgt_key)}"'
        f' name="{label}" client="{uid(src_key)}" supplier="{uid(tgt_key)}"/>'
    )

# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_FIGHT_COMP", "FightSystem_Component")

# ── Interfaces ────────────────────────────────────────────────────────────────
interface("IData", "IData",
    ["Name","Damage","MaxHealth","Health","MaxMana","Mana",
     "Heal","Armor","Level","CurrentXP","XpReward","AttackAnimationName"])

interface("IBattleUnit", "IBattleUnit",
    ["TakeDamage","TakeMagicDamage","TakeHeal","GiveDamage",
     "ProcessStatusEffects","GetXP","UpdateUI"])

interface("IBattleAbility", "IBattleAbility",
    ["Execute","CanUse"])

# ── Component: EntityStats ────────────────────────────────────────────────────
component("COMP_EntityStats", "EntityStats")
provided("COMP_EntityStats", "IData")
end_component()

# ── Component: Base ───────────────────────────────────────────────────────────
component("COMP_Base", "Base")
provided("COMP_Base", "IData")
provided("COMP_Base", "IBattleUnit")
end_component()
dependency("COMP_Base", "COMP_EntityStats", "contains")

# ── Component: Character ─────────────────────────────────────────────────────
component("COMP_Character", "Character")
provided("COMP_Character", "IBattleUnit")
end_component()
dependency("COMP_Character", "COMP_Base",            "extends")
dependency("COMP_Character", "COMP_CharacterData",   "initialized from")
dependency("COMP_Character", "COMP_AbilityManager",  "notifies")

# ── Component: Enemy ─────────────────────────────────────────────────────────
component("COMP_Enemy", "Enemy")
provided("COMP_Enemy", "IBattleUnit")
end_component()
dependency("COMP_Enemy", "COMP_Base",       "extends")
dependency("COMP_Enemy", "COMP_EnemyData",  "initialized from")

# ── Component: CharacterData ──────────────────────────────────────────────────
component("COMP_CharacterData", "CharacterData")
provided("COMP_CharacterData", "IData")
end_component()
dependency("COMP_CharacterData", "COMP_CharacterAbilitySet", "owns")

# ── Component: EnemyData ──────────────────────────────────────────────────────
component("COMP_EnemyData", "EnemyData")
provided("COMP_EnemyData", "IData")
end_component()

# ── Component: CharacterAbilitySet ───────────────────────────────────────────
component("COMP_CharacterAbilitySet", "CharacterAbilitySet")
end_component()
dependency("COMP_CharacterAbilitySet", "COMP_BattleAbility", "contains")

# ── Component: BattleAbility ─────────────────────────────────────────────────
component("COMP_BattleAbility", "BattleAbility")
provided("COMP_BattleAbility", "IBattleAbility")
end_component()

# ── Component: MeleeAbility ───────────────────────────────────────────────────
component("COMP_MeleeAbility", "MeleeAbility")
provided("COMP_MeleeAbility", "IBattleAbility")
end_component()
dependency("COMP_MeleeAbility", "COMP_BattleAbility", "extends")

# ── Component: MagicAbility ───────────────────────────────────────────────────
component("COMP_MagicAbility", "MagicAbility")
provided("COMP_MagicAbility", "IBattleAbility")
end_component()
dependency("COMP_MagicAbility", "COMP_BattleAbility", "extends")

# ── Component: HealAbility ────────────────────────────────────────────────────
component("COMP_HealAbility", "HealAbility")
provided("COMP_HealAbility", "IBattleAbility")
end_component()
dependency("COMP_HealAbility", "COMP_BattleAbility", "extends")

# ── Component: DefenseAbility ─────────────────────────────────────────────────
component("COMP_DefenseAbility", "DefenseAbility")
provided("COMP_DefenseAbility", "IBattleAbility")
end_component()
dependency("COMP_DefenseAbility", "COMP_BattleAbility", "extends")

# ── Component: StatusAbility ──────────────────────────────────────────────────
component("COMP_StatusAbility", "StatusAbility")
provided("COMP_StatusAbility", "IBattleAbility")
end_component()
dependency("COMP_StatusAbility", "COMP_BattleAbility", "extends")

# ── Component: FightManager ───────────────────────────────────────────────────
component("COMP_FightManager", "FightManager")
end_component()
usage("COMP_FightManager", "IBattleUnit")
dependency("COMP_FightManager", "COMP_Character",      "manages")
dependency("COMP_FightManager", "COMP_Enemy",          "manages")
dependency("COMP_FightManager", "COMP_AbilityManager", "uses")
dependency("COMP_FightManager", "COMP_BattleTeamSync", "uses")
dependency("COMP_FightManager", "COMP_GameSettings",   "reads")

# ── Component: AbilityManager ─────────────────────────────────────────────────
component("COMP_AbilityManager", "AbilityManager")
end_component()
usage("COMP_AbilityManager", "IBattleAbility")
dependency("COMP_AbilityManager", "COMP_CharacterAbilitySet", "reads")
dependency("COMP_AbilityManager", "COMP_ActionButtons",       "notifies")

# ── Component: ActionButtons ──────────────────────────────────────────────────
component("COMP_ActionButtons", "ActionButtons")
end_component()
usage("COMP_ActionButtons", "IBattleAbility")
usage("COMP_ActionButtons", "IBattleUnit")
dependency("COMP_ActionButtons", "COMP_FightManager",  "uses")
dependency("COMP_ActionButtons", "COMP_BattleTeamSync","calls on escape")

# ── Component: BattleTeamSync ─────────────────────────────────────────────────
component("COMP_BattleTeamSync", "BattleTeamSync")
end_component()
dependency("COMP_BattleTeamSync", "COMP_FightManager",  "reads characters")
dependency("COMP_BattleTeamSync", "COMP_SaveLoadSystem","saves team data")
dependency("COMP_BattleTeamSync", "COMP_GlobalLoader",  "reads player team")

# ── Component: FightTrigger ───────────────────────────────────────────────────
component("COMP_FightTrigger", "FightTrigger")
end_component()
dependency("COMP_FightTrigger", "COMP_SaveLoadSystem", "saves fight data")
dependency("COMP_FightTrigger", "COMP_GlobalLoader",   "gets sceneLoader")
dependency("COMP_FightTrigger", "COMP_SceneLoader",    "loads fight scene")

# ── Component: FightDataLoader ────────────────────────────────────────────────
component("COMP_FightDataLoader", "FightDataLoader")
end_component()
dependency("COMP_FightDataLoader", "COMP_SaveLoadSystem", "loads fight data")
dependency("COMP_FightDataLoader", "COMP_Character",      "initializes")
dependency("COMP_FightDataLoader", "COMP_Enemy",          "initializes")

# ── Component: StatusEffectSO ─────────────────────────────────────────────────
component("COMP_StatusEffectSO", "StatusEffectSO")
end_component()

# ── External components (referenced) ─────────────────────────────────────────
component("COMP_SaveLoadSystem", "SaveLoadSystem")
end_component()

component("COMP_GlobalLoader", "GlobalLoader")
end_component()

component("COMP_SceneLoader", "SceneLoader")
end_component()

component("COMP_GameSettings", "GameSettings")
end_component()

# ── write deferred ────────────────────────────────────────────────────────────
for rel in deferred:
    lines.append(rel)

end_pkg()
lines.append('</uml:Model>')

content = "\n".join(lines)
out = "EchoRift_FightSystem_Component.xml"
with open(out, "w", encoding="utf-8") as f:
    f.write(content)

import xml.etree.ElementTree as ET
try:
    ET.parse(out)
    comps  = sum(1 for l in lines if 'uml:Component"' in l)
    ifaces = sum(1 for l in lines if 'uml:Interface"' in l)
    deps   = sum(1 for l in lines if 'uml:Dependency"' in l or 'uml:Usage"' in l)
    print(f"XML valid!  {len(lines)} lines -> {out}")
    print(f"  Components : {comps}")
    print(f"  Interfaces : {ifaces}")
    print(f"  Deps/Usage : {deps}")
except ET.ParseError as e:
    src = content.split('\n')
    ln = e.position[0]
    print("ERROR:", e)
    print("Line", ln, ":", repr(src[ln-1]))
