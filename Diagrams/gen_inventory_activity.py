#!/usr/bin/env python3
"""Generate UML2 XMI Activity Diagrams for Inventory module (EA-compatible)"""
import uuid

def uid(n):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.inv.act." + n)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="Inventory_Activity">')
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
begin_pkg("PKG_INV_ACT", "Inventory_Activity")

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 1: Inventory Initialization & Load
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_INIT", "Inventory Initialization and Load")

initial  ("II_INIT")
action   ("II_START",       "Start: InitializeData()\nинициализировать InventoryData\n(inventorySlots.Length, equipmentSlots.Length)")
decision ("II_DEC_DATA",    "inventoryData назначен?")
action   ("II_ERROR",       "LogError: InventoryData не назначен")
action   ("II_LOAD",        "LoadInventory()")
decision ("II_DEC_SAVE",    "SaveLoadSystem.Exists(INVENTORY_DATA)?")
action   ("II_NO_SAVE",     "Log: сохранение не найдено")
action   ("II_LOAD_DATA",   "SaveLoadSystem.Load(INVENTORY_DATA) -> InventorySaver")
decision ("II_DEC_NULL",    "saver == null?")
action   ("II_WARN_NULL",   "LogWarning: не удалось загрузить данные")
action   ("II_APPLY",       "inventoryData.LoadFromSaveData(saver)\nLoadCoins(saver.coins)")
action   ("II_REFRESH_UI",  "RefreshUI():\nClearAllSlots()\nSpawnItemInSlot для каждого слота")
action   ("II_EQUIP_BONUS", "EquipmentManager.RecalculateEquipmentBonuses(equipmentSlots, this)")
final    ("II_FINAL")

flow("II_E01","II_INIT",      "II_START")
flow("II_E02","II_START",     "II_DEC_DATA")
flow("II_E03","II_DEC_DATA",  "II_ERROR",      "[нет]")
flow("II_E04","II_ERROR",     "II_FINAL")
flow("II_E05","II_DEC_DATA",  "II_LOAD",       "[да]")
flow("II_E06","II_LOAD",      "II_DEC_SAVE")
flow("II_E07","II_DEC_SAVE",  "II_NO_SAVE",    "[нет]")
flow("II_E08","II_NO_SAVE",   "II_FINAL")
flow("II_E09","II_DEC_SAVE",  "II_LOAD_DATA",  "[да]")
flow("II_E10","II_LOAD_DATA", "II_DEC_NULL")
flow("II_E11","II_DEC_NULL",  "II_WARN_NULL",  "[да]")
flow("II_E12","II_WARN_NULL", "II_FINAL")
flow("II_E13","II_DEC_NULL",  "II_APPLY",      "[нет]")
flow("II_E14","II_APPLY",     "II_REFRESH_UI")
flow("II_E15","II_REFRESH_UI","II_EQUIP_BONUS")
flow("II_E16","II_EQUIP_BONUS","II_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 2: Add Item to Inventory
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_ADD", "Add Item to Inventory")

initial  ("AI_INIT")
action   ("AI_FIND",        "FindItemDataByName(itemName)\n-> Resources.LoadAll(Items)")
decision ("AI_DEC_FOUND",   "ItemData найден?")
action   ("AI_WARN",        "LogWarning: предмет не найден")
decision ("AI_DEC_STACK",   "item.isStackable?")
action   ("AI_ADD_STACK",   "AddToExistingStacks(item, amount):\nнайти слоты с тем же предметом\nдобавить до maxStackSize")
decision ("AI_DEC_REMAIN",  "remaining > 0?")
action   ("AI_ADD_EMPTY",   "AddToEmptySlots(item, amount):\nнайти пустые слоты\nSpawnItemInSlot + SetInventorySlot")
decision ("AI_DEC_FULL",    "remaining > 0 после добавления?")
action   ("AI_FULL",        "Log: Инвентарь полон!")
action   ("AI_SUCCESS",     "return true")
final    ("AI_FINAL")

flow("AI_E01","AI_INIT",      "AI_FIND")
flow("AI_E02","AI_FIND",      "AI_DEC_FOUND")
flow("AI_E03","AI_DEC_FOUND", "AI_WARN",       "[нет]")
flow("AI_E04","AI_WARN",      "AI_FINAL")
flow("AI_E05","AI_DEC_FOUND", "AI_DEC_STACK",  "[да]")
flow("AI_E06","AI_DEC_STACK", "AI_ADD_STACK",  "[да — стакуемый]")
flow("AI_E07","AI_ADD_STACK", "AI_DEC_REMAIN")
flow("AI_E08","AI_DEC_REMAIN","AI_SUCCESS",    "[нет — всё добавлено]")
flow("AI_E09","AI_DEC_REMAIN","AI_ADD_EMPTY",  "[да — остаток]")
flow("AI_E10","AI_DEC_STACK", "AI_ADD_EMPTY",  "[нет — не стакуемый]")
flow("AI_E11","AI_ADD_EMPTY", "AI_DEC_FULL")
flow("AI_E12","AI_DEC_FULL",  "AI_FULL",       "[да]")
flow("AI_E13","AI_DEC_FULL",  "AI_SUCCESS",    "[нет]")
flow("AI_E14","AI_FULL",      "AI_FINAL")
flow("AI_E15","AI_SUCCESS",   "AI_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 3: Drag & Drop Item (DraggableItem)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_DRAG", "Drag and Drop Item")

initial  ("DD_INIT")
action   ("DD_BEGIN",       "OnBeginDrag:\nпроверить кнопку мыши (Left)\nпроверить бой (FightManager)")
decision ("DD_DEC_BLOCK",   "dragBlocked?")
action   ("DD_HIDE_CTX",    "contextMenu.Hide()\nparentAfterDrag = transform.parent\nперенести на Canvas (SetAsLastSibling)\nimage.raycastTarget = false")
action   ("DD_DRAG",        "OnDrag:\ntransform.position = Input.mousePosition")
action   ("DD_END",         "OnEndDrag:\nimage.raycastTarget = true")
decision ("DD_DEC_PARENT",  "transform.parent == Canvas?")
action   ("DD_SNAP",        "CheckForNearbySlot:\nнайти ближайший слот (sqrMagnitude)\nесли dist <= snapDistance AND IsTypeAllowed\n-> slot.OnDrop(eventData)")
action   ("DD_RETURN",      "transform.SetParent(parentAfterDrag)\ntransform.localPosition = Vector3.zero")
action   ("DD_SYNC",        "inventoryManager.SyncFromUI()")
final    ("DD_FINAL")

flow("DD_E01","DD_INIT",      "DD_BEGIN")
flow("DD_E02","DD_BEGIN",     "DD_DEC_BLOCK")
flow("DD_E03","DD_DEC_BLOCK", "DD_FINAL",      "[да — заблокировано]")
flow("DD_E04","DD_DEC_BLOCK", "DD_HIDE_CTX",   "[нет]")
flow("DD_E05","DD_HIDE_CTX",  "DD_DRAG")
flow("DD_E06","DD_DRAG",      "DD_END")
flow("DD_E07","DD_END",       "DD_DEC_PARENT")
flow("DD_E08","DD_DEC_PARENT","DD_SNAP",        "[да — не попал в слот]")
flow("DD_E09","DD_DEC_PARENT","DD_RETURN",      "[нет — уже в слоте]")
flow("DD_E10","DD_SNAP",      "DD_RETURN")
flow("DD_E11","DD_RETURN",    "DD_SYNC")
flow("DD_E12","DD_SYNC",      "DD_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 4: Drop on Slot (InventorySlot.OnDrop)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_DROP", "Drop Item on Slot")

initial  ("DS_INIT")
action   ("DS_DROP",        "OnDrop(eventData):\nполучить DraggableItem из eventData.pointerDrag")
decision ("DS_DEC_EQUIP",   "IsEquipmentSlot() AND FightManager активен?")
action   ("DS_BLOCK",       "Log: нельзя менять экипировку во время боя")
decision ("DS_DEC_TYPE",    "IsTypeAllowed(draggableItem)?")
action   ("DS_IGNORE",      "return — тип не подходит")
decision ("DS_DEC_EMPTY",   "transform.childCount == 0?")
action   ("DS_PLACE",       "draggableItem.parentAfterDrag = transform\n[если слот экипировки] PlayEquipSound()")
decision ("DS_DEC_STACK",   "CanStackItems(draggableItem, currentItem)?")
action   ("DS_STACK",       "ProcessStackItems:\nсложить count\nесли total > maxStack — остаток в пустой слот")
action   ("DS_SWAP",        "SwapItems:\nпроверить IsTypeAllowed для обоих\nпоменять parentAfterDrag местами")
action   ("DS_SYNC",        "SyncAfterChange() -> Invoke(DoSync, 0.1f)\n-> inventoryManager.SyncFromUI()")
final    ("DS_FINAL")

flow("DS_E01","DS_INIT",      "DS_DROP")
flow("DS_E02","DS_DROP",      "DS_DEC_EQUIP")
flow("DS_E03","DS_DEC_EQUIP", "DS_BLOCK",      "[да — бой активен]")
flow("DS_E04","DS_BLOCK",     "DS_FINAL")
flow("DS_E05","DS_DEC_EQUIP", "DS_DEC_TYPE",   "[нет]")
flow("DS_E06","DS_DEC_TYPE",  "DS_IGNORE",     "[нет — тип запрещён]")
flow("DS_E07","DS_IGNORE",    "DS_FINAL")
flow("DS_E08","DS_DEC_TYPE",  "DS_DEC_EMPTY",  "[да]")
flow("DS_E09","DS_DEC_EMPTY", "DS_PLACE",      "[да — слот пуст]")
flow("DS_E10","DS_DEC_EMPTY", "DS_DEC_STACK",  "[нет — слот занят]")
flow("DS_E11","DS_DEC_STACK", "DS_STACK",      "[да — стакуемые]")
flow("DS_E12","DS_DEC_STACK", "DS_SWAP",       "[нет — разные предметы]")
flow("DS_E13","DS_PLACE",     "DS_SYNC")
flow("DS_E14","DS_STACK",     "DS_SYNC")
flow("DS_E15","DS_SWAP",      "DS_SYNC")
flow("DS_E16","DS_SYNC",      "DS_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 5: Context Menu Actions (Use / Equip / Drop / Buy / Sell)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_CTX", "Context Menu Actions")

initial  ("CM_INIT")
action   ("CM_SHOW",        "ПКМ по предмету -> Show(item, position):\nопределить режим (Shop / Normal)\nсоздать кнопки")
decision ("CM_DEC_SHOP",    "ShopUI.IsShopMode?")
decision ("CM_DEC_MERCHANT","IsMerchantSlot(slot)?")
action   ("CM_BUY_BTNS",    "CreateBuyButtons:\nКупить 1 / 5 / 10 / всё (цена)")
action   ("CM_SELL_BTNS",   "CreateSellButtons:\nПродать 1 / 5 / 10 / всё (цена)")
action   ("CM_NORMAL_BTNS", "CreateButtonsForItem:\nИспользовать (Food/Potion)\nЭкипировать (Weapon/Armor/Amulet/Helmet)\nВыбросить")
action   ("CM_SHOW_PANEL",  "contextMenuPanel.SetActive(true)\nClampMenuToScreen()")
action   ("CM_CLICK",       "Игрок нажимает кнопку")
decision ("CM_DEC_ACTION",  "Действие?")
action   ("CM_USE",         "UseItem:\nнайти TeamMember.CanUseItem\n-> member.UseItemPublic(item)")
action   ("CM_EQUIP",       "EquipItem:\nнайти подходящий equipSlot\nпереместить / поменять местами\nPlayEquipSound()")
action   ("CM_DROP",        "DropItem:\nRemoveItemFromSlot(slot, 1)")
action   ("CM_BUY",         "BuyItem(item, qty):\nShopManager.BuyItem\nOnItemTransactionComplete()")
action   ("CM_SELL",        "SellItem(item, qty):\nShopManager.SellItem\nOnItemTransactionComplete()")
action   ("CM_HIDE",        "Hide(): contextMenuPanel.SetActive(false)\nClearButtons()")
final    ("CM_FINAL")

flow("CM_E01","CM_INIT",       "CM_SHOW")
flow("CM_E02","CM_SHOW",       "CM_DEC_SHOP")
flow("CM_E03","CM_DEC_SHOP",   "CM_DEC_MERCHANT","[да — режим магазина]")
flow("CM_E04","CM_DEC_MERCHANT","CM_BUY_BTNS",   "[да — товар торговца]")
flow("CM_E05","CM_DEC_MERCHANT","CM_SELL_BTNS",  "[нет — предмет игрока]")
flow("CM_E06","CM_DEC_SHOP",   "CM_NORMAL_BTNS", "[нет — обычный режим]")
flow("CM_E07","CM_BUY_BTNS",   "CM_SHOW_PANEL")
flow("CM_E08","CM_SELL_BTNS",  "CM_SHOW_PANEL")
flow("CM_E09","CM_NORMAL_BTNS","CM_SHOW_PANEL")
flow("CM_E10","CM_SHOW_PANEL", "CM_CLICK")
flow("CM_E11","CM_CLICK",      "CM_DEC_ACTION")
flow("CM_E12","CM_DEC_ACTION", "CM_USE",         "[Использовать]")
flow("CM_E13","CM_DEC_ACTION", "CM_EQUIP",       "[Экипировать]")
flow("CM_E14","CM_DEC_ACTION", "CM_DROP",        "[Выбросить]")
flow("CM_E15","CM_DEC_ACTION", "CM_BUY",         "[Купить]")
flow("CM_E16","CM_DEC_ACTION", "CM_SELL",        "[Продать]")
flow("CM_E17","CM_USE",        "CM_HIDE")
flow("CM_E18","CM_EQUIP",      "CM_HIDE")
flow("CM_E19","CM_DROP",       "CM_HIDE")
flow("CM_E20","CM_BUY",        "CM_HIDE")
flow("CM_E21","CM_SELL",       "CM_HIDE")
flow("CM_E22","CM_HIDE",       "CM_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 6: Equipment Bonuses Recalculation (EquipmentManager)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_EQ", "Equipment Bonuses Recalculation")

initial  ("EB_INIT")
action   ("EB_GET_RUNTIME", "GetPlayerRuntime():\nGlobalLoader -> Team -> CharacterDataRuntime")
decision ("EB_DEC_RUNTIME", "runtime == null?")
action   ("EB_SKIP",        "return — нет данных игрока")
action   ("EB_SAVE_HP",     "baseHealth = runtime._health\nbaseMana = runtime._mana")
action   ("EB_REMOVE",      "RemoveAppliedBonuses(runtime):\nвычесть ранее применённые бонусы\nсбросить applied* = 0")
action   ("EB_CALC",        "Для каждого equipmentSlot:\nесли DraggableItem != null AND IsEquipable()\nсуммировать bonusDamage, bonusArmor,\nbonusMaxHealth, bonusMaxMana, bonusHeal, bonusLucky")
action   ("EB_APPLY",       "runtime += новые бонусы\nпересчитать _health / _mana\n(clamp к новым максимумам)")
action   ("EB_STORE",       "сохранить applied* = новые значения")
action   ("EB_UI",          "TeamManager.UpdateTeamUI()")
final    ("EB_FINAL")

flow("EB_E01","EB_INIT",       "EB_GET_RUNTIME")
flow("EB_E02","EB_GET_RUNTIME","EB_DEC_RUNTIME")
flow("EB_E03","EB_DEC_RUNTIME","EB_SKIP",        "[да]")
flow("EB_E04","EB_SKIP",       "EB_FINAL")
flow("EB_E05","EB_DEC_RUNTIME","EB_SAVE_HP",     "[нет]")
flow("EB_E06","EB_SAVE_HP",    "EB_REMOVE")
flow("EB_E07","EB_REMOVE",     "EB_CALC")
flow("EB_E08","EB_CALC",       "EB_APPLY")
flow("EB_E09","EB_APPLY",      "EB_STORE")
flow("EB_E10","EB_STORE",      "EB_UI")
flow("EB_E11","EB_UI",         "EB_FINAL")

end_activity()

end_pkg()

lines.append('</uml:Model>')

content = "\n".join(lines)
out = "EchoRift_Inventory_Activity.xml"
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
