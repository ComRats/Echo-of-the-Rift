#!/usr/bin/env python3
"""Generate UML2 XMI Component Diagram for Inventory module (EA-compatible)"""
import uuid

def uid(n):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.inv.comp." + n)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="Inventory_Component">')
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
pkg("PKG_INV_COMP", "Inventory_Component")

# ── Interfaces ────────────────────────────────────────────────────────────────
interface("IInventoryService", "IInventoryService",
    ["AddItem","RemoveItem","HasItem","GetItemCount",
     "IsInventoryFull","CanAddItem","SaveInventory","LoadInventory"])

interface("IDropHandler", "IDropHandler",
    ["OnDrop"])

interface("IEquipmentService", "IEquipmentService",
    ["RecalculateEquipmentBonuses","SetBattleState","GetBaseStats"])

interface("IShopService", "IShopService",
    ["BuyItem","SellItem","GetBuyPrice","GetSellPrice"])

# ── Component: ItemData ───────────────────────────────────────────────────────
component("COMP_ItemData", "ItemData")
end_component()

# ── Component: InventoryData ──────────────────────────────────────────────────
component("COMP_InventoryData", "InventoryData")
end_component()
dependency("COMP_InventoryData", "COMP_ItemData", "references")

# ── Component: InventorySaver ─────────────────────────────────────────────────
component("COMP_InventorySaver", "InventorySaver")
end_component()

# ── Component: InventoryManager ───────────────────────────────────────────────
component("COMP_InventoryManager", "InventoryManager")
provided("COMP_InventoryManager", "IInventoryService")
end_component()
dependency("COMP_InventoryManager", "COMP_InventoryData",    "owns")
dependency("COMP_InventoryManager", "COMP_InventorySlot",    "contains slots")
dependency("COMP_InventoryManager", "COMP_DraggableItem",    "spawns")
dependency("COMP_InventoryManager", "COMP_PlayerWallet",     "owns")
dependency("COMP_InventoryManager", "COMP_InventorySaver",   "save/load via")
dependency("COMP_InventoryManager", "COMP_SaveLoadSystem",   "persists to")
dependency("COMP_InventoryManager", "COMP_EquipmentManager", "notifies")

# ── Component: InventorySlot ──────────────────────────────────────────────────
component("COMP_InventorySlot", "InventorySlot")
provided("COMP_InventorySlot", "IDropHandler")
end_component()
usage("COMP_InventorySlot", "IInventoryService")
dependency("COMP_InventorySlot", "COMP_DraggableItem",          "hosts")
dependency("COMP_InventorySlot", "COMP_ItemDescriptionDisplay", "shows tooltip via")
dependency("COMP_InventorySlot", "COMP_IAudioManager",          "plays equip sound")

# ── Component: DraggableItem ──────────────────────────────────────────────────
component("COMP_DraggableItem", "DraggableItem")
end_component()
dependency("COMP_DraggableItem", "COMP_ItemData",            "displays")
usage("COMP_DraggableItem", "IInventoryService")
dependency("COMP_DraggableItem", "COMP_InventoryContextMenu","opens on RMB")

# ── Component: InventoryContextMenu ───────────────────────────────────────────
component("COMP_InventoryContextMenu", "InventoryContextMenu")
end_component()
usage("COMP_InventoryContextMenu", "IInventoryService")
usage("COMP_InventoryContextMenu", "IShopService")
dependency("COMP_InventoryContextMenu", "COMP_DraggableItem",  "acts on")
dependency("COMP_InventoryContextMenu", "COMP_TeamMember",     "UseItem via")
dependency("COMP_InventoryContextMenu", "COMP_IAudioManager",  "plays equip sound")

# ── Component: EquipmentManager ───────────────────────────────────────────────
component("COMP_EquipmentManager", "EquipmentManager")
provided("COMP_EquipmentManager", "IEquipmentService")
end_component()
dependency("COMP_EquipmentManager", "COMP_InventorySlot",    "reads equipment slots")
dependency("COMP_EquipmentManager", "COMP_ItemData",         "reads bonuses from")
dependency("COMP_EquipmentManager", "COMP_GlobalLoader",     "gets player runtime")
dependency("COMP_EquipmentManager", "COMP_TeamManager",      "updates UI via")

# ── Component: PlayerWallet ───────────────────────────────────────────────────
component("COMP_PlayerWallet", "PlayerWallet")
end_component()

# ── Component: CoinDisplay ────────────────────────────────────────────────────
component("COMP_CoinDisplay", "CoinDisplay")
end_component()
dependency("COMP_CoinDisplay", "COMP_PlayerWallet", "observes")

# ── Component: ItemDescriptionDisplay ────────────────────────────────────────
component("COMP_ItemDescriptionDisplay", "ItemDescriptionDisplay")
end_component()
dependency("COMP_ItemDescriptionDisplay", "COMP_ItemData", "displays")

# ── Component: TeamManager ────────────────────────────────────────────────────
component("COMP_TeamManager", "TeamManager")
end_component()
dependency("COMP_TeamManager", "COMP_TeamMember", "manages")

# ── Component: TeamMember ─────────────────────────────────────────────────────
component("COMP_TeamMember", "TeamMember")
end_component()
usage("COMP_TeamMember", "IInventoryService")

# ── Component: ShopManager ────────────────────────────────────────────────────
component("COMP_ShopManager", "ShopManager")
provided("COMP_ShopManager", "IShopService")
end_component()
usage("COMP_ShopManager", "IInventoryService")
dependency("COMP_ShopManager", "COMP_PlayerWallet",    "charges/credits")
dependency("COMP_ShopManager", "COMP_ItemData",        "prices from")

# ── Component: ShopUI ─────────────────────────────────────────────────────────
component("COMP_ShopUI", "ShopUI")
end_component()
usage("COMP_ShopUI", "IShopService")
dependency("COMP_ShopUI", "COMP_ShopManager",          "delegates to")
dependency("COMP_ShopUI", "COMP_InventoryContextMenu", "triggers")
dependency("COMP_ShopUI", "COMP_InventorySlot",        "uses player slots")

# ── External components ───────────────────────────────────────────────────────
component("COMP_SaveLoadSystem", "SaveLoadSystem")
end_component()

component("COMP_GlobalLoader", "GlobalLoader")
end_component()

component("COMP_IAudioManager", "IAudioManager")
end_component()

# ── write deferred ────────────────────────────────────────────────────────────
for rel in deferred:
    lines.append(rel)

end_pkg()
lines.append('</uml:Model>')

content = "\n".join(lines)
out = "EchoRift_Inventory_Component.xml"
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
