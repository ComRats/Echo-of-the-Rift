#!/usr/bin/env python3
"""Generate UML2 XMI Activity Diagrams for UI module (EA-compatible)"""
import uuid

def uid(n):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.ui.act." + n)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="UI_Activity">')
lines.append('')

def begin_pkg(key, name):
    lines.append(f'  <packagedElement xmi:type="uml:Package" xmi:id="{uid(key)}" name="{name}">')

def end_pkg():
    lines.append('  </packagedElement>')

def begin_activity(key, name):
    lines.append(f'    <packagedElement xmi:type="uml:Activity" xmi:id="{uid(key)}" name="{name}" visibility="public">')

def end_activity():
    lines.append('    </packagedElement>')

def initial(key):
    lines.append(f'      <ownedNode xmi:type="uml:InitialNode" xmi:id="{uid(key)}" name=""/>')

def final(key):
    lines.append(f'      <ownedNode xmi:type="uml:ActivityFinalNode" xmi:id="{uid(key)}" name=""/>')

def action(key, name):
    safe = name.replace("&","&amp;").replace("<","&lt;").replace(">","&gt;")
    lines.append(f'      <ownedNode xmi:type="uml:OpaqueAction" xmi:id="{uid(key)}" name="{safe}"/>')

def decision(key, name):
    safe = name.replace("&","&amp;").replace("<","&lt;").replace(">","&gt;")
    lines.append(f'      <ownedNode xmi:type="uml:DecisionNode" xmi:id="{uid(key)}" name="{safe}"/>')

def flow(key, src, tgt, guard=""):
    g = f' guard="{guard}"' if guard else ''
    lines.append(
        f'      <ownedEdge xmi:type="uml:ControlFlow" xmi:id="{uid(key)}"'
        f' source="{uid(src)}" target="{uid(tgt)}"{g}/>'
    )

# ══════════════════════════════════════════════════════════════════════════════
begin_pkg("PKG_UI_ACT", "UI_Activity")

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 1: Main Menu
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_MM", "Main Menu")

initial  ("MM_INIT")
action   ("MM_AWAKE",       "Awake: проверить SaveLoadSystem.Exists(GLOBAL_SAVE)\nесли нет сохранения — скрыть кнопку Load")
action   ("MM_START",       "Start: запустить анимацию кнопок\n(Show / ButtonsShowLoad)")
action   ("MM_WAIT",        "Ожидание действия игрока")
decision ("MM_DEC_ACTION",  "Действие игрока?")
action   ("MM_TRY_PLAY",    "TryPlay():\nпроверить HasGameProgress")
decision ("MM_DEC_PROGRESS","Есть прогресс сохранения?")
action   ("MM_ALERT",       "GameMassage.GameAlert:\n'Все сохранения удалятся'\n[Нет] / [Да]")
decision ("MM_DEC_CONFIRM", "Игрок подтвердил?")
action   ("MM_PLAY",        "Play():\nClearAllSaves, ResetCache\nDialogueSaveManager.Delete\nResetForNewGame\ncharacterData.ResetToDefaults\nSave(GLOBAL_SAVE, isStart=true)\nstartSceneLoader.LoadAsync()")
action   ("MM_LOAD",        "Load():\nLoadGlobalData -> SceneIndex\nDialogueSaveManager.Load()\nSaveNameForDialogueActor\nloadSceneLoader.LoadAsync(sceneIndex)")
action   ("MM_QUIT",        "Quit():\nGameMassage.GameAlert\n[Да] -> Application.Quit()")
action   ("MM_CREDITS",     "Credits():\nApplication.OpenURL(comrats.github.io)")
final    ("MM_FINAL")

flow("MM_E01","MM_INIT",       "MM_AWAKE")
flow("MM_E02","MM_AWAKE",      "MM_START")
flow("MM_E03","MM_START",      "MM_WAIT")
flow("MM_E04","MM_WAIT",       "MM_DEC_ACTION")
flow("MM_E05","MM_DEC_ACTION", "MM_TRY_PLAY",    "[Новая игра]")
flow("MM_E06","MM_DEC_ACTION", "MM_LOAD",        "[Загрузить]")
flow("MM_E07","MM_DEC_ACTION", "MM_QUIT",        "[Выход]")
flow("MM_E08","MM_DEC_ACTION", "MM_CREDITS",     "[Авторы]")
flow("MM_E09","MM_TRY_PLAY",   "MM_DEC_PROGRESS")
flow("MM_E10","MM_DEC_PROGRESS","MM_ALERT",      "[да — есть прогресс]")
flow("MM_E11","MM_DEC_PROGRESS","MM_PLAY",       "[нет]")
flow("MM_E12","MM_ALERT",      "MM_DEC_CONFIRM")
flow("MM_E13","MM_DEC_CONFIRM","MM_WAIT",        "[Нет — отмена]")
flow("MM_E14","MM_DEC_CONFIRM","MM_PLAY",        "[Да — подтверждено]")
flow("MM_E15","MM_PLAY",       "MM_FINAL")
flow("MM_E16","MM_LOAD",       "MM_FINAL")
flow("MM_E17","MM_QUIT",       "MM_FINAL")
flow("MM_E18","MM_CREDITS",    "MM_WAIT")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 2: Toggle Inventory (MainUI)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_INV", "Toggle Inventory")

initial  ("TI_INIT")
action   ("TI_KEY",         "Input.GetKeyDown(openInventoryKey)")
decision ("TI_DEC_SHOP",    "ShopUI.IsShopMode?")
action   ("TI_CLOSE_SHOP",  "shopUI.CloseShop()")
decision ("TI_DEC_CAN",     "canOpenUI == true?")
action   ("TI_SKIP",        "return — UI заблокирован")
decision ("TI_DEC_OPEN",    "playerUIbackGround.activeSelf?")
action   ("TI_OPEN",        "OpenInventory():\nShowCursor (если не бой)\nPlayOneShot(OpenUI)\nplayerUIbackGround.SetActive(true)\nOpenPlayerUI()\nUpdateCharacterSlots()\nGameTimer.PauseGame (если не бой)\nmusicManager.DuckMusic()")
action   ("TI_CLOSE",       "CloseInventory():\nHideCursor (если не бой)\nPlayOneShot(OpenUI_R)\nplayerUIbackGround.SetActive(false)\nGameTimer.ResumeGame (если не бой)\nquestLogWindow.Close()\ncontextMenu.Hide()\nmusicManager.RestoreMusic()")
final    ("TI_FINAL")

flow("TI_E01","TI_INIT",      "TI_KEY")
flow("TI_E02","TI_KEY",       "TI_DEC_SHOP")
flow("TI_E03","TI_DEC_SHOP",  "TI_CLOSE_SHOP",  "[да]")
flow("TI_E04","TI_CLOSE_SHOP","TI_FINAL")
flow("TI_E05","TI_DEC_SHOP",  "TI_DEC_CAN",     "[нет]")
flow("TI_E06","TI_DEC_CAN",   "TI_SKIP",        "[нет]")
flow("TI_E07","TI_SKIP",      "TI_FINAL")
flow("TI_E08","TI_DEC_CAN",   "TI_DEC_OPEN",    "[да]")
flow("TI_E09","TI_DEC_OPEN",  "TI_CLOSE",       "[да — открыт]")
flow("TI_E10","TI_DEC_OPEN",  "TI_OPEN",        "[нет — закрыт]")
flow("TI_E11","TI_OPEN",      "TI_FINAL")
flow("TI_E12","TI_CLOSE",     "TI_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 3: Pause Menu
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_PM", "Pause Menu")

initial  ("PM_INIT")
action   ("PM_KEY",         "Input.GetKeyDown(openPauseMenuKey)")
decision ("PM_DEC_SHOP",    "ShopUI.IsShopMode?")
action   ("PM_CLOSE_SHOP",  "shopUI.CloseShop()")
decision ("PM_DEC_INV",     "mainUI.isOpenUI?")
action   ("PM_CLOSE_INV",   "mainUI.CloseInventory()")
decision ("PM_DEC_ACTIVE",  "pauseMenu.activeSelf?")
action   ("PM_OPEN",        "OpenPauseMenu():\nShowCursor\nGameTimer.PauseGame\npauseMenu.SetActive(true)\npauseMenuBackGround.SetActive(true)")
action   ("PM_CLOSE",       "ClosePauseMenu():\nHideCursor\nGameTimer.ResumeGame\npauseMenu.SetActive(false)")
action   ("PM_WAIT",        "Ожидание действия в меню паузы")
decision ("PM_DEC_BTN",     "Кнопка?")
action   ("PM_RESUME",      "ClosePauseMenu()")
action   ("PM_SAVE",        "GlobalLoader.SavePlayer()\nSaveGlobal()\nSaveInventory()")
action   ("PM_SAVE_EXIT",   "SavePlayer, SaveGlobal, SaveInventory\nsceneLoader.LoadAsync() -> главное меню")
action   ("PM_SETTINGS",    "Показать/скрыть settingsPanel\n(анимация ShowSettings / HideSettings)")
action   ("PM_GAME_PAUSED", "Событие GameTimer.OnGamePaused:\nmusicManager.DuckMusic()")
action   ("PM_GAME_RESUMED","Событие GameTimer.OnGameResumed:\nmusicManager.RestoreMusic()")
final    ("PM_FINAL")

flow("PM_E01","PM_INIT",      "PM_KEY")
flow("PM_E02","PM_KEY",       "PM_DEC_SHOP")
flow("PM_E03","PM_DEC_SHOP",  "PM_CLOSE_SHOP",  "[да]")
flow("PM_E04","PM_CLOSE_SHOP","PM_FINAL")
flow("PM_E05","PM_DEC_SHOP",  "PM_DEC_INV",     "[нет]")
flow("PM_E06","PM_DEC_INV",   "PM_CLOSE_INV",   "[да — инвентарь открыт]")
flow("PM_E07","PM_CLOSE_INV", "PM_FINAL")
flow("PM_E08","PM_DEC_INV",   "PM_DEC_ACTIVE",  "[нет]")
flow("PM_E09","PM_DEC_ACTIVE","PM_CLOSE",        "[да — меню открыто]")
flow("PM_E10","PM_DEC_ACTIVE","PM_OPEN",         "[нет]")
flow("PM_E11","PM_OPEN",      "PM_WAIT")
flow("PM_E12","PM_WAIT",      "PM_DEC_BTN")
flow("PM_E13","PM_DEC_BTN",   "PM_RESUME",      "[Продолжить]")
flow("PM_E14","PM_DEC_BTN",   "PM_SETTINGS",    "[Настройки]")
flow("PM_E15","PM_DEC_BTN",   "PM_SAVE",        "[Сохранить]")
flow("PM_E16","PM_DEC_BTN",   "PM_SAVE_EXIT",   "[Сохранить и выйти]")
flow("PM_E17","PM_RESUME",    "PM_CLOSE")
flow("PM_E18","PM_SAVE",      "PM_WAIT")
flow("PM_E19","PM_SETTINGS",  "PM_WAIT")
flow("PM_E20","PM_CLOSE",     "PM_FINAL")
flow("PM_E21","PM_SAVE_EXIT", "PM_FINAL")
flow("PM_E22","PM_INIT",      "PM_GAME_PAUSED")
flow("PM_E23","PM_GAME_PAUSED","PM_GAME_RESUMED")
flow("PM_E24","PM_GAME_RESUMED","PM_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 4: Player UI Tabs (Tongue / SelectableTab)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_TAB", "Player UI Tabs")

initial  ("TB_INIT")
action   ("TB_OPEN",        "OpenPlayerUI(tongueIndex):\nинициализировать все Tongue.Init(OnTongueSelected)\nSelectTongue(index)")
action   ("TB_HOVER",       "OnPointerEnter:\nAnimateToHover (DOAnchorPos + DOScale)")
action   ("TB_PRESS",       "OnPointerDown:\nAnimateToPress")
action   ("TB_CLICK",       "OnPointerUp + onClick:\nOnTongueSelected(selectedIndex)")
decision ("TB_DEC_IDX",     "selectedIndex?")
action   ("TB_SEL_INV",     "Инвентарь (0):\nIsSelected=true -> objectToOpen.SetActive(true)\nAnimateToSelected")
action   ("TB_SEL_STATS",   "Статы (1):\nIsSelected=true -> objectToOpen.SetActive(true)")
action   ("TB_SEL_GUIDE",   "Гайд (2):\nIsSelected=true\nonGuideTongueSelected -> mobGuide.UpdateMobsGrid()")
action   ("TB_SEL_QUEST",   "Квесты (3):\nIsSelected=true\nonQuestTongueSelected -> questLogWindow.Open()")
action   ("TB_DESEL_QUEST", "Предыдущий квест-таб:\nonQuestTongueDeselected -> questLogWindow.Close()")
action   ("TB_DESEL_OTHER", "Остальные табы:\nIsSelected=false -> objectToOpen.SetActive(false)\nAnimateToNormal")
final    ("TB_FINAL")

flow("TB_E01","TB_INIT",      "TB_OPEN")
flow("TB_E02","TB_OPEN",      "TB_HOVER")
flow("TB_E03","TB_HOVER",     "TB_PRESS")
flow("TB_E04","TB_PRESS",     "TB_CLICK")
flow("TB_E05","TB_CLICK",     "TB_DEC_IDX")
flow("TB_E06","TB_DEC_IDX",   "TB_SEL_INV",     "[0 — Инвентарь]")
flow("TB_E07","TB_DEC_IDX",   "TB_SEL_STATS",   "[1 — Статы]")
flow("TB_E08","TB_DEC_IDX",   "TB_SEL_GUIDE",   "[2 — Гайд]")
flow("TB_E09","TB_DEC_IDX",   "TB_SEL_QUEST",   "[3 — Квесты]")
flow("TB_E10","TB_SEL_QUEST", "TB_DESEL_OTHER")
flow("TB_E11","TB_SEL_INV",   "TB_DESEL_QUEST")
flow("TB_E12","TB_SEL_STATS", "TB_DESEL_QUEST")
flow("TB_E13","TB_SEL_GUIDE", "TB_DESEL_QUEST")
flow("TB_E14","TB_DESEL_QUEST","TB_DESEL_OTHER")
flow("TB_E15","TB_DESEL_OTHER","TB_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 5: Player Creation (UploadTarget)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_PC", "Player Creation")

initial  ("PC_INIT")
action   ("PC_AWAKE",       "Awake: DontDestroyOnLoad")
action   ("PC_START",       "Start: mainUI.canOpenUI = false\nActivateInputField()")
action   ("PC_INPUT",       "Игрок вводит имя персонажа\nonValueChanged -> очистить descriptionStats")
action   ("PC_SUBMIT",      "NextScene() (Enter или кнопка)")
decision ("PC_DEC_NAME",    "inputField.text пустой?")
action   ("PC_WARN_NAME",   "descriptionStats = 'Введите имя!'\nActivateInputField()")
decision ("PC_DEC_POINTS",  "usedPoints >= maxPoints?")
action   ("PC_WARN_POINTS", "descriptionStats = 'Распределите очки: N'")
action   ("PC_APPLY",       "points.AddPointsToPlayer()\ndescriptionStats = 'Загрузка...'\nRefreshPlayerDataFromCharacterData()\nRestoreValues()")
action   ("PC_SAVE_NAME",   "SaveNameForDialogueActor(playerName)\nApplyName()\nPlayerDataHolder.PlayerName = name")
action   ("PC_LOAD",        "nextSceneLoader._onSceneActivated:\nStartConversationDelay()\nmovement.CanMoveTrue()\nnextSceneLoader.LoadAsync()")
final    ("PC_FINAL")

flow("PC_E01","PC_INIT",      "PC_AWAKE")
flow("PC_E02","PC_AWAKE",     "PC_START")
flow("PC_E03","PC_START",     "PC_INPUT")
flow("PC_E04","PC_INPUT",     "PC_SUBMIT")
flow("PC_E05","PC_SUBMIT",    "PC_DEC_NAME")
flow("PC_E06","PC_DEC_NAME",  "PC_WARN_NAME",   "[да — пустое]")
flow("PC_E07","PC_WARN_NAME", "PC_INPUT")
flow("PC_E08","PC_DEC_NAME",  "PC_DEC_POINTS",  "[нет]")
flow("PC_E09","PC_DEC_POINTS","PC_WARN_POINTS", "[нет — очки не распределены]")
flow("PC_E10","PC_WARN_POINTS","PC_INPUT")
flow("PC_E11","PC_DEC_POINTS","PC_APPLY",       "[да — все очки распределены]")
flow("PC_E12","PC_APPLY",     "PC_SAVE_NAME")
flow("PC_E13","PC_SAVE_NAME", "PC_LOAD")
flow("PC_E14","PC_LOAD",      "PC_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 6: GlobalLoader Scene Lifecycle
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_GL", "GlobalLoader Scene Lifecycle")

initial  ("GL_INIT")
action   ("GL_AWAKE",       "Awake: Singleton, DontDestroyOnLoad\nSceneManager.sceneLoaded += OnSceneLoaded\nLoadGlobal(), LoadPlayerData()\nSetListenerToEvents(OnConversationStart, OnConversationEnd)")
action   ("GL_SCENE_LOAD",  "Событие: OnSceneLoaded(scene)")
action   ("GL_RESTORE_DLG", "RestoreDialogueState():\nDialogueSaveManager.Load()")
decision ("GL_DEC_DICE",    "scene.name == Dice?")
action   ("GL_PERSIST",     "PersistentObject.LoadAll()")
action   ("GL_LOAD_PLAYER", "LoadPlayer():\nпроверить SceneTransitionData.NextPosition\nили загрузить позицию из файла")
action   ("GL_RESET_UI",    "mainUI.ResetUIState()\ncanOpenUI = !isMenuScene\nteamManager.UpdateTeamUI()")
decision ("GL_DEC_RESTORE", "shouldRestoreAfterIsolatedSceneLoad?")
action   ("GL_EXIT_ISO",    "ExitIsolatedScene():\nвосстановить Player, MainUI, курсор\nRestoreGameplayAudioListener()")
action   ("GL_CONV_START",  "Событие: OnConversationStart:\nmainUI.canOpenUI = false")
action   ("GL_CONV_END",    "Событие: OnConversationEnd:\nmainUI.canOpenUI = true")
final    ("GL_FINAL")

flow("GL_E01","GL_INIT",       "GL_AWAKE")
flow("GL_E02","GL_AWAKE",      "GL_SCENE_LOAD")
flow("GL_E03","GL_SCENE_LOAD", "GL_RESTORE_DLG")
flow("GL_E04","GL_RESTORE_DLG","GL_DEC_DICE")
flow("GL_E05","GL_DEC_DICE",   "GL_LOAD_PLAYER", "[да — Dice сцена]")
flow("GL_E06","GL_DEC_DICE",   "GL_PERSIST",     "[нет]")
flow("GL_E07","GL_PERSIST",    "GL_LOAD_PLAYER")
flow("GL_E08","GL_LOAD_PLAYER","GL_RESET_UI")
flow("GL_E09","GL_RESET_UI",   "GL_DEC_RESTORE")
flow("GL_E10","GL_DEC_RESTORE","GL_EXIT_ISO",    "[да]")
flow("GL_E11","GL_DEC_RESTORE","GL_FINAL",       "[нет]")
flow("GL_E12","GL_EXIT_ISO",   "GL_FINAL")
flow("GL_E13","GL_AWAKE",      "GL_CONV_START")
flow("GL_E14","GL_CONV_START", "GL_CONV_END")
flow("GL_E15","GL_CONV_END",   "GL_FINAL")

end_activity()

end_pkg()

lines.append('</uml:Model>')

content = "\n".join(lines)
out = "EchoRift_UI_Activity.xml"
with open(out, "w", encoding="utf-8") as f:
    f.write(content)

import xml.etree.ElementTree as ET
try:
    ET.parse(out)
    acts  = sum(1 for l in lines if 'uml:Activity"'  in l)
    nodes = sum(1 for l in lines if 'ownedNode'       in l)
    edges = sum(1 for l in lines if 'ownedEdge'       in l)
    print(f"XML valid!  {len(lines)} lines -> {out}")
    print(f"  Activities : {acts}")
    print(f"  Nodes      : {nodes}")
    print(f"  Edges      : {edges}")
except ET.ParseError as e:
    src = content.split('\n')
    ln = e.position[0]
    print("ERROR:", e)
    print("Line", ln, ":", repr(src[ln-1]))
