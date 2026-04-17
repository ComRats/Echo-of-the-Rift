#!/usr/bin/env python3
"""Generate UML2 XMI Component Diagram for UI module (EA-compatible)"""
import uuid

def uid(n):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.ui.comp." + n)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="UI_Component">')
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
pkg("PKG_UI_COMP", "UI_Component")

# ── Interfaces ────────────────────────────────────────────────────────────────
interface("IUIController", "IUIController",
    ["Show","Hide","ToggleInventory","ToggleQuestLog",
     "OpenInventory","CloseInventory","ShowCursor","HideCursor"])

interface("IPauseController", "IPauseController",
    ["OpenPauseMenu","ClosePauseMenu","PauseGame","ResumeGame"])

interface("ISceneLifecycle", "ISceneLifecycle",
    ["SavePlayer","SaveGlobal","LoadToScene","Show","Hide"])

interface("ITabSelectable", "ITabSelectable",
    ["Init","IsSelected"])

# ── Component: MainUI ─────────────────────────────────────────────────────────
component("COMP_MainUI", "MainUI")
provided("COMP_MainUI", "IUIController")
end_component()
dependency("COMP_MainUI", "COMP_PauseMenu",              "owns")
dependency("COMP_MainUI", "COMP_PlayerUI",               "owns")
dependency("COMP_MainUI", "COMP_ScreenFader",            "uses")
dependency("COMP_MainUI", "COMP_InventoryManager",       "owns")
dependency("COMP_MainUI", "COMP_TeamManager",            "owns")
dependency("COMP_MainUI", "COMP_ShopUI",                 "owns")
dependency("COMP_MainUI", "COMP_FishingUI",              "owns")
dependency("COMP_MainUI", "COMP_QuestLogWindow",         "owns")
dependency("COMP_MainUI", "COMP_MusicTransitionManager", "duck/restore via")
dependency("COMP_MainUI", "COMP_IAudioManager",          "plays UI sounds")
dependency("COMP_MainUI", "COMP_GameSettings",           "reads hotkeys from")
dependency("COMP_MainUI", "COMP_CursorManager",          "delegates cursor to")

# ── Component: PauseMenu ──────────────────────────────────────────────────────
component("COMP_PauseMenu", "PauseMenu")
provided("COMP_PauseMenu", "IPauseController")
end_component()
usage("COMP_PauseMenu", "IUIController")
dependency("COMP_PauseMenu", "COMP_GameTimer",           "pause/resume via")
dependency("COMP_PauseMenu", "COMP_MusicTransitionManager","duck/restore via")
dependency("COMP_PauseMenu", "COMP_GlobalLoader",        "save/load via")
dependency("COMP_PauseMenu", "COMP_SceneLoader",         "exit to menu via")
dependency("COMP_PauseMenu", "COMP_GameSettings",        "reads hotkey from")

# ── Component: PlayerUI ───────────────────────────────────────────────────────
component("COMP_PlayerUI", "PlayerUI")
end_component()
usage("COMP_PlayerUI", "ITabSelectable")
dependency("COMP_PlayerUI", "COMP_Tongue",       "contains tabs")
dependency("COMP_PlayerUI", "COMP_MobGuide",     "owns")
dependency("COMP_PlayerUI", "COMP_QuestLogWindow","opens/closes")

# ── Component: Tongue ─────────────────────────────────────────────────────────
component("COMP_Tongue", "Tongue")
provided("COMP_Tongue", "ITabSelectable")
end_component()

# ── Component: ScreenFader ────────────────────────────────────────────────────
component("COMP_ScreenFader", "ScreenFader")
end_component()

# ── Component: CursorManager ──────────────────────────────────────────────────
component("COMP_CursorManager", "CursorManager")
end_component()

# ── Component: GameTimer ──────────────────────────────────────────────────────
component("COMP_GameTimer", "GameTimer")
end_component()

# ── Component: QuestLogWindow ─────────────────────────────────────────────────
component("COMP_QuestLogWindow", "QuestLogWindow")
end_component()

# ── Component: MobGuide ───────────────────────────────────────────────────────
component("COMP_MobGuide", "MobGuide")
end_component()

# ── Component: GameAlert / GameMassage ────────────────────────────────────────
component("COMP_GameAlert", "GameAlert")
end_component()

component("COMP_GameMassage", "GameMassage")
end_component()
dependency("COMP_GameMassage", "COMP_GameAlert", "instantiates")

# ── Component: ContextText ────────────────────────────────────────────────────
component("COMP_ContextText", "ContextText")
end_component()

# ── Component: TurnPintogramm ─────────────────────────────────────────────────
component("COMP_TurnPintogramm", "TurnPintogramm")
end_component()

# ── Component: GlobalLoader ───────────────────────────────────────────────────
component("COMP_GlobalLoader", "GlobalLoader")
provided("COMP_GlobalLoader", "ISceneLifecycle")
end_component()
usage("COMP_GlobalLoader", "IUIController")
dependency("COMP_GlobalLoader", "COMP_MainUI",        "owns ref")
dependency("COMP_GlobalLoader", "COMP_SaveLoadSystem","persists via")
dependency("COMP_GlobalLoader", "COMP_SceneLoader",   "loads scenes via")
dependency("COMP_GlobalLoader", "COMP_GameTimer",     "controls time")
dependency("COMP_GlobalLoader", "COMP_ScreenFader",   "fades via MainUI")

# ── Component: MainMenu ───────────────────────────────────────────────────────
component("COMP_MainMenu", "MainMenu")
end_component()
usage("COMP_MainMenu", "ISceneLifecycle")
dependency("COMP_MainMenu", "COMP_SaveLoadSystem", "checks/clears saves")
dependency("COMP_MainMenu", "COMP_SceneLoader",    "loads game scene")
dependency("COMP_MainMenu", "COMP_GameMassage",    "shows alerts")
dependency("COMP_MainMenu", "COMP_GlobalLoader",   "resets state via")

# ── Component: UploadTarget (PlayerCreation) ──────────────────────────────────
component("COMP_UploadTarget", "UploadTarget")
end_component()
usage("COMP_UploadTarget", "IUIController")
dependency("COMP_UploadTarget", "COMP_GlobalLoader",  "refreshes player data")
dependency("COMP_UploadTarget", "COMP_SceneLoader",   "loads next scene")
dependency("COMP_UploadTarget", "COMP_PointsManager", "validates points")
dependency("COMP_UploadTarget", "COMP_GameTimer",     "resumes game")

# ── Component: QuestPanel ─────────────────────────────────────────────────────
component("COMP_QuestPanel", "QuestPanel")
end_component()
dependency("COMP_QuestPanel", "COMP_QuestLogWindow", "wraps")

# ── Component: ContentScaler ──────────────────────────────────────────────────
component("COMP_ContentScaler", "ContentScaler")
end_component()

# ── External components ───────────────────────────────────────────────────────
component("COMP_SaveLoadSystem", "SaveLoadSystem")
end_component()

component("COMP_SceneLoader", "SceneLoader")
end_component()

component("COMP_GameSettings", "GameSettings")
end_component()

component("COMP_IAudioManager", "IAudioManager")
end_component()

component("COMP_MusicTransitionManager", "MusicTransitionManager")
end_component()

component("COMP_InventoryManager", "InventoryManager")
end_component()

component("COMP_TeamManager", "TeamManager")
end_component()

component("COMP_ShopUI", "ShopUI")
end_component()

component("COMP_FishingUI", "FishingUI")
end_component()

component("COMP_PointsManager", "PointsManager")
end_component()

# ── write deferred ────────────────────────────────────────────────────────────
for rel in deferred:
    lines.append(rel)

end_pkg()
lines.append('</uml:Model>')

content = "\n".join(lines)
out = "EchoRift_UI_Component.xml"
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
