#!/usr/bin/env python3
"""Generate EMX/UML2 XMI for Audio module"""
import uuid

def uid(n):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.audio." + n)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="AudioSystem">')
lines.append('')

deferred = []

# ── helpers ───────────────────────────────────────────────────────────────────
def pkg(key, name, parent="ROOT"):
    pid = uid(key)
    lines.append(f'  <packagedElement xmi:type="uml:Package" xmi:id="{pid}" name="{name}">')
    return pid

def end_pkg():
    lines.append('  </packagedElement>')

def cls(key, name, abstract=False, iface=False, indent="    "):
    t = "uml:Interface" if iface else "uml:Class"
    ab = ' isAbstract="true"' if abstract else ''
    lines.append(f'{indent}<packagedElement xmi:type="{t}" xmi:id="{uid(key)}" name="{name}" visibility="public"{ab}>')

def end_cls(indent="    "):
    lines.append(f'{indent}</packagedElement>')

def attr(owner, name, typ, vis="private", indent="      "):
    safe = typ.replace("<","[").replace(">","]")
    lines.append(f'{indent}<ownedAttribute xmi:id="{uid(owner+"_a_"+name)}" name="{name}" visibility="{vis}" type="{safe}"/>')

def op(owner, name, ret="void", vis="public", abstract=False, indent="      "):
    ab = ' isAbstract="true"' if abstract else ''
    lines.append(f'{indent}<ownedOperation xmi:id="{uid(owner+"_o_"+name)}" name="{name}" visibility="{vis}"{ab}/>')

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

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Core
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Core", "Core")

# IAudioManager (interface)
cls("IAudioManager", "IAudioManager", iface=True)
for m in ["AddSoundFromPath","GetEnumerator","Play","PlayAtTimeStamp","PlayOneShot",
          "PlayDelayed","PlayScheduled","Stop","ToggleMute","TogglePause",
          "LerpVolume","LerpPitch","TryGetSource","GetProgress","GetPlaybackPosition",
          "SetPlaybackDirection","ChangePitch","GetClipLength","SubscribeSourceChanged",
          "UnsubscribeSourceChanged","SubscribeProgressCoroutine","UnsubscribeProgressCoroutine",
          "RegisterChildAt3DPos","RegisterChildAttachedToGo","DeregisterChild",
          "ChangeGroupValue","GetGroupValue","ResetGroupValue","LerpGroupValue",
          "RemoveGroup","AddGroup","RemoveSound","Set3DAudioOptions",
          "SetStartTime","SkipTime","SetTypeVolume","GetTypeVolume"]:
    op("IAudioManager", m, abstract=True)
end_cls()

# IFluentAudioManager (interface)
cls("IFluentAudioManager", "IFluentAudioManager", iface=True)
for m in ["Execute","Play","PlayAtTimeStamp","PlayDelayed","PlayOneShot","PlayScheduled",
          "Stop","RemoveSound","DeregisterChild","GetPlaybackPosition","SetPlaybackDirection",
          "ChangePitch","GetClipLength","ToggleMute","TogglePause","SubscribeSourceChanged",
          "UnsubscribeSourceChanged","SubscribeProgressCoroutine","UnsubscribeProgressCoroutine",
          "GetProgress","TryGetSource","LerpPitch","LerpVolume","ChangeGroupValue",
          "GetGroupValue","ResetGroupValue","LerpGroupValue","RemoveGroup","AddGroup",
          "Set3DAudioOptions","SetStartTime","SkipTime"]:
    op("IFluentAudioManager", m, abstract=True)
end_cls()

# AudioSourceWrapper
cls("AudioSourceWrapper", "AudioSourceWrapper")
attr("AudioSourceWrapper", "m_wrappedSource", "AudioSource", "private")
attr("AudioSourceWrapper", "m_childrenDictionary", "IDictionary[ChildType,AudioSource]", "private")
attr("AudioSourceWrapper", "m_cb", "SourceChangedCallback", "private")
attr("AudioSourceWrapper", "soundTypeIndex", "int", "public")
attr("AudioSourceWrapper", "lastTypeVolume", "float", "public")
attr("AudioSourceWrapper", "lerpAlpha", "float", "public")
attr("AudioSourceWrapper", "Source", "AudioSource", "public")
attr("AudioSourceWrapper", "Volume", "float", "public")
attr("AudioSourceWrapper", "Pitch", "float", "public")
attr("AudioSourceWrapper", "Loop", "bool", "public")
attr("AudioSourceWrapper", "Mute", "bool", "public")
attr("AudioSourceWrapper", "MixerGroup", "AudioMixerGroup", "public")
op("AudioSourceWrapper", "GetChildren", "ICollection[AudioSource]")
op("AudioSourceWrapper", "TryGetRegisteredChild", "bool")
op("AudioSourceWrapper", "DeregisterChildren")
op("AudioSourceWrapper", "DeregisterChild", "AudioError")
op("AudioSourceWrapper", "RegisterNewChild")
op("AudioSourceWrapper", "RegisterCallback")
op("AudioSourceWrapper", "DeregisterCallback")
op("AudioSourceWrapper", "SetChildrenVolume")
end_cls()

# Constants (struct)
cls("Constants", "Constants")
attr("Constants", "DEFAULT_VOLUME", "float", "public")
attr("Constants", "DEFAULT_PITCH", "float", "public")
attr("Constants", "DEFAULT_LOOP", "bool", "public")
attr("Constants", "DEFAULT_DURATION", "float", "public")
attr("Constants", "DEFAULT_CHILD_TYPE", "ChildType", "public")
attr("Constants", "NULL_AUDIO_ERROR", "AudioError", "public")
end_cls()

# Enums
for ename, elits in [
    ("AudioError", ["OK","DOES_NOT_EXIST","ALREADY_EXISTS","INVALID_PATH","INVALID_END_VALUE",
                    "INVALID_TIME","INVALID_PROGRESS","MIXER_NOT_EXPOSED","MISSING_SOURCE",
                    "MISSING_MIXER_GROUP","CAN_NOT_BE_3D","NOT_INITIALIZED","MISSING_CLIP",
                    "MISSING_PARENT","INVALID_PARENT","ALREADY_SUBSCRIBED","NOT_SUBSCRIBED",
                    "MISSING_WRAPPER","INVALID_CHILD"]),
    ("ChildType",  ["ALL","PARENT","AT_3D_POS","ATTCHD_TO_GO"]),
    ("ProgressResponse", ["UNSUB","RESUB_IN_LOOP","RESUB_IMMEDIATE"]),
]:
    lines.append(f'    <packagedElement xmi:type="uml:Enumeration" xmi:id="{uid(ename)}" name="{ename}" visibility="public">')
    for lit in elits:
        lines.append(f'      <ownedLiteral xmi:id="{uid(ename+lit)}" name="{lit}"/>')
    lines.append('    </packagedElement>')

end_pkg()  # Core

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Locator
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Locator", "Locator")

# ServiceLocator (static)
cls("ServiceLocator", "ServiceLocator")
attr("ServiceLocator", "s_audioManagerService", "IAudioManager", "private")
attr("ServiceLocator", "s_nullAudioManagerService", "NullAudioManager", "private")
op("ServiceLocator", "GetService", "IAudioManager")
op("ServiceLocator", "RegisterService")
op("ServiceLocator", "RegisterLogger")
end_cls()
assoc("ServiceLocator", "IAudioManager", "s_audioManagerService")
assoc("ServiceLocator", "NullAudioManager", "s_nullAudioManagerService")

# NullAudioManager (Null Object pattern)
cls("NullAudioManager", "NullAudioManager")
for m in ["AddSoundFromPath","Play","PlayAtTimeStamp","PlayOneShot","PlayDelayed",
          "PlayScheduled","Stop","ToggleMute","TogglePause","LerpVolume","LerpPitch",
          "TryGetSource","GetProgress","GetPlaybackPosition","SetPlaybackDirection",
          "ChangePitch","GetClipLength","SubscribeSourceChanged","UnsubscribeSourceChanged",
          "SubscribeProgressCoroutine","UnsubscribeProgressCoroutine","RegisterChildAt3DPos",
          "RegisterChildAttachedToGo","DeregisterChild","ChangeGroupValue","GetGroupValue",
          "ResetGroupValue","LerpGroupValue","RemoveGroup","AddGroup","RemoveSound",
          "Set3DAudioOptions","SetStartTime","SkipTime","SetTypeVolume","GetTypeVolume",
          "GetEnumerator"]:
    op("NullAudioManager", m)
end_cls()
realization("NullAudioManager", "IAudioManager")

end_pkg()  # Locator

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Logger
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Logger", "Logger")

# IAudioLogger (interface)
cls("IAudioLogger", "IAudioLogger", iface=True)
for m in ["Log","LogFormat","LogException","LogAssert","LogAssertFormat"]:
    op("IAudioLogger", m, abstract=True)
end_cls()

# AudioLogger
cls("AudioLogger", "AudioLogger")
attr("AudioLogger", "m_logLevel", "LoggingLevel", "private")
for m in ["Log","LogFormat","LogException","LogAssert","LogAssertFormat"]:
    op("AudioLogger", m)
end_cls()
realization("AudioLogger", "IAudioLogger")

# LoggedAudioManager (Decorator pattern)
cls("LoggedAudioManager", "LoggedAudioManager")
attr("LoggedAudioManager", "m_logger", "IAudioLogger", "private")
attr("LoggedAudioManager", "m_wrappedInstance", "IAudioManager", "private")
attr("LoggedAudioManager", "m_logContext", "Object", "private")
attr("LoggedAudioManager", "m_enterMethodTime", "float", "private")
for m in ["AddSoundFromPath","Play","PlayAtTimeStamp","Stop","LerpVolume","LerpPitch",
          "TryGetSource","SetTypeVolume","GetTypeVolume"]:
    op("LoggedAudioManager", m)
end_cls()
realization("LoggedAudioManager", "IAudioManager")
assoc("LoggedAudioManager", "IAudioLogger", "m_logger")
assoc("LoggedAudioManager", "IAudioManager", "m_wrappedInstance")

# Enums
for ename, elits in [
    ("LoggingLevel", ["NONE","LOW","INTERMEDIATE","HIGH","STOPWATCH"]),
    ("LoggingType",  ["NORMAL","WARNING","ERROR","ASSERTION"]),
]:
    lines.append(f'    <packagedElement xmi:type="uml:Enumeration" xmi:id="{uid(ename)}" name="{ename}" visibility="public">')
    for lit in elits:
        lines.append(f'      <ownedLiteral xmi:id="{uid(ename+lit)}" name="{lit}"/>')
    lines.append('    </packagedElement>')

end_pkg()  # Logger

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Service
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Service", "Service")

# DefaultAudioManager
cls("DefaultAudioManager", "DefaultAudioManager")
attr("DefaultAudioManager", "m_parentGameObject", "GameObject", "private")
attr("DefaultAudioManager", "m_parentBehaviour", "MonoBehaviour", "private")
attr("DefaultAudioManager", "m_soundDictionary", "IDictionary[string,AudioSourceWrapper]", "private")
attr("DefaultAudioManager", "m_soundProgressDictionary", "IDictionary[string,IDictionary]", "private")
attr("DefaultAudioManager", "_typeVolumes", "Dictionary[int,float]", "private")
for m in ["AddSoundFromPath","Play","PlayAtTimeStamp","PlayOneShot","PlayDelayed",
          "PlayScheduled","Stop","ToggleMute","TogglePause","LerpVolume","LerpPitch",
          "TryGetSource","GetProgress","GetPlaybackPosition","SetPlaybackDirection",
          "ChangePitch","GetClipLength","SubscribeSourceChanged","UnsubscribeSourceChanged",
          "SubscribeProgressCoroutine","UnsubscribeProgressCoroutine","RegisterChildAt3DPos",
          "RegisterChildAttachedToGo","DeregisterChild","ChangeGroupValue","GetGroupValue",
          "ResetGroupValue","LerpGroupValue","RemoveGroup","AddGroup","RemoveSound",
          "Set3DAudioOptions","SetStartTime","SkipTime","SetTypeVolume","GetTypeVolume",
          "GetEnumerator"]:
    op("DefaultAudioManager", m)
end_cls()
realization("DefaultAudioManager", "IAudioManager")
assoc("DefaultAudioManager", "AudioSourceWrapper", "m_soundDictionary", "composite", "0..*")

# FluentAudioManager (Decorator/Fluent pattern)
cls("FluentAudioManager", "FluentAudioManager")
attr("FluentAudioManager", "m_wrappedInstance", "IAudioManager", "private")
attr("FluentAudioManager", "m_name", "string", "private")
attr("FluentAudioManager", "m_child", "ChildType", "private")
attr("FluentAudioManager", "m_error", "AudioError", "private")
op("FluentAudioManager", "ReuseInstance")
op("FluentAudioManager", "Execute", "AudioError")
op("FluentAudioManager", "Play", "AudioError")
op("FluentAudioManager", "Stop", "AudioError")
op("FluentAudioManager", "LerpVolume", "IFluentAudioManager")
end_cls()
realization("FluentAudioManager", "IFluentAudioManager")
assoc("FluentAudioManager", "IAudioManager", "m_wrappedInstance")

# AudioChainer (static factory)
cls("AudioChainer", "AudioChainer")
op("AudioChainer", "AddSoundFromPath", "IFluentAudioManager")
op("AudioChainer", "SelectSound", "IFluentAudioManager")
op("AudioChainer", "RegisterChildAt3DPos", "IFluentAudioManager")
op("AudioChainer", "RegisterChildAttachedToGo", "IFluentAudioManager")
end_cls()
dep("AudioChainer", "IAudioManager", "uses")
dep("AudioChainer", "FluentAudioManager", "creates")

end_pkg()  # Service

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Provider
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Provider", "Provider")

cls("AudioManagerSettings", "AudioManagerSettings")
attr("AudioManagerSettings", "loggingLevel", "LoggingLevel", "private")
attr("AudioManagerSettings", "settings", "AudioSourceSetting[]", "private")
attr("AudioManagerSettings", "customHideFlags", "HideFlags", "private")
op("AudioManagerSettings", "Awake", vis="private")
op("AudioManagerSettings", "OnEnable", vis="private")
end_cls()
dep("AudioManagerSettings", "ServiceLocator", "registers")
dep("AudioManagerSettings", "DefaultAudioManager", "creates")
dep("AudioManagerSettings", "AudioLogger", "creates")
assoc("AudioManagerSettings", "AudioSourceSetting", "settings", "composite", "0..*")

end_pkg()  # Provider

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Settings
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Settings", "Settings")

cls("AudioSourceSetting", "AudioSourceSetting")
attr("AudioSourceSetting", "audioClips", "List[AudioClips]", "public")
end_cls()

cls("AudioClips", "AudioClips")
attr("AudioClips", "soundName", "string", "public")
attr("AudioClips", "type", "SoundType", "public")
attr("AudioClips", "audioClip", "AudioClip", "public")
attr("AudioClips", "mixerGroup", "AudioMixerGroup", "public")
attr("AudioClips", "loop", "bool", "public")
attr("AudioClips", "volume", "float", "public")
attr("AudioClips", "pitch", "float", "public")
attr("AudioClips", "spatialBlend", "float", "public")
attr("AudioClips", "source", "AudioSource", "public")
end_cls()
assoc("AudioSourceSetting", "AudioClips", "audioClips", "composite", "0..*")

lines.append(f'    <packagedElement xmi:type="uml:Enumeration" xmi:id="{uid("SoundType")}" name="SoundType" visibility="public">')
for lit in ["SFX","Music"]:
    lines.append(f'      <ownedLiteral xmi:id="{uid("SoundType"+lit)}" name="{lit}"/>')
lines.append('    </packagedElement>')

end_pkg()  # Settings

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: Helper
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_Helper", "Helper")

cls("AudioHelper", "AudioHelper")
op("AudioHelper", "LoadAudioClipFromPath", "AudioError")
op("AudioHelper", "AttachAudioSource")
op("AudioHelper", "AddAudioSourceComponent")
op("AudioHelper", "IsSound2D", "bool")
op("AudioHelper", "IsEndValueValid", "bool")
op("AudioHelper", "ConvertToAudioError", "AudioError")
end_cls()

end_pkg()  # Helper

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: GameScripts (project-specific audio scripts)
# ══════════════════════════════════════════════════════════════════════════════
pkg("PKG_GameScripts", "GameScripts")

# UIAudioLogger
cls("UIAudioLogger", "UIAudioLogger")
attr("UIAudioLogger", "m_logOutput", "Text", "private")
attr("UIAudioLogger", "m_logLevel", "LoggingLevel", "private")
for m in ["Log","LogFormat","LogException","LogAssert","LogAssertFormat"]:
    op("UIAudioLogger", m)
end_cls()
realization("UIAudioLogger", "IAudioLogger")
assoc("UIAudioLogger", "LoggingLevel", "m_logLevel")

# UIAudioAutoInstaller
cls("UIAudioAutoInstaller", "UIAudioAutoInstaller")
attr("UIAudioAutoInstaller", "clickSoundName", "string", "private")
op("UIAudioAutoInstaller", "Start", vis="private")
op("UIAudioAutoInstaller", "InitializeWithDelay", vis="private")
op("UIAudioAutoInstaller", "PlaySound", vis="private")
end_cls()
dep("UIAudioAutoInstaller", "ServiceLocator", "uses")
dep("UIAudioAutoInstaller", "IAudioManager", "uses")

# AreaAmbientSound
cls("AreaAmbientSound", "AreaAmbientSound")
attr("AreaAmbientSound", "soundName", "string", "private")
attr("AreaAmbientSound", "fadeDuration", "float", "private")
attr("AreaAmbientSound", "targetVolume", "float", "private")
attr("AreaAmbientSound", "playOnEnter", "bool", "private")
attr("AreaAmbientSound", "stopOnExit", "bool", "private")
attr("AreaAmbientSound", "loop", "bool", "private")
attr("AreaAmbientSound", "_am", "IAudioManager", "private")
attr("AreaAmbientSound", "_musicManager", "MusicTransitionManager", "private")
attr("AreaAmbientSound", "_isPlaying", "bool", "private")
op("AreaAmbientSound", "OnTriggerEnter2D", vis="private")
op("AreaAmbientSound", "OnTriggerExit2D", vis="private")
op("AreaAmbientSound", "PlaySound", vis="private")
op("AreaAmbientSound", "StopSound", vis="private")
end_cls()
assoc("AreaAmbientSound", "IAudioManager", "_am")
assoc("AreaAmbientSound", "MusicTransitionManager", "_musicManager")
dep("AreaAmbientSound", "ServiceLocator", "uses")

# MusicTransitionManager
cls("MusicTransitionManager", "MusicTransitionManager")
attr("MusicTransitionManager", "sceneMusicSettings", "List[SceneMusicConfig]", "private")
attr("MusicTransitionManager", "fadeDuration", "float", "private")
attr("MusicTransitionManager", "normalVolume", "float", "private")
attr("MusicTransitionManager", "pausedVolume", "float", "private")
attr("MusicTransitionManager", "_am", "IAudioManager", "private")
attr("MusicTransitionManager", "_currentMusicNames", "List[string]", "private")
attr("MusicTransitionManager", "_ambientSounds", "List[string]", "private")
attr("MusicTransitionManager", "_isPaused", "bool", "private")
attr("MusicTransitionManager", "_isDialogueActive", "bool", "private")
op("MusicTransitionManager", "RegisterAmbient")
op("MusicTransitionManager", "UnregisterAmbient")
op("MusicTransitionManager", "DuckMusic")
op("MusicTransitionManager", "RestoreMusic")
op("MusicTransitionManager", "HandleMusicChange", vis="private")
op("MusicTransitionManager", "OnConversationStarted", vis="private")
op("MusicTransitionManager", "OnConversationEnded", vis="private")
end_cls()
assoc("MusicTransitionManager", "IAudioManager", "_am")
assoc("MusicTransitionManager", "SceneMusicConfig", "sceneMusicSettings", "composite", "0..*")
dep("MusicTransitionManager", "ServiceLocator", "uses")

# SceneMusicConfig
cls("SceneMusicConfig", "SceneMusicConfig")
attr("SceneMusicConfig", "scene", "SerializableScene", "public")
attr("SceneMusicConfig", "musicNames", "List[string]", "public")
end_cls()

# SmoothMusicController
cls("SmoothMusicController", "SmoothMusicController")
attr("SmoothMusicController", "musicName", "string", "private")
attr("SmoothMusicController", "fadeDuration", "float", "private")
attr("SmoothMusicController", "targetVolume", "float", "private")
attr("SmoothMusicController", "am", "IAudioManager", "private")
op("SmoothMusicController", "FadeIn")
op("SmoothMusicController", "FadeOut")
end_cls()
assoc("SmoothMusicController", "IAudioManager", "am")
dep("SmoothMusicController", "ServiceLocator", "uses")

end_pkg()  # GameScripts

# ── write deferred relationships ──────────────────────────────────────────────
for rel in deferred:
    lines.append(rel)

lines.append('</uml:Model>')

content = "\n".join(lines)
with open("EchoRift_Audio.xml", "w", encoding="utf-8") as f:
    f.write(content)

import xml.etree.ElementTree as ET
try:
    ET.parse("EchoRift_Audio.xml")
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
