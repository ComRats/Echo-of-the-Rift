#!/usr/bin/env python3
"""Generate EMX/UML2 compatible XMI for Enterprise Architect"""

import uuid

def uid(name):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, name)).upper().replace("-","_")

classes = [
    # (id_key, name, package, stereotype, parent, interfaces, attrs, ops)
    # FightSystem
    ("IF_IData", "IData", "FightSystem", "interface", None, [],
     [("Name","String"),("Description","String"),("Damage","int"),("MaxHealth","int"),
      ("Health","int"),("MaxMana","int"),("Mana","int"),("Heal","int"),("Armor","int"),
      ("Level","int"),("CurrentXP","int"),("MaxXP","int"),("XpReward","int")],
     []),
    ("CLS_EntityStats", "EntityStats", "FightSystem", "class", None, ["IF_IData"],
     [("_name","String","protected"),("_damage","int","protected"),("_maxHealth","int","protected"),
      ("_health","int","protected"),("_level","int","protected")],
     [("RecalculateStats","void","public"),("CaptureBaseStats","void","public")]),
    ("CLS_Base", "Base", "FightSystem", "class", None, ["IF_IData"],
     [("stats","EntityStats","private"),("activeEffects","List<ActiveStatusEffect>","public"),
      ("IsBlinking","bool","public")],
     [("Initialize","void","public"),("TakeDamage","void","public"),("TakeMagicDamage","void","public"),
      ("TakeHeal","void","public"),("GiveDamage","int","public"),("PlayAnimation","void","public"),
      ("ApplyStatusEffect","void","public"),("ProcessStatusEffects","void","public"),
      ("GetXP","void","public"),("UpdateUI","void","public")]),
    ("CLS_Character", "Character", "FightSystem", "class", "CLS_Base", [],
     [("IsTurn","bool","public"),("characterData","CharacterData","private"),
      ("AbilitySet","CharacterAbilitySet","public")],
     [("InitializeFromSettings","void","public")]),
    ("CLS_Enemy", "Enemy", "FightSystem", "class", "CLS_Base", [],
     [("enemyData","EnemyData","private")],
     [("InitializeFromSettings","void","public"),("LocalInizialize","void","public")]),
    ("CLS_FightManager", "FightManager", "FightSystem", "class", None, [],
     [("ActiveCharacter","Character","public"),("enemies","List<Enemy>","public"),
      ("characters","List<Character>","public"),("abilityManager","AbilityManager","private"),
      ("battleTeamSync","BattleTeamSync","private"),("gameSettings","GameSettings","private")],
     [("StartFight","void","private"),("EndFight","void","private"),
      ("WaitCharacterTurn","void","private"),("GetCharacterLowestHP","Character","private"),
      ("DeleteEnemyOnList","void","public")]),
    ("CLS_BattleAbility", "BattleAbility", "FightSystem", "class", None, [],
     [("AbilityName","String","public"),("ManaCost","int","public"),
      ("targetType","TargetType","public"),("animTrigger","String","public"),
      ("Description","String","public")],
     [("Execute","void","public"),("CanUse","bool","public"),
      ("PlayHitAnimation","void","protected"),("SpawnVFX","void","protected")]),
    ("CLS_CharacterAbilitySet", "CharacterAbilitySet", "FightSystem", "class", None, [],
     [("abilities","List<CharacterAbility>","private")],
     [("GetUnlockedAbilities","List","public"),("GetActiveAbilities","List","public"),
      ("ActivateAbility","void","public")]),
    ("CLS_CharacterAbility", "CharacterAbility", "FightSystem", "class", None, [],
     [("ability","BattleAbility","public"),("requiredLevel","int","public"),
      ("abilityType","AbilityType","public"),("isActiveByDefault","bool","public")],
     [("IsUnlocked","bool","public")]),
    ("CLS_AbilityManager", "AbilityManager", "FightSystem", "class", None, [],
     [],
     [("SetupAbilitiesForCharacter","void","public"),("CreateAbilityButton","void","public")]),
    ("CLS_BattleTeamSync", "BattleTeamSync", "FightSystem", "class", None, [],
     [],
     [("OnTeamManagerReady","void","public"),("LinkTeamManagerWithBattle","void","public")]),
    ("CLS_ActionButtons", "ActionButtons", "FightSystem", "class", None, [],
     [("currentEnemy","Enemy","public")],
     [("OnEnemySelected","void","public"),("OnCharacterSelected","void","public"),
      ("SetPendingAbility","void","public"),("ShowAbilityDescription","void","public")]),
    ("CLS_CharacterData", "CharacterData", "FightSystem", "class", None, [],
     [("AbilitySet","CharacterAbilitySet","public")], []),
    ("CLS_EnemyData", "EnemyData", "FightSystem", "class", None, [], [], []),
    # Player
    ("CLS_Player", "Player", "Player", "class", None, [],
     [("movement","Movement","public"),("cameraSettings","CameraSettings","public"),
      ("thinking","PlayerThinking","public"),("team","Team","public"),
      ("playerSaver","PlayerSaver","public"),("startPosition","Vector3","public"),
      ("Result","FightResult","public")],
     [("SetListenerToEvents","void","public"),("Hide","void","public"),("Show","void","public")]),
    ("CLS_Team", "Team", "Player", "class", None, [],
     [("characters","List<CharactersSettings>","public")],
     [("AddCharacter","void","public"),("CreateSaveData","TeamSaveData","public"),
      ("LoadFromSaveData","void","public")]),
    ("CLS_CharactersSettings", "CharactersSettings", "Player", "class", None, ["IF_IData"],
     [("useCharacterData","bool","public"),("characterData","CharacterData","public"),
      ("characterDataName","String","public"),("RuntimeData","CharacterDataRuntime","public")],
     [("GetSprite","Sprite","public"),("GetCharacterData","CharacterData","public"),
      ("ResetRuntimeData","void","public")]),
    ("CLS_PlayerSaver", "PlayerSaver", "Player", "class", "CLS_EntityStats", [],
     [("spritePath","String","public")],
     [("LoadFrom","void","public"),("GetSprite","Sprite","public")]),
    ("CLS_Movement", "Movement", "Player", "class", None, [],
     [], [("Moving","void","private")]),
    ("CLS_CameraSettings", "CameraSettings", "Player", "class", None, [],
     [], [("Initialize","void","public")]),
    ("CLS_PlayerThinking", "PlayerThinking", "Player", "class", None, [],
     [], [("SetThink","void","public"),("EraseText","void","public")]),
    ("CLS_PlayerEvents", "PlayerEvents", "Player", "class", None, [],
     [], [("CloseUI","void","public"),("ShowCursor","void","public"),("HideCursor","void","public")]),
    ("CLS_TeamSaveData", "TeamSaveData", "Player", "class", None, [],
     [("charactersData","List<CharacterSaveData>","public")], []),
    ("CLS_CharacterSaveData", "CharacterSaveData", "Player", "class", None, [],
     [("characterDataName","String","public"),("Health","int","public"),
      ("MaxHealth","int","public"),("Mana","int","public"),("Level","int","public")], []),
    # NPC_Dialogue
    ("IF_ITalkable", "ITalkable", "NPC_Dialogue", "interface", None, [],
     [], [("Talk","void","public")]),
    ("CLS_NPC", "NPC", "NPC_Dialogue", "class", None, ["IF_ITalkable"],
     [("isTalkable","bool","private"),("DSTrigger","DialogueSystemTrigger","private")],
     [("Talk","void","public")]),
    ("CLS_LuaFunctions", "LuaFunctions", "NPC_Dialogue", "class", None, [],
     [("inventoryManager","InventoryManager","private")],
     [("HasItem","bool","public"),("AddItem","void","public"),("RemoveItem","void","public"),
      ("HasCoins","bool","public"),("AddCoins","void","public"),("GetStat","double","public")]),
    ("CLS_DialogueSaveMgr", "DialogueSaveManager", "NPC_Dialogue", "class", None, [],
     [], [("Save","void","public"),("Load","void","public"),
          ("Delete","void","public"),("Exists","bool","public")]),
    ("CLS_SequencerCmd", "SequencerCommand", "NPC_Dialogue", "class", None, [],
     [], [("OnStart","void","public")]),
    ("CLS_SC_GiveItem", "SequencerCommandGiveItem", "NPC_Dialogue", "class", "CLS_SequencerCmd", [], [], []),
    ("CLS_SC_PlaySound", "SequencerCommandPlaySound", "NPC_Dialogue", "class", "CLS_SequencerCmd", [], [], []),
    ("CLS_SC_RecruitComp", "SequencerCommandRecruitCompanion", "NPC_Dialogue", "class", "CLS_SequencerCmd", [], [], []),
    ("CLS_SC_Shop", "SequencerCommandShop", "NPC_Dialogue", "class", "CLS_SequencerCmd", [], [], []),
    ("CLS_InventoryManager", "InventoryManager", "NPC_Dialogue", "class", None, [],
     [("Wallet","PlayerWallet","public")],
     [("HasItem","bool","public"),("AddItem","void","public"),("SyncFromUI","void","public")]),
    ("CLS_PlayerWallet", "PlayerWallet", "NPC_Dialogue", "class", None, [],
     [("coins","int","public")],
     [("AddCoins","void","public"),("RemoveCoins","void","public")]),
    # SceneManagement
    ("CLS_GlobalLoader", "GlobalLoader", "SceneManagement", "class", None, [],
     [("Instance","GlobalLoader","public"),("playerInstance","Player","public"),
      ("mainUI","MainUI","public"),("gameSettings","GameSettings","private"),
      ("fightSceneLoader","SceneLoader","public")],
     [("SavePlayer","void","public"),("LoadPlayerData","void","public"),
      ("SaveGlobal","void","public"),("LoadGlobal","void","public"),
      ("LoadToScene","void","public"),("Show","void","public"),("Hide","void","public")]),
    ("CLS_SceneLoader", "SceneLoader", "SceneManagement", "class", None, [],
     [("GlobalLoadingSpeed","float","public")],
     [("LoadScene","void","public"),("LoadSceneAsync","void","public")]),
    ("CLS_ScreenFader", "ScreenFader", "SceneManagement", "class", None, [],
     [("fadeDuration","float","private")],
     [("FadeInAsync","Task","public"),("FadeOutAsync","Task","public"),("SetAlpha","void","public")]),
    ("CLS_SceneLoaderTrigger", "SceneLoaderTrigger", "SceneManagement", "class", None, [],
     [], [("OnTriggerEnter2D","void","private")]),
    ("CLS_TimeManager", "TimeManager", "SceneManagement", "class", None, [],
     [], [("Pause","void","public"),("Resume","void","public")]),
    ("CLS_GameSettings", "GameSettings", "SceneManagement", "class", None, [],
     [("loadingSceneSpeed","float","public"),("enemyTurnDelay","float","public"),
      ("enemyTurnSpeed","float","public")], []),
    ("CLS_MainUI", "MainUI", "SceneManagement", "class", None, [],
     [("inventoryManager","InventoryManager","public"),("canOpenUI","bool","public")],
     [("Show","void","public"),("Hide","void","public")]),
    # SaveLoadSystem
    ("CLS_SaveLoadSystem", "SaveLoadSystem", "SaveLoadSystem", "class", None, [],
     [], [("GetPath","String","public"),("Save","void","public"),("Load","T","public"),
          ("Exists","bool","public"),("Delete","void","public"),("ClearAllSaves","void","public")]),
    ("CLS_PersistentObject", "PersistentObject", "SaveLoadSystem", "class", None, [],
     [("persistentId","String","private")],
     [("Save","void","public"),("Load","void","public"),
      ("SaveAll","void","public"),("LoadAll","void","public")]),
    ("CLS_SceneObjectSaver", "SceneObjectSaver", "SaveLoadSystem", "class", None, [],
     [("dialogueVariable","String","private"),("hideWhenTrue","bool","private"),
      ("disableGameObject","bool","private")],
     [("ApplyState","void","public"),("SetVariableAndApply","void","public")]),
    ("CLS_SaveFileNames", "SaveFileNames", "SaveLoadSystem", "class", None, [],
     [("PLAYER_DATA","String","public"),("TEAM_DATA","String","public"),
      ("DIALOGUE_STATE","String","public"),("GAME_DIRECTORY","String","public")], []),
    # Audio
    ("IF_IAudioManager", "IAudioManager", "Audio_Notification", "interface", None, [],
     [], [("Play","void","public"),("AddSoundFromPath","void","public"),("TryGetSource","bool","public")]),
    ("IF_IAudioLogger", "IAudioLogger", "Audio_Notification", "interface", None, [],
     [], [("Log","void","public"),("LogFormat","void","public"),("LogException","void","public")]),
    ("CLS_UIAudioLogger", "UIAudioLogger", "Audio_Notification", "class", None, ["IF_IAudioLogger"],
     [("m_logOutput","Text","private"),("m_logLevel","LoggingLevel","private")],
     [("Log","void","public"),("LogFormat","void","public"),("CanLog","bool","private")]),
    ("CLS_UIAudioAutoInstaller", "UIAudioAutoInstaller", "Audio_Notification", "class", None, [],
     [("clickSoundName","String","private")],
     [("InitializeWithDelay","void","private"),("PlaySound","void","private")]),
    ("CLS_AreaAmbientSound", "AreaAmbientSound", "Audio_Notification", "class", None, [],
     [("soundName","String","private")],
     [("OnTriggerEnter2D","void","private"),("OnTriggerExit2D","void","private")]),
    # Debug
    ("CLS_DebugCommands", "DebugCommands", "Debug_Editor", "class", None, [],
     [], [("SpawnConsole","void","private"),("RegisterCommands","void","private"),
          ("LoadScene","void","private"),("GiveItem","void","private"),
          ("SaveGame","void","private"),("LoadGame","void","private")]),
    ("CLS_GameSettingsEditor", "GameSettingsEditor", "Debug_Editor", "class", None, [],
     [], [("OnInspectorGUI","void","public")]),
    ("IF_IColliderDebug", "IColliderDebugDrawable2D", "Debug_Editor", "interface", None, [],
     [], [("GetCollider2D","Collider2D","public"),("ShouldDrawGizmos","bool","public"),
          ("OnDrawColliderGizmos2D","void","public")]),
]

enums = [
    ("ENUM_AbilityType", "AbilityType", "FightSystem", ["Physical","Magic","Defense","Support"]),
    ("ENUM_TargetType", "TargetType", "FightSystem", ["Enemy","Ally","Self","All"]),
    ("ENUM_StateEffect", "StateEffect", "FightSystem", ["None","Fire","Water","Air","Ground"]),
    ("ENUM_FightResult", "FightResult", "Player", ["None","Win","Lose"]),
    ("ENUM_LoggingLevel", "LoggingLevel", "Audio_Notification", ["NONE","ERROR","WARNING","INFO"]),
]

packages = ["FightSystem","Player","NPC_Dialogue","SceneManagement","SaveLoadSystem","Audio_Notification","Debug_Editor"]

# Associations (source_id, target_id, name, src_mult, tgt_mult, aggregation)
associations = [
    ("CLS_Base", "CLS_EntityStats", "stats", "1", "1", "composite"),
    ("CLS_FightManager", "CLS_Character", "characters", "1", "0..*", "shared"),
    ("CLS_FightManager", "CLS_Enemy", "enemies", "1", "0..*", "shared"),
    ("CLS_CharacterAbilitySet", "CLS_CharacterAbility", "abilities", "1", "0..*", "composite"),
    ("CLS_CharacterAbility", "CLS_BattleAbility", "ability", "1", "1", "none"),
    ("CLS_CharacterData", "CLS_CharacterAbilitySet", "AbilitySet", "1", "1", "composite"),
    ("CLS_Player", "CLS_Team", "team", "1", "1", "composite"),
    ("CLS_Team", "CLS_CharactersSettings", "characters", "1", "0..*", "composite"),
    ("CLS_TeamSaveData", "CLS_CharacterSaveData", "charactersData", "1", "0..*", "composite"),
    ("CLS_GlobalLoader", "CLS_Player", "playerInstance", "1", "1", "composite"),
    ("CLS_GlobalLoader", "CLS_MainUI", "mainUI", "1", "1", "composite"),
    ("CLS_MainUI", "CLS_InventoryManager", "inventoryManager", "1", "1", "composite"),
    ("CLS_InventoryManager", "CLS_PlayerWallet", "Wallet", "1", "1", "composite"),
    ("CLS_LuaFunctions", "CLS_InventoryManager", "uses", "1", "1", "none"),
    ("CLS_GlobalLoader", "CLS_SaveLoadSystem", "uses", "1", "1", "none"),
    ("CLS_PersistentObject", "CLS_SaveLoadSystem", "uses", "1", "1", "none"),
    ("CLS_DialogueSaveMgr", "CLS_SaveLoadSystem", "uses", "1", "1", "none"),
    ("CLS_DebugCommands", "CLS_GlobalLoader", "uses", "1", "1", "none"),
    ("CLS_UIAudioAutoInstaller", "IF_IAudioManager", "uses", "1", "1", "none"),
    ("CLS_GameSettingsEditor", "CLS_GameSettings", "edits", "1", "1", "none"),
]

vis_map = {"public":"+","private":"-","protected":"#"}

def write_emx():
    lines = []
    lines.append('<?xml version="1.0" encoding="UTF-8"?>')
    lines.append('<uml:Model xmi:version="2.1" xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
    lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
    lines.append('  xmlns:ecore="http://www.eclipse.org/emf/2002/Ecore"')
    lines.append('  xmi:id="' + uid("ROOT") + '" name="EchoRift">')
    lines.append('')

    # Group by package
    pkg_classes = {p: [] for p in packages}
    for c in classes:
        pkg_classes[c[2]].append(c)
    pkg_enums = {p: [] for p in packages}
    for e in enums:
        pkg_enums[e[2]].append(e)

    for pkg in packages:
        pkg_id = uid("PKG_" + pkg)
        lines.append(f'  <packagedElement xmi:type="uml:Package" xmi:id="{pkg_id}" name="{pkg}">')

        for c in pkg_classes[pkg]:
            cid, cname, cpkg, ctype, cparent, cifaces, cattrs, cops = c
            eid = uid(cid)
            is_abstract = "true" if cname in ["BattleAbility","SequencerCommand"] else "false"
            if ctype == "interface":
                lines.append(f'    <packagedElement xmi:type="uml:Interface" xmi:id="{eid}" name="{cname}" visibility="public">')
            else:
                lines.append(f'    <packagedElement xmi:type="uml:Class" xmi:id="{eid}" name="{cname}" visibility="public" isAbstract="{is_abstract}">')

            # Generalization
            if cparent:
                gid = uid("GEN_" + cid)
                pid = uid(cparent)
                lines.append(f'      <generalization xmi:id="{gid}" general="{pid}"/>')

            # Interface realizations
            for iface in cifaces:
                rid = uid("REAL_" + cid + "_" + iface)
                iid = uid(iface)
                lines.append(f'      <interfaceRealization xmi:id="{rid}" supplier="{iid}" client="{eid}"/>')

            # Attributes
            for attr in cattrs:
                if len(attr) == 2:
                    aname, atype = attr
                    avis = "public"
                else:
                    aname, atype, avis = attr
                aid = uid(cid + "_attr_" + aname)
                # Use simple type comment to avoid XML special chars in href
                safe_type = atype.replace("<","[").replace(">","]")
                lines.append(f'      <ownedAttribute xmi:id="{aid}" name="{aname}" visibility="{avis}" type="{safe_type}"/>')

            # Operations
            for op in cops:
                if len(op) == 2:
                    oname, otype = op
                    ovis = "public"
                else:
                    oname, otype, ovis = op
                oid = uid(cid + "_op_" + oname)
                lines.append(f'      <ownedOperation xmi:id="{oid}" name="{oname}" visibility="{ovis}"/>')

            lines.append(f'    </packagedElement>')

        # Enumerations
        for e in pkg_enums[pkg]:
            eid_key, ename, epkg, eliterals = e
            eid = uid(eid_key)
            lines.append(f'    <packagedElement xmi:type="uml:Enumeration" xmi:id="{eid}" name="{ename}" visibility="public">')
            for lit in eliterals:
                lid = uid(eid_key + "_" + lit)
                lines.append(f'      <ownedLiteral xmi:id="{lid}" name="{lit}"/>')
            lines.append(f'    </packagedElement>')

        lines.append(f'  </packagedElement>')
        lines.append('')

    # Associations — grouped by source package
    # Build a map: class_id -> package
    cls_pkg = {c[0]: c[2] for c in classes}

    pkg_assocs = {p: [] for p in packages}
    for assoc in associations:
        src = assoc[0]
        pkg = cls_pkg.get(src, packages[0])
        pkg_assocs[pkg].append(assoc)

    # We already closed packages above, so reopen them as separate packagedElements
    # Actually EA handles top-level associations fine — just rename them clearly
    for assoc in associations:
        src, tgt, aname, smult, tmult, aggr = assoc
        aid = uid("ASSOC_" + src + "_" + tgt + "_" + aname)
        src_id = uid(src)
        tgt_id = uid(tgt)
        src_name = next((c[1] for c in classes if c[0]==src), src)
        tgt_name = next((c[1] for c in classes if c[0]==tgt), tgt)
        full_name = f"{src_name}__{tgt_name}"
        lines.append(f'  <packagedElement xmi:type="uml:Association" xmi:id="{aid}" name="{full_name}">')
        e1id = uid(aid + "_end1")
        e2id = uid(aid + "_end2")
        agg_val = "composite" if aggr == "composite" else ("shared" if aggr == "shared" else "none")
        lines.append(f'    <ownedEnd xmi:id="{e1id}" type="{src_id}" aggregation="{agg_val}" isNavigable="false"/>')
        lines.append(f'    <ownedEnd xmi:id="{e2id}" name="{aname}" type="{tgt_id}" isNavigable="true"/>')
        lines.append(f'  </packagedElement>')

    lines.append('</uml:Model>')
    return "\n".join(lines)

content = write_emx()
with open("EchoRift_ClassDiagram.xml", "w", encoding="utf-8") as f:
    f.write(content)

import xml.etree.ElementTree as ET
try:
    ET.parse("EchoRift_ClassDiagram.xml")
    print("XML valid! Size:", len(content), "chars")
except ET.ParseError as e:
    print("ERROR:", e)
