#!/usr/bin/env python3
"""Generate EMX/UML2 XMI for Inventory + Shop module"""
import uuid

def uid(n):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.inv." + n)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="InventorySystem">')
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
# PACKAGE: Data
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Data", "Data")

# ItemData (ScriptableObject)
cls("ItemData", "ItemData")
attr("ItemData", "icon", "Sprite", "public")
attr("ItemData", "itemName", "string", "public")
attr("ItemData", "itemGameName", "string", "public")
attr("ItemData", "itemType", "ItemType", "public")
attr("ItemData", "itemPrice", "int", "public")
attr("ItemData", "description", "string", "public")
attr("ItemData", "isStackable", "bool", "public")
attr("ItemData", "maxStackSize", "int", "public")
attr("ItemData", "healthRestore", "int", "public")
attr("ItemData", "manaRestore", "int", "public")
attr("ItemData", "bonusDamage", "int", "public")
attr("ItemData", "bonusMagicDamage", "int", "public")
attr("ItemData", "bonusArmor", "int", "public")
attr("ItemData", "bonusMaxHealth", "int", "public")
attr("ItemData", "bonusMaxMana", "int", "public")
attr("ItemData", "bonusHeal", "int", "public")
attr("ItemData", "bonusLucky", "int", "public")
op("ItemData", "IsEquipable", "bool")
end_cls()

# InventoryData (ScriptableObject)
cls("InventoryData", "InventoryData")
attr("InventoryData", "inventorySlots", "List[InventorySlotData]", "private")
attr("InventoryData", "equipmentSlots", "List[InventorySlotData]", "private")
op("InventoryData", "Initialize")
op("InventoryData", "SetInventorySlot")
op("InventoryData", "SetEquipmentSlot")
op("InventoryData", "ClearInventorySlot")
op("InventoryData", "ClearEquipmentSlot")
op("InventoryData", "FindItem", "int")
op("InventoryData", "HasItem", "bool")
op("InventoryData", "GetItemCount", "int")
op("InventoryData", "FindEmptyInventorySlot", "int")
op("InventoryData", "FindStackableSlot", "int")
op("InventoryData", "CreateSaveData", "InventorySaver")
op("InventoryData", "LoadFromSaveData")
op("InventoryData", "Clear")
end_cls()
assoc("InventoryData", "InventorySlotData", "inventorySlots", "composite", "0..*")

# InventorySlotData (Serializable)
cls("InventorySlotData", "InventorySlotData")
attr("InventorySlotData", "itemName", "string", "public")
attr("InventorySlotData", "count", "int", "public")
end_cls()

# InventorySaver (Serializable)
cls("InventorySaver", "InventorySaver")
attr("InventorySaver", "inventorySlots", "InventorySlotData[]", "public")
attr("InventorySaver", "equipmentSlots", "InventorySlotData[]", "public")
attr("InventorySaver", "coins", "int", "public")
end_cls()
assoc("InventorySaver", "InventorySlotData", "inventorySlots", "composite", "0..*")

# Enums
enum("ItemType", "ItemType", ["None","Subject","Food","Potion","Weapon","Armor","Amulet","Helmet"])

end_pkg()  # Data

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Inventory
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Inventory", "Inventory")

# InventoryManager
cls("InventoryManager", "InventoryManager")
attr("InventoryManager", "inventoryData", "InventoryData", "private")
attr("InventoryManager", "inventorySlots", "InventorySlot[]", "public")
attr("InventoryManager", "equipmentSlots", "InventorySlot[]", "public")
attr("InventoryManager", "playerWallet", "PlayerWallet", "private")
attr("InventoryManager", "inventoryItemPrefab", "GameObject", "public")
op("InventoryManager", "AddItem", "bool")
op("InventoryManager", "RemoveItem", "bool")
op("InventoryManager", "GetItemCount", "int")
op("InventoryManager", "HasItem", "bool")
op("InventoryManager", "FindItem", "int")
op("InventoryManager", "IsInventoryFull", "bool")
op("InventoryManager", "CanAddItem", "bool")
op("InventoryManager", "HasSpaceForItem", "bool")
op("InventoryManager", "RemoveItemFromSlot", "bool")
op("InventoryManager", "SyncFromUI")
op("InventoryManager", "RefreshUIFromData")
op("InventoryManager", "SaveInventory")
op("InventoryManager", "LoadInventory")
op("InventoryManager", "FindItemDataByName", "ItemData")
op("InventoryManager", "ResetForNewGame")
end_cls()
assoc("InventoryManager", "InventoryData", "inventoryData", "composite")
assoc("InventoryManager", "InventorySlot", "inventorySlots", "composite", "0..*")
assoc("InventoryManager", "InventorySlot", "equipmentSlots", "composite", "0..*")
assoc("InventoryManager", "PlayerWallet", "playerWallet", "composite")
dep("InventoryManager", "EquipmentManager", "uses")
dep("InventoryManager", "InventorySaver", "creates")

# InventorySlot
cls("InventorySlot", "InventorySlot")
attr("InventorySlot", "allowedType", "ItemType", "public")
attr("InventorySlot", "inventoryManager", "InventoryManager", "private")
attr("InventorySlot", "descriptionDisplay", "ItemDescriptionDisplay", "private")
attr("InventorySlot", "equipSoundName", "string", "private")
attr("InventorySlot", "audioManager", "IAudioManager", "private")
op("InventorySlot", "OnDrop")
op("InventorySlot", "IsTypeAllowed", "bool")
op("InventorySlot", "OnPointerEnter")
op("InventorySlot", "OnPointerExit")
end_cls()
assoc("InventorySlot", "InventoryManager", "inventoryManager")
assoc("InventorySlot", "ItemDescriptionDisplay", "descriptionDisplay")
dep("InventorySlot", "DraggableItem", "uses")

# DraggableItem
cls("DraggableItem", "DraggableItem")
attr("DraggableItem", "image", "SVGImage", "public")
attr("DraggableItem", "countText", "TextMeshProUGUI", "public")
attr("DraggableItem", "parentAfterDrag", "Transform", "public")
attr("DraggableItem", "itemData", "ItemData", "public")
attr("DraggableItem", "count", "int", "public")
attr("DraggableItem", "inventoryManager", "InventoryManager", "private")
attr("DraggableItem", "contextMenu", "InventoryContextMenu", "private")
op("DraggableItem", "InitialiseItem")
op("DraggableItem", "RefreshCount")
op("DraggableItem", "OnBeginDrag")
op("DraggableItem", "OnDrag")
op("DraggableItem", "OnEndDrag")
op("DraggableItem", "OnPointerClick")
end_cls()
assoc("DraggableItem", "ItemData", "itemData")
assoc("DraggableItem", "InventoryManager", "inventoryManager")
assoc("DraggableItem", "InventoryContextMenu", "contextMenu")

# PlayerWallet
cls("PlayerWallet", "PlayerWallet")
attr("PlayerWallet", "coins", "int", "private")
attr("PlayerWallet", "OnCoinsChanged", "Action[int]", "public")
op("PlayerWallet", "HasEnoughCoins", "bool")
op("PlayerWallet", "TrySpendCoins", "bool")
op("PlayerWallet", "AddCoins")
op("PlayerWallet", "SetCoins")
end_cls()

# EquipmentManager (Singleton)
cls("EquipmentManager", "EquipmentManager")
attr("EquipmentManager", "Instance", "EquipmentManager", "public")
attr("EquipmentManager", "IsInBattle", "bool", "public")
attr("EquipmentManager", "appliedDamage", "int", "private")
attr("EquipmentManager", "appliedArmor", "int", "private")
attr("EquipmentManager", "appliedMaxHealth", "int", "private")
op("EquipmentManager", "SetBattleState")
op("EquipmentManager", "RecalculateEquipmentBonuses")
op("EquipmentManager", "GetBaseStats")
op("EquipmentManager", "RemoveAppliedBonuses", vis="private")
end_cls()
dep("EquipmentManager", "InventoryManager", "uses")
dep("EquipmentManager", "GlobalLoader", "uses")

# InventoryContextMenu
cls("InventoryContextMenu", "InventoryContextMenu")
attr("InventoryContextMenu", "contextMenuPanel", "GameObject", "private")
attr("InventoryContextMenu", "buttonPrefab", "GameObject", "private")
attr("InventoryContextMenu", "inventoryManager", "InventoryManager", "private")
attr("InventoryContextMenu", "shopUI", "ShopUI", "private")
attr("InventoryContextMenu", "currentItem", "DraggableItem", "private")
attr("InventoryContextMenu", "activeButtons", "List[GameObject]", "private")
op("InventoryContextMenu", "Show")
op("InventoryContextMenu", "Hide")
op("InventoryContextMenu", "CreateButton", vis="private")
op("InventoryContextMenu", "UseItem", vis="private")
op("InventoryContextMenu", "EquipItem", vis="private")
op("InventoryContextMenu", "DropItem", vis="private")
op("InventoryContextMenu", "BuyItem", vis="private")
op("InventoryContextMenu", "SellItem", vis="private")
end_cls()
assoc("InventoryContextMenu", "InventoryManager", "inventoryManager")
assoc("InventoryContextMenu", "DraggableItem", "currentItem")
dep("InventoryContextMenu", "ShopUI", "uses")
dep("InventoryContextMenu", "TeamMember", "uses")

# ContextMenuButton
cls("ContextMenuButton", "ContextMenuButton")
attr("ContextMenuButton", "buttonText", "TextMeshProUGUI", "private")
attr("ContextMenuButton", "button", "Button", "private")
attr("ContextMenuButton", "onClickAction", "Action", "private")
op("ContextMenuButton", "Initialize")
end_cls()

# ItemDescriptionDisplay
cls("ItemDescriptionDisplay", "ItemDescriptionDisplay")
attr("ItemDescriptionDisplay", "descriptionText", "TextMeshProUGUI", "private")
op("ItemDescriptionDisplay", "ShowItem")
op("ItemDescriptionDisplay", "Hide")
end_cls()
dep("ItemDescriptionDisplay", "DraggableItem", "uses")

# CoinDisplay
cls("CoinDisplay", "CoinDisplay")
attr("CoinDisplay", "playerWallet", "PlayerWallet", "private")
attr("CoinDisplay", "prefix", "string", "private")
attr("CoinDisplay", "coinText", "TextMeshProUGUI", "private")
op("CoinDisplay", "UpdateDisplay", vis="private")
end_cls()
assoc("CoinDisplay", "PlayerWallet", "playerWallet")

# TeamManager
cls("TeamManager", "TeamManager")
attr("TeamManager", "teamMemberPrefab", "GameObject", "private")
attr("TeamManager", "teamMembersContainer", "Transform", "private")
attr("TeamManager", "team", "Team", "private")
attr("TeamManager", "teamMembers", "List[TeamMember]", "private")
op("TeamManager", "InitializeTeam")
op("TeamManager", "UpdateTeamUI")
op("TeamManager", "LinkBattleCharacters")
op("TeamManager", "SyncFromBattle")
op("TeamManager", "AddTeamMember")
op("TeamManager", "SaveTeam")
end_cls()
assoc("TeamManager", "TeamMember", "teamMembers", "composite", "0..*")
dep("TeamManager", "GlobalLoader", "uses")
dep("TeamManager", "BattleTeamSync", "notifies")

# TeamMember
cls("TeamMember", "TeamMember")
attr("TeamMember", "xpBar", "Slider", "private")
attr("TeamMember", "healthBar", "Slider", "private")
attr("TeamMember", "nameText", "TextMeshProUGUI", "private")
attr("TeamMember", "characterIcon", "Image", "private")
attr("TeamMember", "character", "Character", "private")
attr("TeamMember", "settings", "CharactersSettings", "private")
attr("TeamMember", "isInBattle", "bool", "private")
op("TeamMember", "Initialize")
op("TeamMember", "SetCharacter")
op("TeamMember", "UpdateUI")
op("TeamMember", "SyncFromBattle")
op("TeamMember", "CanUseItemPublic", "bool")
op("TeamMember", "UseItemPublic")
op("TeamMember", "OnDrop")
op("TeamMember", "OnPointerEnter")
op("TeamMember", "OnPointerExit")
end_cls()
assoc("TeamMember", "CharactersSettings", "settings")
dep("TeamMember", "Character", "subscribes")
dep("TeamMember", "InventoryManager", "uses")

# CollectTrigger
cls("CollectTrigger", "CollectTrigger")
attr("CollectTrigger", "eventQueue", "List[CollectEvent]", "private")
attr("CollectTrigger", "currentStepIndex", "int", "private")
attr("CollectTrigger", "playerInside", "bool", "private")
attr("CollectTrigger", "mainUI", "MainUI", "private")
attr("CollectTrigger", "gameSettings", "GameSettings", "private")
op("CollectTrigger", "TryExecuteCurrentEvent", vis="private")
op("CollectTrigger", "FinishCurrentStep", vis="private")
op("CollectTrigger", "CanExecute", "bool", vis="private")
op("CollectTrigger", "SaveState", vis="private")
op("CollectTrigger", "LoadState", vis="private")
end_cls()
dep("CollectTrigger", "InventoryManager", "uses")
dep("CollectTrigger", "SaveLoadSystem", "uses")

# CollectEvent (nested Serializable)
cls("CollectEvent", "CollectEvent")
attr("CollectEvent", "questCode", "string", "public")
attr("CollectEvent", "itemNameToCollect", "string", "public")
attr("CollectEvent", "itemNameToHas", "string", "public")
attr("CollectEvent", "removeItemAfterStep", "bool", "public")
attr("CollectEvent", "isRepeatable", "bool", "public")
attr("CollectEvent", "isMinigame", "bool", "public")
attr("CollectEvent", "isDone", "bool", "public")
end_cls()
assoc("CollectTrigger", "CollectEvent", "eventQueue", "composite", "0..*")

end_pkg()  # Inventory

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Shop
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Shop", "Shop")

# IShopkeeper (interface)
cls("IShopkeeper", "IShopkeeper", iface=True)
attr("IShopkeeper", "ShopData", "ShopData", "public")
op("IShopkeeper", "OpenShop", abstract=True)
op("IShopkeeper", "CloseShop", abstract=True)
end_cls()

# ShopData (ScriptableObject)
cls("ShopData", "ShopData")
attr("ShopData", "shopName", "string", "public")
attr("ShopData", "shopDescription", "string", "public")
attr("ShopData", "items", "List[ShopItem]", "public")
attr("ShopData", "acceptsPlayerItems", "bool", "public")
attr("ShopData", "buybackPercentage", "int", "public")
op("ShopData", "FindShopItem", "ShopItem")
op("ShopData", "HasItemInStock", "bool")
op("ShopData", "GetSellPriceForItem", "int")
end_cls()
assoc("ShopData", "ShopItem", "items", "composite", "0..*")

# ShopItem (Serializable)
cls("ShopItem", "ShopItem")
attr("ShopItem", "item", "ItemData", "public")
attr("ShopItem", "quantity", "int", "public")
attr("ShopItem", "buyPrice", "int", "public")
attr("ShopItem", "sellPrice", "int", "public")
attr("ShopItem", "infiniteStock", "bool", "public")
op("ShopItem", "GetBuyPrice", "int")
op("ShopItem", "GetSellPrice", "int")
end_cls()
assoc("ShopItem", "ItemData", "item")

# ShopManager
cls("ShopManager", "ShopManager")
attr("ShopManager", "playerInventory", "InventoryManager", "private")
attr("ShopManager", "playerWallet", "PlayerWallet", "private")
attr("ShopManager", "currentShop", "ShopData", "private")
attr("ShopManager", "IsShopOpen", "bool", "public")
attr("ShopManager", "OnShopOpened", "Action[ShopData]", "public")
attr("ShopManager", "OnShopClosed", "Action", "public")
attr("ShopManager", "OnItemBought", "Action[ItemData,int,int]", "public")
attr("ShopManager", "OnItemSold", "Action[ItemData,int,int]", "public")
op("ShopManager", "Initialize")
op("ShopManager", "OpenShop")
op("ShopManager", "CloseShop")
op("ShopManager", "BuyItem", "bool")
op("ShopManager", "SellItem", "bool")
op("ShopManager", "GetBuyPrice", "int")
op("ShopManager", "GetSellPrice", "int")
op("ShopManager", "ClearEventSubscriptions")
end_cls()
assoc("ShopManager", "InventoryManager", "playerInventory")
assoc("ShopManager", "PlayerWallet", "playerWallet")
assoc("ShopManager", "ShopData", "currentShop")

# ShopUI
cls("ShopUI", "ShopUI")
attr("ShopUI", "shopPanel", "GameObject", "private")
attr("ShopUI", "merchantSlots", "InventorySlot[]", "private")
attr("ShopUI", "playerSlots", "InventorySlot[]", "private")
attr("ShopUI", "inventoryManager", "InventoryManager", "private")
attr("ShopUI", "playerWallet", "PlayerWallet", "private")
attr("ShopUI", "contextMenu", "InventoryContextMenu", "private")
attr("ShopUI", "shopManager", "ShopManager", "private")
attr("ShopUI", "IsShopMode", "bool", "public")
op("ShopUI", "OpenShop")
op("ShopUI", "CloseShop")
op("ShopUI", "OnItemTransactionComplete")
op("ShopUI", "IsMerchantSlot", "bool")
op("ShopUI", "IsPlayerShopSlot", "bool")
op("ShopUI", "LoadMerchantInventory", vis="private")
op("ShopUI", "SyncPlayerInventory", vis="private")
end_cls()
assoc("ShopUI", "ShopManager", "shopManager", "composite")
assoc("ShopUI", "InventoryManager", "inventoryManager")
assoc("ShopUI", "PlayerWallet", "playerWallet")
assoc("ShopUI", "InventoryContextMenu", "contextMenu")
assoc("ShopUI", "InventorySlot", "merchantSlots", "none", "0..*")
assoc("ShopUI", "InventorySlot", "playerSlots", "none", "0..*")

# ShopInventorySlot
cls("ShopInventorySlot", "ShopInventorySlot")
attr("ShopInventorySlot", "contextMenu", "InventoryContextMenu", "private")
attr("ShopInventorySlot", "descriptionDisplay", "ItemDescriptionDisplay", "private")
op("ShopInventorySlot", "OnPointerClick")
op("ShopInventorySlot", "OnPointerEnter")
op("ShopInventorySlot", "OnPointerExit")
end_cls()
assoc("ShopInventorySlot", "InventoryContextMenu", "contextMenu")
assoc("ShopInventorySlot", "ItemDescriptionDisplay", "descriptionDisplay")

# MerchantNPC
cls("MerchantNPC", "MerchantNPC")
attr("MerchantNPC", "shopData", "ShopData", "private")
attr("MerchantNPC", "dialogueTrigger", "DialogueSystemTrigger", "private")
attr("MerchantNPC", "shopUI", "ShopUI", "private")
op("MerchantNPC", "OpenShop")
op("MerchantNPC", "CloseShop")
end_cls()
realization("MerchantNPC", "IShopkeeper")
assoc("MerchantNPC", "ShopData", "shopData")
assoc("MerchantNPC", "ShopUI", "shopUI")

end_pkg()  # Shop

# ── write deferred relationships ──────────────────────────────────────────────
for rel in deferred:
    lines.append(rel)

lines.append('</uml:Model>')

content = "\n".join(lines)
with open("EchoRift_Inventory.xml", "w", encoding="utf-8") as f:
    f.write(content)

import xml.etree.ElementTree as ET
try:
    ET.parse("EchoRift_Inventory.xml")
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
