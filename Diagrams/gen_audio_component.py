#!/usr/bin/env python3
"""Generate UML2 XMI Component Diagram for Audio module (EA-compatible)"""
import uuid

def uid(n):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.audio.comp." + n)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="AudioSystem_Component">')
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

def provided(comp_key, iface_key, iface_name, indent="      "):
    """Provided interface — компонент реализует интерфейс"""
    rid = uid(f"PROV_{comp_key}_{iface_key}")
    lines.append(f'{indent}<interfaceRealization xmi:id="{rid}" contract="{uid(iface_key)}" implementingClassifier="{uid(comp_key)}"/>')

def required(comp_key, iface_key, indent="      "):
    """Required interface — компонент использует интерфейс (Usage dependency)"""
    deferred.append(
        f'    <packagedElement xmi:type="uml:Usage" xmi:id="{uid("USE_"+comp_key+"_"+iface_key)}"'
        f' client="{uid(comp_key)}" supplier="{uid(iface_key)}"/>'
    )

def interface(key, name, ops=None, indent="    "):
    lines.append(f'{indent}<packagedElement xmi:type="uml:Interface" xmi:id="{uid(key)}" name="{name}" visibility="public">')
    if ops:
        for op in ops:
            lines.append(f'{indent}  <ownedOperation xmi:id="{uid(key+"_op_"+op)}" name="{op}" visibility="public" isAbstract="true"/>')
    lines.append(f'{indent}</packagedElement>')

def dependency(src_key, tgt_key, label="uses"):
    deferred.append(
        f'    <packagedElement xmi:type="uml:Dependency" xmi:id="{uid("DEP_"+src_key+"_"+tgt_key)}"'
        f' name="{label}" client="{uid(src_key)}" supplier="{uid(tgt_key)}"/>'
    )

# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_AUDIO_COMP", "AudioSystem_Component")

# ── Interfaces ────────────────────────────────────────────────────────────────
interface("IAudioManager", "IAudioManager",
    ["Play","Stop","LerpVolume","LerpPitch","TryGetSource","PlayOneShot",
     "SetTypeVolume","GetTypeVolume","ChangeGroupValue"])

interface("IFluentAudioManager", "IFluentAudioManager",
    ["Execute","Play","Stop","LerpVolume","TryGetSource"])

interface("IAudioLogger", "IAudioLogger",
    ["Log","LogFormat","LogException","LogAssert"])

# ── Component: AudioCore ──────────────────────────────────────────────────────
component("COMP_AudioCore", "AudioCore")
provided("COMP_AudioCore", "IAudioManager", "IAudioManager")
end_component()

# ── Component: ServiceLocator ─────────────────────────────────────────────────
component("COMP_ServiceLocator", "ServiceLocator")
provided("COMP_ServiceLocator", "IAudioManager", "IAudioManager")
end_component()
dependency("COMP_ServiceLocator", "COMP_AudioCore", "registers")

# ── Component: LoggedAudioManager ────────────────────────────────────────────
component("COMP_LoggedAudioManager", "LoggedAudioManager")
provided("COMP_LoggedAudioManager", "IAudioManager", "IAudioManager")
end_component()
required("COMP_LoggedAudioManager", "IAudioManager")
required("COMP_LoggedAudioManager", "IAudioLogger")

# ── Component: FluentAudioManager ────────────────────────────────────────────
component("COMP_FluentAudioManager", "FluentAudioManager")
provided("COMP_FluentAudioManager", "IFluentAudioManager", "IFluentAudioManager")
end_component()
required("COMP_FluentAudioManager", "IAudioManager")

# ── Component: AudioLogger ────────────────────────────────────────────────────
component("COMP_AudioLogger", "AudioLogger")
provided("COMP_AudioLogger", "IAudioLogger", "IAudioLogger")
end_component()

# ── Component: UIAudioLogger ──────────────────────────────────────────────────
component("COMP_UIAudioLogger", "UIAudioLogger")
provided("COMP_UIAudioLogger", "IAudioLogger", "IAudioLogger")
end_component()

# ── Component: AudioManagerSettings (Provider) ───────────────────────────────
component("COMP_AudioManagerSettings", "AudioManagerSettings")
end_component()
dependency("COMP_AudioManagerSettings", "COMP_ServiceLocator", "registers into")
dependency("COMP_AudioManagerSettings", "COMP_AudioCore",      "creates")
dependency("COMP_AudioManagerSettings", "COMP_AudioLogger",    "creates")

# ── Component: MusicTransitionManager ────────────────────────────────────────
component("COMP_MusicTransitionManager", "MusicTransitionManager")
end_component()
required("COMP_MusicTransitionManager", "IAudioManager")
dependency("COMP_MusicTransitionManager", "COMP_ServiceLocator", "uses")

# ── Component: AreaAmbientSound ───────────────────────────────────────────────
component("COMP_AreaAmbientSound", "AreaAmbientSound")
end_component()
required("COMP_AreaAmbientSound", "IAudioManager")
dependency("COMP_AreaAmbientSound", "COMP_ServiceLocator",        "uses")
dependency("COMP_AreaAmbientSound", "COMP_MusicTransitionManager","registers ambient")

# ── Component: SmoothMusicController ─────────────────────────────────────────
component("COMP_SmoothMusicController", "SmoothMusicController")
end_component()
required("COMP_SmoothMusicController", "IAudioManager")
dependency("COMP_SmoothMusicController", "COMP_ServiceLocator", "uses")

# ── Component: UIAudioAutoInstaller ──────────────────────────────────────────
component("COMP_UIAudioAutoInstaller", "UIAudioAutoInstaller")
end_component()
required("COMP_UIAudioAutoInstaller", "IAudioManager")
dependency("COMP_UIAudioAutoInstaller", "COMP_ServiceLocator", "uses")

# ── write deferred ────────────────────────────────────────────────────────────
for rel in deferred:
    lines.append(rel)

end_pkg()
lines.append('</uml:Model>')

content = "\n".join(lines)
out = "EchoRift_Audio_Component.xml"
with open(out, "w", encoding="utf-8") as f:
    f.write(content)

import xml.etree.ElementTree as ET
try:
    ET.parse(out)
    comps = sum(1 for l in lines if 'uml:Component"' in l)
    ifaces = sum(1 for l in lines if 'uml:Interface"' in l)
    deps  = sum(1 for l in lines if 'uml:Dependency"' in l or 'uml:Usage"' in l)
    print(f"XML valid!  {len(lines)} lines -> {out}")
    print(f"  Components : {comps}")
    print(f"  Interfaces : {ifaces}")
    print(f"  Deps/Usage : {deps}")
except ET.ParseError as e:
    src = content.split('\n')
    ln = e.position[0]
    print("ERROR:", e)
    print("Line", ln, ":", repr(src[ln-1]))
