#!/usr/bin/env python3
"""Generate UML2 XMI Activity Diagrams for FightSystem module (EA-compatible)"""
import uuid

def uid(n):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.fight.act." + n)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="FightSystem_Activity">')
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

def merge(key):
    lines.append(f'      <ownedNode xmi:type="uml:MergeNode" xmi:id="{uid(key)}" name=""/>')

def flow(key, src, tgt, guard=""):
    g = f' guard="{guard}"' if guard else ''
    lines.append(
        f'      <ownedEdge xmi:type="uml:ControlFlow" xmi:id="{uid(key)}"'
        f' source="{uid(src)}" target="{uid(tgt)}"{g}/>'
    )

# ══════════════════════════════════════════════════════════════════════════════
begin_pkg("PKG_FIGHT_ACT", "FightSystem_Activity")

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 1: Fight Trigger — запуск боя из мира
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_FT", "Fight Trigger")

initial  ("FT_INIT")
action   ("FT_START",        "Start: получить fightSceneLoader из GlobalLoader")
action   ("FT_TRIGGER",      "Событие: OnTriggerEnter2D / OnTriggerStay2D(collision)")
decision ("FT_DEC_PLAYER",   "collision — это Player AND canTriggered?")
decision ("FT_DEC_STARTING", "_isStartingFight уже true?")
action   ("FT_SET_FLAGS",    "_isStartingFight = true\ncanTriggered = false")
action   ("FT_GET_TEAM",     "characters = collision.GetComponent<Team>().characters")
action   ("FT_SAVE_ENEMIES", "SaveEnemiesToFile()\n-> SaveLoadSystem.Save(ENEMY_SAVE, FightData)")
action   ("FT_SAVE_CHARS",   "SaveCharactersToFile()\n-> SaveLoadSystem.Save(CHARACTER_SAVE, CharacterDataWrapper)")
action   ("FT_SUBSCRIBE",    "sceneLoader._onLoadingSceneLoad += GlobalLoader.Hide()\nsceneLoader._onSceneActivated += CursorManager.Show()")
action   ("FT_LOAD",         "sceneLoader.LoadAsync() -> загрузка боевой сцены")
final    ("FT_FINAL")

flow("FT_E01","FT_INIT",       "FT_START")
flow("FT_E02","FT_START",      "FT_TRIGGER")
flow("FT_E03","FT_TRIGGER",    "FT_DEC_PLAYER")
flow("FT_E04","FT_DEC_PLAYER", "FT_TRIGGER",    "[нет — игнорировать]")
flow("FT_E05","FT_DEC_PLAYER", "FT_DEC_STARTING","[да]")
flow("FT_E06","FT_DEC_STARTING","FT_TRIGGER",   "[да — уже запускается]")
flow("FT_E07","FT_DEC_STARTING","FT_SET_FLAGS", "[нет]")
flow("FT_E08","FT_SET_FLAGS",  "FT_GET_TEAM")
flow("FT_E09","FT_GET_TEAM",   "FT_SAVE_ENEMIES")
flow("FT_E10","FT_SAVE_ENEMIES","FT_SAVE_CHARS")
flow("FT_E11","FT_SAVE_CHARS", "FT_SUBSCRIBE")
flow("FT_E12","FT_SUBSCRIBE",  "FT_LOAD")
flow("FT_E13","FT_LOAD",       "FT_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 2: Fight Manager — основной игровой цикл боя
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_FM", "Fight Manager — Battle Loop")

initial  ("FM_INIT")
action   ("FM_START",         "Start: InitializationLists()\nInitialization()\nEquipmentManager.SetBattleState(true)")
action   ("FM_SORT",          "Отсортировать bases по Priority (враги + персонажи)")
action   ("FM_LOOP_BEGIN",    "Итерация по bases[i]")
decision ("FM_DEC_ALIVE",     "base.Health > 0?")
action   ("FM_PROCESS_FX",    "base.ProcessStatusEffects()\nждать 0.5 сек")
decision ("FM_DEC_DEAD_AFTER","base.Health <= 0 после эффектов?")
decision ("FM_DEC_IS_ENEMY",  "base — это Enemy?")
action   ("FM_ENEMY_TURN",    "Ход врага:\nGetCharacterLowestHP() -> target\nSetAnimationSpeed(enemySpeed)\ntarget.TakeDamage(enemy.GiveDamage())\ntarget.PlayAnimation(attackAnim)\nDeleteCharacterOnList(target)")
action   ("FM_CHAR_TURN",     "Ход персонажа:\nWaitCharacterTurn(character)\n(ждать пока IsTurn == false)")
decision ("FM_DEC_MORE",      "Ещё есть bases?")
action   ("FM_WAIT_DELETIONS","Ждать pendingEnemyDeletions == 0")
action   ("FM_CHANGE_TURN",   "contextText.ChangeTurnText()")
action   ("FM_END_FIGHT",     "EndFight()")
final    ("FM_FINAL")

flow("FM_E01","FM_INIT",         "FM_START")
flow("FM_E02","FM_START",        "FM_SORT")
flow("FM_E03","FM_SORT",         "FM_LOOP_BEGIN")
flow("FM_E04","FM_LOOP_BEGIN",   "FM_DEC_ALIVE")
flow("FM_E05","FM_DEC_ALIVE",    "FM_DEC_MORE",    "[нет — пропустить]")
flow("FM_E06","FM_DEC_ALIVE",    "FM_PROCESS_FX",  "[да]")
flow("FM_E07","FM_PROCESS_FX",   "FM_DEC_DEAD_AFTER")
flow("FM_E08","FM_DEC_DEAD_AFTER","FM_DEC_MORE",   "[да — пропустить]")
flow("FM_E09","FM_DEC_DEAD_AFTER","FM_DEC_IS_ENEMY","[нет]")
flow("FM_E10","FM_DEC_IS_ENEMY", "FM_ENEMY_TURN",  "[да — Enemy]")
flow("FM_E11","FM_DEC_IS_ENEMY", "FM_CHAR_TURN",   "[нет — Character]")
flow("FM_E12","FM_ENEMY_TURN",   "FM_DEC_MORE")
flow("FM_E13","FM_CHAR_TURN",    "FM_DEC_MORE")
flow("FM_E14","FM_DEC_MORE",     "FM_LOOP_BEGIN",  "[да — следующий]")
flow("FM_E15","FM_DEC_MORE",     "FM_WAIT_DELETIONS","[нет — конец раунда]")
flow("FM_E16","FM_WAIT_DELETIONS","FM_CHANGE_TURN")
flow("FM_E17","FM_CHANGE_TURN",  "FM_END_FIGHT")
flow("FM_E18","FM_END_FIGHT",    "FM_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 3: Character Turn — ход персонажа (выбор и применение способности)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_CT", "Character Turn")

initial  ("CT_INIT")
action   ("CT_SET_TURN",     "character.IsTurn = true\nActiveCharacter = character")
action   ("CT_SETUP_ABIL",   "AbilityManager.SetupAbilitiesForCharacter(character)\nАнимация панели способностей")
action   ("CT_BLINK",        "StartEnemyBlinking()")
action   ("CT_WAIT",         "Ждать пока character.IsTurn == true\n(игрок выбирает действие)")
action   ("CT_ABILITY_CLICK","Игрок нажимает кнопку способности\nAbilityManager.OnAbilityButtonClicked(ability)")
decision ("CT_DEC_CAN_USE",  "ability.CanUse(character)?")
action   ("CT_WARN",         "Игнорировать (недостаточно маны / не доступна)")
action   ("CT_SET_PENDING",  "ActionButtons.SetPendingAbility(ability, character)")
decision ("CT_DEC_TARGET",   "ability.targetType?")
action   ("CT_SELF",         "ExecuteAbilityOnTarget(character)")
action   ("CT_SELECT_ENEMY", "Ждать выбора врага\nOnEnemySelected(enemy)")
action   ("CT_EXEC_ENEMY",   "ExecuteAbilityOnTarget(enemy)")
action   ("CT_SELECT_ALLY",  "StartCharacterSelection()\nОжидать OnCharacterSelected(ally)")
action   ("CT_EXEC_ALLY",    "ExecuteAbilityOnTarget(ally)")
action   ("CT_ALL_ENEMIES",  "ExecuteAbilityOnAllEnemies()\nДля каждого врага: ability.Execute + DeleteEnemyOnList")
action   ("CT_ALL_ALLIES",   "ExecuteAbilityOnAllAllies()\nДля каждого союзника: ability.Execute")
action   ("CT_END_TURN",     "character.UpdateUI()\ncharacter.IsTurn = false")
action   ("CT_STOP_BLINK",   "StopEnemyBlinking()\nActiveCharacter = null")
final    ("CT_FINAL")

flow("CT_E01","CT_INIT",        "CT_SET_TURN")
flow("CT_E02","CT_SET_TURN",    "CT_SETUP_ABIL")
flow("CT_E03","CT_SETUP_ABIL",  "CT_BLINK")
flow("CT_E04","CT_BLINK",       "CT_WAIT")
flow("CT_E05","CT_WAIT",        "CT_ABILITY_CLICK")
flow("CT_E06","CT_ABILITY_CLICK","CT_DEC_CAN_USE")
flow("CT_E07","CT_DEC_CAN_USE", "CT_WARN",        "[нет]")
flow("CT_E08","CT_WARN",        "CT_WAIT")
flow("CT_E09","CT_DEC_CAN_USE", "CT_SET_PENDING", "[да]")
flow("CT_E10","CT_SET_PENDING", "CT_DEC_TARGET")
flow("CT_E11","CT_DEC_TARGET",  "CT_SELF",        "[Self]")
flow("CT_E12","CT_DEC_TARGET",  "CT_SELECT_ENEMY","[Enemy]")
flow("CT_E13","CT_DEC_TARGET",  "CT_SELECT_ALLY", "[Ally]")
flow("CT_E14","CT_DEC_TARGET",  "CT_ALL_ENEMIES", "[AllEnemies]")
flow("CT_E15","CT_DEC_TARGET",  "CT_ALL_ALLIES",  "[AllAllies]")
flow("CT_E16","CT_SELECT_ENEMY","CT_EXEC_ENEMY")
flow("CT_E17","CT_SELF",        "CT_END_TURN")
flow("CT_E18","CT_EXEC_ENEMY",  "CT_END_TURN")
flow("CT_E19","CT_SELECT_ALLY", "CT_EXEC_ALLY")
flow("CT_E20","CT_EXEC_ALLY",   "CT_END_TURN")
flow("CT_E21","CT_ALL_ENEMIES", "CT_END_TURN")
flow("CT_E22","CT_ALL_ALLIES",  "CT_END_TURN")
flow("CT_E23","CT_END_TURN",    "CT_STOP_BLINK")
flow("CT_E24","CT_STOP_BLINK",  "CT_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 4: End Fight — завершение боя и синхронизация
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_EF", "End Fight")

initial  ("EF_INIT")
action   ("EF_CHECK",        "EndFight(): проверить состояние сторон")
decision ("EF_DEC_WIN",      "Все враги мертвы AND все персонажи живы?")
decision ("EF_DEC_LOSE",     "Все персонажи мертвы AND все враги живы?")
decision ("EF_DEC_CONTINUE", "Обе стороны живы?")
action   ("EF_WIN_XP",       "Для каждого base:\nbase.GetXP(allEnemyXP / characterStartCount)")
action   ("EF_WIN_RESULT",   "Player.Result = FightResult.Win")
action   ("EF_LOSE_XP",      "Для каждого base:\nbase.GetXP(allCharacterXP / enemiesStartCount)")
action   ("EF_LOSE_RESULT",  "Player.Result = FightResult.Lose")
action   ("EF_SYNC",         "BattleTeamSync.SyncTeamAfterBattle()\nСохранить данные команды в файл")
action   ("EF_BATTLE_OFF",   "EquipmentManager.SetBattleState(false)")
action   ("EF_WAIT",         "WaitForSecondsRealtime(1f)")
action   ("EF_LOAD_SCENE",   "GlobalLoader.LoadToScene()")
action   ("EF_NEXT_ROUND",   "StartFight() — следующий раунд")
final    ("EF_FINAL")

flow("EF_E01","EF_INIT",       "EF_CHECK")
flow("EF_E02","EF_CHECK",      "EF_DEC_WIN")
flow("EF_E03","EF_DEC_WIN",    "EF_WIN_XP",      "[да — победа]")
flow("EF_E04","EF_DEC_WIN",    "EF_DEC_LOSE",    "[нет]")
flow("EF_E05","EF_DEC_LOSE",   "EF_LOSE_XP",     "[да — поражение]")
flow("EF_E06","EF_DEC_LOSE",   "EF_DEC_CONTINUE","[нет]")
flow("EF_E07","EF_DEC_CONTINUE","EF_NEXT_ROUND", "[да — продолжить]")
flow("EF_E08","EF_DEC_CONTINUE","EF_FINAL",      "[нет — неопределённый исход]")
flow("EF_E09","EF_WIN_XP",     "EF_WIN_RESULT")
flow("EF_E10","EF_WIN_RESULT", "EF_SYNC")
flow("EF_E11","EF_LOSE_XP",    "EF_LOSE_RESULT")
flow("EF_E12","EF_LOSE_RESULT","EF_SYNC")
flow("EF_E13","EF_SYNC",       "EF_BATTLE_OFF")
flow("EF_E14","EF_BATTLE_OFF", "EF_WAIT")
flow("EF_E15","EF_WAIT",       "EF_LOAD_SCENE")
flow("EF_E16","EF_LOAD_SCENE", "EF_FINAL")
flow("EF_E17","EF_NEXT_ROUND", "EF_FINAL")

end_activity()

end_pkg()

lines.append('</uml:Model>')

# ── write & validate ──────────────────────────────────────────────────────────
content = "\n".join(lines)
out = "EchoRift_FightSystem_Activity.xml"
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
