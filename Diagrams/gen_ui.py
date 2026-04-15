#!/usr/bin/env python3
"""Generate EMX/UML2 XMI for UI module"""
import uuid

def uid(n):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.ui." + n)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="UISystem">')
lines.append('')

deferred = []

def pkg(key, name):
    lines.append(f'  <packagedElement xmi:type="uml:Package" xmi:id="{uid(key)}" name="{name}">')

def end_pkg():
    lines.append('  </packagedElement>')

def cls(key, name, abstract=False, iface=False):
    t = "uml:Interface" if iface else "uml:Class"
    ab = ' isAbstract="true"' if abstract else ''
    lines.append(f'    <packagedElement xmi:type="{t}" xmi:id="{uid(key)}" name="{name}" visibility="public"{ab}>')

def end_cls():
    lines.append('    </packagedElement>')

def attr(owner, name, typ, vis="private"):
    safe = typ.replace("<","[").replace(">","]")
    lines.append(f'      <ownedAttribute xmi:id="{uid(owner+"_a_"+name)}" name="{name}" visibility="{vis}" type="{safe}"/>')

def op(owner, name, ret="void", vis="public", abstract=False):
    ab = ' isAbstract="true"' if abstract else ''
    lines.append(f'      <ownedOperation xmi:id="{uid(owner+"_o_"+name)}" name="{name}" visibility="{vis}"{ab}/>')

def generalization(child, parent):
    deferred.append(
        f'    <packagedElement xmi:type="uml:Generalization" xmi:id="{uid("GEN_"+child+"_"+parent)}"'
        f' specific="{uid(child)}" general="{uid(parent)}"/>'
    )

def realization(cls_key, iface_key):
    deferred.append(
        f'    <packagedElement xmi:type="uml:InterfaceRealization" xmi:id="{uid("REAL_"+cls_key+"_"+iface_key)}"'
        f' implementingClassifier="{uid(cls_key)}" contract="{uid(iface_key)}"/>'
    )

def assoc(src, tgt, role, aggr="none", mult="1"):
    aid = uid(f"ASSOC_{src}_{tgt}_{role}")
    e1 = uid(aid+"_e1"); e2 = uid(aid+"_e2")
    deferred.append(f'    <packagedElement xmi:type="uml:Association" xmi:id="{aid}" name="{src}__{role}">')
    deferred.append(f'      <ownedEnd xmi:id="{e1}" type="{uid(src)}" aggregation="{aggr}" isNavigable="false"/>')
    deferred.append(f'      <ownedEnd xmi:id="{e2}" name="{role}" type="{uid(tgt)}" isNavigable="true" multiplicity="{mult}"/>')
    deferred.append('    </packagedElement>')

def dep(src, tgt, label="uses"):
    deferred.append(
        f'    <packagedElement xmi:type="uml:Dependency" xmi:id="{uid("DEP_"+src+"_"+tgt+"_"+label)}"'
        f' name="{label}" client="{uid(src)}" supplier="{uid(tgt)}"/>'
    )

def enum(key, name, lits):
    lines.append(f'    <packagedElement xmi:type="uml:Enumeration" xmi:id="{uid(key)}" name="{name}" visibility="public">')
    for lit in lits:
        lines.append(f'      <ownedLiteral xmi:id="{uid(key+lit)}" name="{lit}"/>')
    lines.append('    </packagedElement>')

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Core UI
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_CoreUI", "CoreUI")

# MainUI — центральный хаб всего UI
cls("MainUI", "MainUI")
attr("MainUI", "pauseMenu", "PauseMenu", "public")
attr("MainUI", "playerUI", "PlayerUI", "public")
attr("MainUI", "canvas", "Canvas", "public")
attr("MainUI", "screenFader", "ScreenFader", "public")
attr("MainUI", "spriteCollection", "SpriteCollection", "public")
attr("MainUI", "inventoryManager", "InventoryManager", "public")
attr("MainUI", "fishingUI", "FishingUI", "public")
attr("MainUI", "shopUI", "ShopUI", "public")
attr("MainUI", "tonguesCanvas", "Canvas", "public")
attr("MainUI", "questLogWindow", "StandardUIQuestLogWindow", "public")
attr("MainUI", "teamManager", "TeamManager", "public")
attr("MainUI", "canOpenUI", "bool", "public")
attr("MainUI", "isOpenUI", "bool", "public")
attr("MainUI", "isCursorVisible", "bool", "public")
attr("MainUI", "gameSettings", "GameSettings", "private")
attr("MainUI", "contextMenu", "InventoryContextMenu", "private")
attr("MainUI", "musicManager", "MusicTransitionManager", "private")
op("MainUI", "ToggleInventory")
op("MainUI", "OpenInventory")
op("MainUI", "CloseInventory")
op("MainUI", "ToggleQuestLog")
op("MainUI", "OpenQuestLog")
op("MainUI", "CloseQuestLog")
op("MainUI", "ShowCursor")
op("MainUI", "HideCursor")
op("MainUI", "ToggleCursorVisible")
op("MainUI", "Show")
op("MainUI", "Hide")
op("MainUI", "ResetUIState")
end_cls()
assoc("MainUI", "PauseMenu", "pauseMenu", "composite")
assoc("MainUI", "PlayerUI", "playerUI", "composite")
assoc("MainUI", "InventoryManager", "inventoryManager")
assoc("MainUI", "TeamManager", "teamManager")
dep("MainUI", "CursorManager", "uses")
dep("MainUI", "MusicTransitionManager", "uses")
dep("MainUI", "ShopUI", "uses")

# PlayerUI — панель игрока с вкладками
cls("PlayerUI", "PlayerUI")
attr("PlayerUI", "mobGuide", "MobGuide", "public")
attr("PlayerUI", "tongues", "List[Tongue]", "private")
attr("PlayerUI", "playerPanel", "GameObject", "private")
attr("PlayerUI", "overlayPanel", "GameObject", "private")
attr("PlayerUI", "questTongueIndex", "int", "private")
attr("PlayerUI", "guideTongueIndex", "int", "private")
attr("PlayerUI", "onQuestTongueSelected", "Action", "public")
attr("PlayerUI", "onQuestTongueDeselected", "Action", "public")
attr("PlayerUI", "onGuideTongueSelected", "Action", "public")
op("PlayerUI", "OpenPlayerUI")
op("PlayerUI", "ToggleInventoryOnFight")
op("PlayerUI", "OnTongueSelected", vis="private")
op("PlayerUI", "SelectTongue", vis="private")
end_cls()
assoc("PlayerUI", "Tongue", "tongues", "composite", "0..*")
assoc("PlayerUI", "MobGuide", "mobGuide")

# PauseMenu
cls("PauseMenu", "PauseMenu")
attr("PauseMenu", "sceneLoader", "SceneLoader", "private")
attr("PauseMenu", "settingsPanel", "GameObject", "private")
attr("PauseMenu", "pauseMenu", "GameObject", "private")
attr("PauseMenu", "buttons", "ButtonSettings[]", "private")
attr("PauseMenu", "gameSettings", "GameSettings", "private")
attr("PauseMenu", "isActive", "bool", "public")
attr("PauseMenu", "_musicManager", "MusicTransitionManager", "private")
op("PauseMenu", "OpenPauseMenu")
op("PauseMenu", "ClosePauseMenu")
op("PauseMenu", "PauseGame")
op("PauseMenu", "ResumeGame")
end_cls()
dep("PauseMenu", "MainUI", "uses")
dep("PauseMenu", "GlobalLoader", "uses")
dep("PauseMenu", "MusicTransitionManager", "uses")

# CursorManager (Singleton)
cls("CursorManager", "CursorManager")
attr("CursorManager", "Instance", "CursorManager", "public")
attr("CursorManager", "_wantVisible", "bool", "private")
op("CursorManager", "Show")
op("CursorManager", "Hide")
op("CursorManager", "EnforceState", vis="private")
end_cls()

# ContextText (fight turn counter)
cls("ContextText", "ContextText")
attr("ContextText", "fjghtTurn", "TextMeshProUGUI", "private")
attr("ContextText", "turn", "int", "private")
op("ContextText", "ChangeTurnText")
end_cls()

# QuestPanel
cls("QuestPanel", "QuestPanel")
attr("QuestPanel", "questNameText", "TextMeshProUGUI", "private")
end_cls()

# GameAlert (prefab data)
cls("GameAlert", "GameAlert")
attr("GameAlert", "mainText", "TextMeshProUGUI", "public")
attr("GameAlert", "leftButton", "Button", "public")
attr("GameAlert", "leftButtonText", "TextMeshProUGUI", "public")
attr("GameAlert", "rightButton", "Button", "public")
attr("GameAlert", "rightButtonText", "TextMeshProUGUI", "public")
end_cls()

# GameMassage (static utility)
cls("GameMassage", "GameMassage")
op("GameMassage", "ButtonMassage")
op("GameMassage", "MassageText")
op("GameMassage", "ButtonMassageWithText")
op("GameMassage", "WarningMassage")
op("GameMassage", "GameAlert")
op("GameMassage", "CloseAlert")
op("GameMassage", "FindCanvas", vis="private")
end_cls()
dep("GameMassage", "GameAlert", "creates")

# ContentScaler
cls("ContentScaler", "ContentScaler")
attr("ContentScaler", "content", "RectTransform", "private")
attr("ContentScaler", "layoutContainer", "RectTransform", "private")
op("ContentScaler", "UpdateContentHeight")
end_cls()

# TurnPintogramm (fight UI)
cls("TurnPintogramm", "TurnPintogramm")
attr("TurnPintogramm", "pintogramm", "GameObject", "private")
attr("TurnPintogramm", "fightManager", "FightManager", "private")
end_cls()
assoc("TurnPintogramm", "FightManager", "fightManager")

# CustomInputField
cls("CustomInputField", "CustomInputField")
op("CustomInputField", "OnPointerClick")
op("CustomInputField", "OnBeginDrag")
op("CustomInputField", "OnEndDrag")
end_cls()
# CustomInputField extends TMP_InputField (external)
cls("TMP_InputField_ext", "TMP_InputField")
end_cls()
generalization("CustomInputField", "TMP_InputField_ext")

end_pkg()  # CoreUI

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Tabs
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Tabs", "Tabs")

# SelectableTab (abstract base)
cls("SelectableTab", "SelectableTab", abstract=True)
attr("SelectableTab", "index", "int", "public")
attr("SelectableTab", "objectToOpen", "GameObject", "public")
attr("SelectableTab", "isSelected", "bool", "protected")
attr("SelectableTab", "rectTransform", "RectTransform", "protected")
attr("SelectableTab", "originalPosition", "Vector2", "protected")
end_cls()

# Tongue (animated tab)
cls("Tongue", "Tongue")
attr("Tongue", "hoverOffsetY", "float", "private")
attr("Tongue", "selectedOffsetY", "float", "private")
attr("Tongue", "animationDuration", "float", "private")
attr("Tongue", "button", "Button", "private")
attr("Tongue", "IsSelected", "bool", "public")
op("Tongue", "Init")
op("Tongue", "OnPointerEnter")
op("Tongue", "OnPointerExit")
op("Tongue", "OnPointerDown")
op("Tongue", "OnPointerUp")
op("Tongue", "AnimateToSelected", vis="private")
op("Tongue", "AnimateToNormal", vis="private")
end_cls()
generalization("Tongue", "SelectableTab")

end_pkg()  # Tabs

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Tooltip
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Tooltip", "Tooltip")

cls("TMP_BoundTooltipItem", "TMP_BoundTooltipItem")
attr("TMP_BoundTooltipItem", "Instance", "TMP_BoundTooltipItem", "public")
attr("TMP_BoundTooltipItem", "TooltipText", "TextMeshProUGUI", "public")
attr("TMP_BoundTooltipItem", "ToolTipOffset", "Vector3", "public")
op("TMP_BoundTooltipItem", "ShowTooltip")
op("TMP_BoundTooltipItem", "HideTooltip")
end_cls()

cls("TMP_BoundTooltipTrigger", "TMP_BoundTooltipTrigger")
attr("TMP_BoundTooltipTrigger", "text", "string", "public")
attr("TMP_BoundTooltipTrigger", "useMousePosition", "bool", "public")
attr("TMP_BoundTooltipTrigger", "offset", "Vector3", "public")
op("TMP_BoundTooltipTrigger", "OnPointerEnter")
op("TMP_BoundTooltipTrigger", "OnPointerExit")
op("TMP_BoundTooltipTrigger", "OnSelect")
op("TMP_BoundTooltipTrigger", "OnDeselect")
end_cls()
dep("TMP_BoundTooltipTrigger", "TMP_BoundTooltipItem", "uses")

end_pkg()  # Tooltip

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: GuideBook
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_GuideBook", "GuideBook")

# IMobProvider (interface)
cls("IMobProvider", "IMobProvider", iface=True)
attr("IMobProvider", "MobData", "Mob", "public")
end_cls()

# Mob (ScriptableObject)
cls("Mob", "Mob")
attr("Mob", "nickname", "string", "public")
attr("Mob", "shortDescription", "string", "public")
attr("Mob", "longDescription", "string", "public")
attr("Mob", "sprite", "Sprite", "public")
end_cls()

# MobUI
cls("MobUI", "MobUI")
attr("MobUI", "nickname", "TextMeshProUGUI", "public")
attr("MobUI", "shortDescription", "TextMeshProUGUI", "public")
attr("MobUI", "button", "Button", "public")
end_cls()

# MobGuide
cls("MobGuide", "MobGuide")
attr("MobGuide", "mobs", "List[Mob]", "private")
attr("MobGuide", "mobsUI", "List[MobUI]", "private")
attr("MobGuide", "mobPrefab", "GameObject", "private")
attr("MobGuide", "mobImage", "Image", "private")
attr("MobGuide", "mobLongDescription", "TextMeshProUGUI", "private")
op("MobGuide", "UpdateMobsGrid")
op("MobGuide", "AddMob")
op("MobGuide", "ShowMob", vis="private")
op("MobGuide", "LoadBestiary", vis="private")
op("MobGuide", "SaveBestiary", vis="private")
op("MobGuide", "CreateMobs", vis="private")
end_cls()
assoc("MobGuide", "Mob", "mobs", "shared", "0..*")
assoc("MobGuide", "MobUI", "mobsUI", "composite", "0..*")
dep("MobGuide", "SaveLoadSystem", "uses")

# BestiaryData
cls("BestiaryData", "BestiaryData")
attr("BestiaryData", "mobNames", "List[string]", "public")
end_cls()

end_pkg()  # GuideBook

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: PlayerCreation
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_PlayerCreation", "PlayerCreation")

# IUpdatableUI (interface)
cls("IUpdatableUI", "IUpdatableUI", iface=True)
attr("IUpdatableUI", "DescriptionText", "TextMeshProUGUI", "public")
op("IUpdatableUI", "UpdateUI", abstract=True)
end_cls()

# PointsManager
cls("PointsManager", "PointsManager")
attr("PointsManager", "maxPoints", "int", "public")
attr("PointsManager", "usedPoints", "int", "public")
attr("PointsManager", "pointsData", "List[PointsData]", "private")
attr("PointsManager", "characterData", "CharacterData", "private")
op("PointsManager", "CanAddPoint", "bool")
op("PointsManager", "AddPoint")
op("PointsManager", "RemovePoint")
op("PointsManager", "AddPointsToPlayer")
op("PointsManager", "GetDescription", "string")
end_cls()
dep("PointsManager", "CharacterData", "modifies")

# PointsData (nested Serializable)
cls("PointsData", "PointsData")
attr("PointsData", "statType", "StatType", "public")
attr("PointsData", "choosing", "Choosing", "public")
attr("PointsData", "baseValue", "int", "public")
attr("PointsData", "multiplier", "float", "public")
attr("PointsData", "baseValue2", "int", "public")
attr("PointsData", "multiplier2", "float", "public")
end_cls()
assoc("PointsManager", "PointsData", "pointsData", "composite", "0..*")

# Choosing
cls("Choosing", "Choosing")
attr("Choosing", "statType", "StatType", "private")
attr("Choosing", "valueText", "TextMeshProUGUI", "private")
attr("Choosing", "descriptionText", "TextMeshProUGUI", "private")
attr("Choosing", "pointsManager", "PointsManager", "private")
attr("Choosing", "maxStatValue", "int", "private")
attr("Choosing", "currentValue", "int", "public")
op("Choosing", "UpdateUI")
op("Choosing", "ChangeValue", vis="private")
end_cls()
realization("Choosing", "IUpdatableUI")
assoc("Choosing", "PointsManager", "pointsManager")

# Apperance
cls("Apperance", "Apperance")
attr("Apperance", "descriptionText", "TextMeshProUGUI", "private")
attr("Apperance", "leftButton", "Button", "private")
attr("Apperance", "rightButton", "Button", "private")
attr("Apperance", "valueText", "TextMeshProUGUI", "private")
attr("Apperance", "appProperty", "List[ApperanceProperty]", "private")
attr("Apperance", "currentValue", "int", "private")
op("Apperance", "UpdateUI")
op("Apperance", "ChangeValue", vis="private")
end_cls()
realization("Apperance", "IUpdatableUI")

# HeroSwitcher
cls("HeroSwitcher", "HeroSwitcher")
attr("HeroSwitcher", "heroes", "List[GameObject]", "private")
attr("HeroSwitcher", "currentIndex", "int", "private")
op("HeroSwitcher", "NextHero")
op("HeroSwitcher", "PrevHero")
op("HeroSwitcher", "ShowHero", vis="private")
end_cls()

# HoverHandler
cls("HoverHandler", "HoverHandler")
attr("HoverHandler", "descriptionText", "string", "private")
attr("HoverHandler", "points", "PointsManager", "private")
attr("HoverHandler", "uiHandler", "IUpdatableUI", "private")
op("HoverHandler", "OnPointerEnter")
op("HoverHandler", "OnPointerExit")
end_cls()
assoc("HoverHandler", "PointsManager", "points")
dep("HoverHandler", "IUpdatableUI", "uses")

# UploadTarget
cls("UploadTarget", "UploadTarget")
attr("UploadTarget", "nextSceneLoader", "SceneLoader", "private")
attr("UploadTarget", "points", "PointsManager", "private")
attr("UploadTarget", "descriptionStats", "TextMeshProUGUI", "private")
attr("UploadTarget", "inputField", "CustomInputField", "private")
attr("UploadTarget", "maxNameLength", "int", "private")
attr("UploadTarget", "playerInstance", "Player", "private")
attr("UploadTarget", "mainUiInstance", "MainUI", "private")
op("UploadTarget", "NextScene")
op("UploadTarget", "RestoreValues", vis="private")
op("UploadTarget", "OnNameChanged", vis="private")
end_cls()
assoc("UploadTarget", "PointsManager", "points")
assoc("UploadTarget", "CustomInputField", "inputField")
dep("UploadTarget", "GlobalLoader", "uses")
dep("UploadTarget", "MainUI", "uses")

# Enums
enum("StatType", "StatType", ["Power","Intellect","Charisma","Lucky","HP","MP"])

end_pkg()  # PlayerCreation

# ── write deferred relationships ──────────────────────────────────────────────
for rel in deferred:
    lines.append(rel)

lines.append('</uml:Model>')

content = "\n".join(lines)
with open("EchoRift_UI.xml", "w", encoding="utf-8") as f:
    f.write(content)

import xml.etree.ElementTree as ET
try:
    ET.parse("EchoRift_UI.xml")
    gen   = sum(1 for l in lines if 'uml:Generalization"'      in l)
    real  = sum(1 for l in lines if 'uml:InterfaceRealization"' in l)
    asc   = sum(1 for l in lines if 'uml:Association"'          in l)
    d     = sum(1 for l in lines if 'uml:Dependency"'           in l)
    c     = sum(1 for l in lines if ('uml:Class"' in l or 'uml:Interface"' in l or 'uml:Enumeration"' in l))
    print(f"XML valid!  {len(lines)} lines")
    print(f"  Classes/Interfaces/Enums : {c}")
    print(f"  Generalization           : {gen}")
    print(f"  InterfaceRealization     : {real}")
    print(f"  Association              : {asc}")
    print(f"  Dependency               : {d}")
except ET.ParseError as e:
    src = content.split('\n')
    ln = e.position[0]
    print("ERROR:", e)
    print("Line", ln, ":", repr(src[ln-1]))
