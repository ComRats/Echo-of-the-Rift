#!/usr/bin/env python3
"""Generate UML2 XMI Activity Diagrams for Audio module (EA-compatible)"""
import uuid

def uid(n):
    return "EAID_" + str(uuid.uuid5(uuid.NAMESPACE_DNS, "echorift.audio.act." + n)).upper().replace("-","_")

lines = []
lines.append('<?xml version="1.0" encoding="UTF-8"?>')
lines.append('<uml:Model xmi:version="2.1"')
lines.append('  xmlns:xmi="http://www.omg.org/spec/XMI/20131001"')
lines.append('  xmlns:uml="http://www.eclipse.org/uml2/5.0.0/UML"')
lines.append(f'  xmi:id="{uid("ROOT")}" name="AudioSystem_Activity">')
lines.append('')

# ── helpers ───────────────────────────────────────────────────────────────────

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
    safe = name.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
    lines.append(f'      <ownedNode xmi:type="uml:OpaqueAction" xmi:id="{uid(key)}" name="{safe}"/>')

def decision(key, name):
    safe = name.replace("&", "&amp;").replace("<", "&lt;").replace(">", "&gt;")
    lines.append(f'      <ownedNode xmi:type="uml:DecisionNode" xmi:id="{uid(key)}" name="{safe}"/>')

def merge(key, name=""):
    lines.append(f'      <ownedNode xmi:type="uml:MergeNode" xmi:id="{uid(key)}" name="{name}"/>')

def fork(key):
    lines.append(f'      <ownedNode xmi:type="uml:ForkNode" xmi:id="{uid(key)}" name=""/>')

def join(key):
    lines.append(f'      <ownedNode xmi:type="uml:JoinNode" xmi:id="{uid(key)}" name=""/>')

def partition(key, name):
    lines.append(f'      <ownedGroup xmi:type="uml:ActivityPartition" xmi:id="{uid(key)}" name="{name}">')

def end_partition():
    lines.append('      </ownedGroup>')

def flow(key, src_key, tgt_key, guard=""):
    g = f' guard="{guard}"' if guard else ''
    lines.append(
        f'      <ownedEdge xmi:type="uml:ControlFlow" xmi:id="{uid(key)}"'
        f' source="{uid(src_key)}" target="{uid(tgt_key)}"{g}/>'
    )

# ══════════════════════════════════════════════════════════════════════════════
# PACKAGE: AudioSystem_Activity
# ══════════════════════════════════════════════════════════════════════════════
begin_pkg("PKG_AUDIO_ACT", "AudioSystem_Activity")

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 1: Scene Music Transition  (MusicTransitionManager)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_MT", "Scene Music Transition")

initial("MT_INIT")
action ("MT_AWAKE",        "Awake: DontDestroyOnLoad")
action ("MT_SUBSCRIBE",    "OnEnable: подписаться на SceneManager.sceneLoaded, DialogueManager events")
action ("MT_START",        "Start: получить IAudioManager, вызвать HandleMusicChange(currentScene)")
action ("MT_SCENE_LOADED", "Событие: OnSceneLoaded(scene)")
action ("MT_GET_MUSIC",    "GetMusicForScene(sceneName) -> targetMusicList")
decision("MT_DEC_EMPTY",   "targetMusicList пустой?")
decision("MT_DEC_SAME",    "Списки одинаковые?")
action ("MT_RESTORE_VOL",  "LerpVolume(играющий трек, normalVolume, fadeDuration)")
action ("MT_FADE_OLD",     "LerpVolume(oldMusic, 0, fadeDuration) для каждого старого трека")
action ("MT_UPDATE_LIST",  "_currentMusicNames = targetMusicList")
action ("MT_PLAY_NEW",     "Play(newMusic), volume=0, LerpVolume(newMusic, normalVolume, fadeDuration)")
final  ("MT_FINAL")

flow("MT_E01", "MT_INIT",        "MT_AWAKE")
flow("MT_E02", "MT_AWAKE",       "MT_SUBSCRIBE")
flow("MT_E03", "MT_SUBSCRIBE",   "MT_START")
flow("MT_E04", "MT_START",       "MT_GET_MUSIC")
flow("MT_E05", "MT_SCENE_LOADED","MT_GET_MUSIC")
flow("MT_E06", "MT_GET_MUSIC",   "MT_DEC_EMPTY")
flow("MT_E07", "MT_DEC_EMPTY",   "MT_FINAL",      "[да - список пуст]")
flow("MT_E08", "MT_DEC_EMPTY",   "MT_DEC_SAME",   "[нет]")
flow("MT_E09", "MT_DEC_SAME",    "MT_RESTORE_VOL","[да - треки совпадают]")
flow("MT_E10", "MT_DEC_SAME",    "MT_FADE_OLD",   "[нет - треки изменились]")
flow("MT_E11", "MT_RESTORE_VOL", "MT_FINAL")
flow("MT_E12", "MT_FADE_OLD",    "MT_UPDATE_LIST")
flow("MT_E13", "MT_UPDATE_LIST", "MT_PLAY_NEW")
flow("MT_E14", "MT_PLAY_NEW",    "MT_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 2: Dialogue Music Duck  (MusicTransitionManager)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_DD", "Dialogue Music Duck")

initial  ("DD_INIT")
action   ("DD_CONV_START",   "Событие: OnConversationStarted(actor)")
decision ("DD_DEC_ACTIVE",   "_isDialogueActive уже true?")
action   ("DD_SET_ACTIVE",   "_isDialogueActive = true")
action   ("DD_DUCK",         "LerpVolume(все звуки, pausedVolume, fadeDuration)")
action   ("DD_CONV_END",     "Событие: OnConversationEnded(actor)")
decision ("DD_DEC_INACTIVE", "_isDialogueActive уже false?")
action   ("DD_SET_INACTIVE", "_isDialogueActive = false")
action   ("DD_RESTORE",      "LerpVolume(все звуки, normalVolume, fadeDuration)")
final    ("DD_FINAL")

flow("DD_E01", "DD_INIT",        "DD_CONV_START")
flow("DD_E02", "DD_CONV_START",  "DD_DEC_ACTIVE")
flow("DD_E03", "DD_DEC_ACTIVE",  "DD_FINAL",       "[да - уже активен]")
flow("DD_E04", "DD_DEC_ACTIVE",  "DD_SET_ACTIVE",  "[нет]")
flow("DD_E05", "DD_SET_ACTIVE",  "DD_DUCK")
flow("DD_E06", "DD_DUCK",        "DD_CONV_END")
flow("DD_E07", "DD_CONV_END",    "DD_DEC_INACTIVE")
flow("DD_E08", "DD_DEC_INACTIVE","DD_FINAL",        "[да - уже неактивен]")
flow("DD_E09", "DD_DEC_INACTIVE","DD_SET_INACTIVE", "[нет]")
flow("DD_E10", "DD_SET_INACTIVE","DD_RESTORE")
flow("DD_E11", "DD_RESTORE",     "DD_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 3: Area Ambient Sound  (AreaAmbientSound)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_AMB", "Area Ambient Sound")

initial  ("AMB_INIT")
action   ("AMB_START",        "Start: получить IAudioManager, найти MusicTransitionManager")
action   ("AMB_ENTER",        "Событие: OnTriggerEnter2D(other)")
decision ("AMB_DEC_ENTER",    "playOnEnter == true AND tag == Player?")
decision ("AMB_DEC_PLAYING",  "_isPlaying уже true?")
action   ("AMB_PLAY",         "_isPlaying = true\nPlay(soundName), RegisterAmbient(soundName)\nустановить loop, volume=0\nLerpVolume(soundName, targetVolume, fadeDuration)")
action   ("AMB_EXIT",         "Событие: OnTriggerExit2D(other)")
decision ("AMB_DEC_EXIT",     "stopOnExit == true AND tag == Player?")
decision ("AMB_DEC_STOPPED",  "_isPlaying уже false?")
action   ("AMB_STOP",         "_isPlaying = false\nLerpVolume(soundName, 0, fadeDuration)\nUnregisterAmbient(soundName)")
decision ("AMB_DEC_ACTIVE",   "gameObject активен?")
action   ("AMB_COROUTINE",    "StartCoroutine(StopAfterFade)\nждать fadeDuration\nStop(soundName)")
action   ("AMB_STOP_IMMED",   "Stop(soundName) немедленно")
final    ("AMB_FINAL")

flow("AMB_E01", "AMB_INIT",        "AMB_START")
flow("AMB_E02", "AMB_START",       "AMB_ENTER")
flow("AMB_E03", "AMB_ENTER",       "AMB_DEC_ENTER")
flow("AMB_E04", "AMB_DEC_ENTER",   "AMB_EXIT",        "[нет]")
flow("AMB_E05", "AMB_DEC_ENTER",   "AMB_DEC_PLAYING", "[да]")
flow("AMB_E06", "AMB_DEC_PLAYING", "AMB_EXIT",        "[да - уже играет]")
flow("AMB_E07", "AMB_DEC_PLAYING", "AMB_PLAY",        "[нет]")
flow("AMB_E08", "AMB_PLAY",        "AMB_EXIT")
flow("AMB_E09", "AMB_EXIT",        "AMB_DEC_EXIT")
flow("AMB_E10", "AMB_DEC_EXIT",    "AMB_FINAL",        "[нет]")
flow("AMB_E11", "AMB_DEC_EXIT",    "AMB_DEC_STOPPED",  "[да]")
flow("AMB_E12", "AMB_DEC_STOPPED", "AMB_FINAL",        "[да - уже остановлен]")
flow("AMB_E13", "AMB_DEC_STOPPED", "AMB_STOP",         "[нет]")
flow("AMB_E14", "AMB_STOP",        "AMB_DEC_ACTIVE")
flow("AMB_E15", "AMB_DEC_ACTIVE",  "AMB_COROUTINE",    "[да]")
flow("AMB_E16", "AMB_DEC_ACTIVE",  "AMB_STOP_IMMED",   "[нет]")
flow("AMB_E17", "AMB_COROUTINE",   "AMB_FINAL")
flow("AMB_E18", "AMB_STOP_IMMED",  "AMB_FINAL")

end_activity()

# ─────────────────────────────────────────────────────────────────────────────
# ACTIVITY 4: UI Audio Auto Installer  (UIAudioAutoInstaller)
# ─────────────────────────────────────────────────────────────────────────────
begin_activity("ACT_UA", "UI Audio Auto Installer")

initial  ("UA_INIT")
action   ("UA_START",        "Start: запустить корутину InitializeWithDelay()")
action   ("UA_WAIT",         "yield WaitForSeconds(0.1f)")
action   ("UA_GET_AM",       "ServiceLocator.GetService() -> audioManager")
decision ("UA_DEC_AM",       "audioManager == null?")
action   ("UA_WARN_AM",      "LogWarning: AudioManager not found")
action   ("UA_CHECK_SOUND",  "TryGetSource(clickSoundName)")
decision ("UA_DEC_SOUND",    "Звук зарегистрирован (AudioError.OK)?")
action   ("UA_WARN_SOUND",   "LogWarning: Sound not registered")
action   ("UA_GET_BUTTONS",  "GetComponentsInChildren(Button, includeInactive=true)")
action   ("UA_BIND",         "Для каждой кнопки: btn.onClick.AddListener(PlaySound)")
action   ("UA_PLAY",         "[Runtime] PlaySound:\nServiceLocator.GetService()\naudioManager.Play(clickSoundName, ChildType.PARENT)")
final    ("UA_FINAL")

flow("UA_E01", "UA_INIT",        "UA_START")
flow("UA_E02", "UA_START",       "UA_WAIT")
flow("UA_E03", "UA_WAIT",        "UA_GET_AM")
flow("UA_E04", "UA_GET_AM",      "UA_DEC_AM")
flow("UA_E05", "UA_DEC_AM",      "UA_WARN_AM",      "[да - null]")
flow("UA_E06", "UA_DEC_AM",      "UA_CHECK_SOUND",  "[нет]")
flow("UA_E07", "UA_WARN_AM",     "UA_FINAL")
flow("UA_E08", "UA_CHECK_SOUND", "UA_DEC_SOUND")
flow("UA_E09", "UA_DEC_SOUND",   "UA_WARN_SOUND",   "[нет - не найден]")
flow("UA_E10", "UA_DEC_SOUND",   "UA_GET_BUTTONS",  "[да - OK]")
flow("UA_E11", "UA_WARN_SOUND",  "UA_FINAL")
flow("UA_E12", "UA_GET_BUTTONS", "UA_BIND")
flow("UA_E13", "UA_BIND",        "UA_FINAL")
flow("UA_E14", "UA_BIND",        "UA_PLAY",         "[клик по кнопке]")
flow("UA_E15", "UA_PLAY",        "UA_FINAL")

end_activity()

end_pkg()

lines.append('</uml:Model>')

# ── write & validate ──────────────────────────────────────────────────────────
content = "\n".join(lines)
out = "EchoRift_Audio_Activity.xml"
with open(out, "w", encoding="utf-8") as f:
    f.write(content)

import xml.etree.ElementTree as ET
try:
    ET.parse(out)
    acts  = sum(1 for l in lines if 'uml:Activity"'      in l)
    nodes = sum(1 for l in lines if 'ownedNode'           in l)
    edges = sum(1 for l in lines if 'ownedEdge'           in l)
    print(f"XML valid!  {len(lines)} lines -> {out}")
    print(f"  Activities : {acts}")
    print(f"  Nodes      : {nodes}")
    print(f"  Edges      : {edges}")
except ET.ParseError as e:
    src = content.split('\n')
    ln = e.position[0]
    print("ERROR:", e)
    print("Line", ln, ":", repr(src[ln-1]))
