/*
 * EA JScript: Create Audio Activity Diagrams
 * Запуск: EA -> Scripting -> New Script -> вставить -> Run
 * Создаёт 4 диаграммы деятельности в пакете AudioSystem_Activity
 */

var repo = Repository;

// ── helpers ──────────────────────────────────────────────────────────────────

function findOrCreatePackage(parentPkg, name) {
    var pkgs = parentPkg.Packages;
    for (var i = 0; i < pkgs.Count; i++) {
        if (pkgs.GetAt(i).Name == name) return pkgs.GetAt(i);
    }
    var p = pkgs.AddNew(name, "");
    p.Update();
    pkgs.Refresh();
    return p;
}

function createActivityDiagram(pkg, name) {
    var diags = pkg.Diagrams;
    var d = diags.AddNew(name, "Activity");
    d.Update();
    diags.Refresh();
    return d;
}

function addElement(pkg, name, type) {
    var elems = pkg.Elements;
    var e = elems.AddNew(name, type);
    e.Update();
    elems.Refresh();
    return e;
}

// Add element to diagram with position
function addToDiagram(diag, elem, x, y, w, h) {
    var objs = diag.DiagramObjects;
    var obj = objs.AddNew("", "");
    obj.ElementID = elem.ElementID;
    obj.left   = x;
    obj.right  = x + w;
    obj.top    = -y;
    obj.bottom = -(y + h);
    obj.Update();
    objs.Refresh();
    return obj;
}

// Add connector between two elements on diagram
function addFlow(diag, srcElem, tgtElem, guard) {
    var conn = srcElem.Connectors.AddNew("", "ControlFlow");
    conn.SupplierID = tgtElem.ElementID;
    if (guard && guard != "") {
        conn.TransitionGuard = guard;
    }
    conn.Update();
    srcElem.Connectors.Refresh();

    // Add to diagram
    var dlinks = diag.DiagramLinks;
    var lnk = dlinks.AddNew("", "");
    lnk.ConnectorID = conn.ConnectorID;
    lnk.Update();
    dlinks.Refresh();
    return conn;
}

// ── find root package ─────────────────────────────────────────────────────────

var rootPkg = null;
var models = repo.Models;
for (var m = 0; m < models.Count; m++) {
    var model = models.GetAt(m);
    var pkgs = model.Packages;
    for (var p = 0; p < pkgs.Count; p++) {
        if (pkgs.GetAt(p).Name == "AudioSystem_Activity") {
            rootPkg = pkgs.GetAt(p);
            break;
        }
    }
    if (!rootPkg) {
        // create under first model
        rootPkg = findOrCreatePackage(model, "AudioSystem_Activity");
        break;
    }
}

if (!rootPkg) {
    Session.Output("ERROR: Could not find or create root package");
} else {
    Session.Output("Root package: " + rootPkg.Name);
    buildAllDiagrams(rootPkg);
    Session.Output("DONE. Refresh the browser (F5).");
}

// ═════════════════════════════════════════════════════════════════════════════
function buildAllDiagrams(root) {
    buildSceneMusicTransition(root);
    buildDialogueDuck(root);
    buildAreaAmbientSound(root);
    buildUIAudioAutoInstaller(root);
}

// ─────────────────────────────────────────────────────────────────────────────
// 1. Scene Music Transition
// ─────────────────────────────────────────────────────────────────────────────
function buildSceneMusicTransition(root) {
    var pkg  = findOrCreatePackage(root, "Scene Music Transition");
    var diag = createActivityDiagram(pkg, "Scene Music Transition");

    var W = 200; var H = 40; var X = 100;

    var init        = addElement(pkg, "",                                                    "ActivityInitial");
    var awake       = addElement(pkg, "Awake: DontDestroyOnLoad",                           "Action");
    var subscribe   = addElement(pkg, "OnEnable: подписаться на SceneManager.sceneLoaded\nи DialogueManager events", "Action");
    var start       = addElement(pkg, "Start: получить IAudioManager\nHandleMusicChange(currentScene)", "Action");
    var sceneLoaded = addElement(pkg, "Событие: OnSceneLoaded(scene)",                      "Action");
    var getMusic    = addElement(pkg, "GetMusicForScene(sceneName) -> targetMusicList",      "Action");
    var decEmpty    = addElement(pkg, "targetMusicList пустой?",                             "Decision");
    var decSame     = addElement(pkg, "Списки одинаковые?",                                  "Decision");
    var restoreVol  = addElement(pkg, "LerpVolume(играющий трек, normalVolume, fadeDuration)", "Action");
    var fadeOld     = addElement(pkg, "LerpVolume(oldMusic, 0, fadeDuration)\nдля каждого старого трека", "Action");
    var updateList  = addElement(pkg, "_currentMusicNames = targetMusicList",                "Action");
    var playNew     = addElement(pkg, "Play(newMusic), volume=0\nLerpVolume(newMusic, normalVolume, fadeDuration)", "Action");
    var fin         = addElement(pkg, "",                                                    "ActivityFinal");

    // Layout: single column, Y steps of 70
    var Y = 20;
    addToDiagram(diag, init,        X+80, Y,      30,  30); Y += 60;
    addToDiagram(diag, awake,       X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, subscribe,   X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, start,       X,    Y,       W,   H); Y += 70;
    // sceneLoaded branches in from the side
    addToDiagram(diag, sceneLoaded, X+250,Y-140,   W,   H);
    addToDiagram(diag, getMusic,    X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, decEmpty,    X+50, Y,       100, 40); Y += 70;
    addToDiagram(diag, decSame,     X+50, Y,       100, 40); Y += 70;
    addToDiagram(diag, restoreVol,  X+250,Y,       W,   H);
    addToDiagram(diag, fadeOld,     X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, updateList,  X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, playNew,     X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, fin,         X+80, Y,       30,  30);

    addFlow(diag, init,        awake,      "");
    addFlow(diag, awake,       subscribe,  "");
    addFlow(diag, subscribe,   start,      "");
    addFlow(diag, start,       getMusic,   "");
    addFlow(diag, sceneLoaded, getMusic,   "");
    addFlow(diag, getMusic,    decEmpty,   "");
    addFlow(diag, decEmpty,    fin,        "[да - список пуст]");
    addFlow(diag, decEmpty,    decSame,    "[нет]");
    addFlow(diag, decSame,     restoreVol, "[да - треки совпадают]");
    addFlow(diag, decSame,     fadeOld,    "[нет - треки изменились]");
    addFlow(diag, restoreVol,  fin,        "");
    addFlow(diag, fadeOld,     updateList, "");
    addFlow(diag, updateList,  playNew,    "");
    addFlow(diag, playNew,     fin,        "");

    repo.ReloadDiagram(diag.DiagramID);
    Session.Output("  Created: Scene Music Transition");
}

// ─────────────────────────────────────────────────────────────────────────────
// 2. Dialogue Music Duck
// ─────────────────────────────────────────────────────────────────────────────
function buildDialogueDuck(root) {
    var pkg  = findOrCreatePackage(root, "Dialogue Music Duck");
    var diag = createActivityDiagram(pkg, "Dialogue Music Duck");

    var W = 220; var H = 40; var X = 100;

    var init        = addElement(pkg, "",                                                "ActivityInitial");
    var convStart   = addElement(pkg, "Событие: OnConversationStarted(actor)",           "Action");
    var decActive   = addElement(pkg, "_isDialogueActive уже true?",                    "Decision");
    var setActive   = addElement(pkg, "_isDialogueActive = true",                       "Action");
    var duck        = addElement(pkg, "LerpVolume(все звуки, pausedVolume, fadeDuration)", "Action");
    var convEnd     = addElement(pkg, "Событие: OnConversationEnded(actor)",             "Action");
    var decInactive = addElement(pkg, "_isDialogueActive уже false?",                   "Decision");
    var setInactive = addElement(pkg, "_isDialogueActive = false",                      "Action");
    var restore     = addElement(pkg, "LerpVolume(все звуки, normalVolume, fadeDuration)", "Action");
    var fin         = addElement(pkg, "",                                                "ActivityFinal");

    var Y = 20;
    addToDiagram(diag, init,        X+90, Y,      30,  30); Y += 60;
    addToDiagram(diag, convStart,   X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, decActive,   X+60, Y,       100, 40); Y += 70;
    addToDiagram(diag, setActive,   X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, duck,        X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, convEnd,     X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, decInactive, X+60, Y,       100, 40); Y += 70;
    addToDiagram(diag, setInactive, X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, restore,     X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, fin,         X+90, Y,       30,  30);

    addFlow(diag, init,        convStart,   "");
    addFlow(diag, convStart,   decActive,   "");
    addFlow(diag, decActive,   fin,         "[да - уже активен]");
    addFlow(diag, decActive,   setActive,   "[нет]");
    addFlow(diag, setActive,   duck,        "");
    addFlow(diag, duck,        convEnd,     "");
    addFlow(diag, convEnd,     decInactive, "");
    addFlow(diag, decInactive, fin,         "[да - уже неактивен]");
    addFlow(diag, decInactive, setInactive, "[нет]");
    addFlow(diag, setInactive, restore,     "");
    addFlow(diag, restore,     fin,         "");

    repo.ReloadDiagram(diag.DiagramID);
    Session.Output("  Created: Dialogue Music Duck");
}

// ─────────────────────────────────────────────────────────────────────────────
// 3. Area Ambient Sound
// ─────────────────────────────────────────────────────────────────────────────
function buildAreaAmbientSound(root) {
    var pkg  = findOrCreatePackage(root, "Area Ambient Sound");
    var diag = createActivityDiagram(pkg, "Area Ambient Sound");

    var W = 230; var H = 40; var X = 100;

    var init       = addElement(pkg, "",                                                          "ActivityInitial");
    var start      = addElement(pkg, "Start: получить IAudioManager\nнайти MusicTransitionManager", "Action");
    var enter      = addElement(pkg, "Событие: OnTriggerEnter2D(other)",                          "Action");
    var decEnter   = addElement(pkg, "playOnEnter == true AND tag == Player?",                    "Decision");
    var decPlaying = addElement(pkg, "_isPlaying уже true?",                                      "Decision");
    var play       = addElement(pkg, "_isPlaying = true\nPlay(soundName), RegisterAmbient(soundName)\nloop, volume=0, LerpVolume(targetVolume)", "Action");
    var exit_ev    = addElement(pkg, "Событие: OnTriggerExit2D(other)",                           "Action");
    var decExit    = addElement(pkg, "stopOnExit == true AND tag == Player?",                     "Decision");
    var decStopped = addElement(pkg, "_isPlaying уже false?",                                     "Decision");
    var stop       = addElement(pkg, "_isPlaying = false\nLerpVolume(0, fadeDuration)\nUnregisterAmbient(soundName)", "Action");
    var decActive  = addElement(pkg, "gameObject активен?",                                       "Decision");
    var coroutine  = addElement(pkg, "StartCoroutine(StopAfterFade)\nждать fadeDuration -> Stop(soundName)", "Action");
    var stopImmed  = addElement(pkg, "Stop(soundName) немедленно",                                "Action");
    var fin        = addElement(pkg, "",                                                          "ActivityFinal");

    var Y = 20;
    addToDiagram(diag, init,       X+90, Y,      30,  30); Y += 60;
    addToDiagram(diag, start,      X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, enter,      X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, decEnter,   X+65, Y,       100, 40); Y += 70;
    addToDiagram(diag, decPlaying, X+65, Y,       100, 40); Y += 70;
    addToDiagram(diag, play,       X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, exit_ev,    X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, decExit,    X+65, Y,       100, 40); Y += 70;
    addToDiagram(diag, decStopped, X+65, Y,       100, 40); Y += 70;
    addToDiagram(diag, stop,       X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, decActive,  X+65, Y,       100, 40); Y += 70;
    addToDiagram(diag, coroutine,  X,    Y,       W,   H);
    addToDiagram(diag, stopImmed,  X+280,Y,       W,   H); Y += 70;
    addToDiagram(diag, fin,        X+90, Y,       30,  30);

    addFlow(diag, init,       start,      "");
    addFlow(diag, start,      enter,      "");
    addFlow(diag, enter,      decEnter,   "");
    addFlow(diag, decEnter,   exit_ev,    "[нет]");
    addFlow(diag, decEnter,   decPlaying, "[да]");
    addFlow(diag, decPlaying, exit_ev,    "[да - уже играет]");
    addFlow(diag, decPlaying, play,       "[нет]");
    addFlow(diag, play,       exit_ev,    "");
    addFlow(diag, exit_ev,    decExit,    "");
    addFlow(diag, decExit,    fin,        "[нет]");
    addFlow(diag, decExit,    decStopped, "[да]");
    addFlow(diag, decStopped, fin,        "[да - уже остановлен]");
    addFlow(diag, decStopped, stop,       "[нет]");
    addFlow(diag, stop,       decActive,  "");
    addFlow(diag, decActive,  coroutine,  "[да]");
    addFlow(diag, decActive,  stopImmed,  "[нет]");
    addFlow(diag, coroutine,  fin,        "");
    addFlow(diag, stopImmed,  fin,        "");

    repo.ReloadDiagram(diag.DiagramID);
    Session.Output("  Created: Area Ambient Sound");
}

// ─────────────────────────────────────────────────────────────────────────────
// 4. UI Audio Auto Installer
// ─────────────────────────────────────────────────────────────────────────────
function buildUIAudioAutoInstaller(root) {
    var pkg  = findOrCreatePackage(root, "UI Audio Auto Installer");
    var diag = createActivityDiagram(pkg, "UI Audio Auto Installer");

    var W = 230; var H = 40; var X = 100;

    var init       = addElement(pkg, "",                                                          "ActivityInitial");
    var start      = addElement(pkg, "Start: запустить корутину InitializeWithDelay()",           "Action");
    var wait       = addElement(pkg, "yield WaitForSeconds(0.1f)",                               "Action");
    var getAM      = addElement(pkg, "ServiceLocator.GetService() -> audioManager",              "Action");
    var decAM      = addElement(pkg, "audioManager == null?",                                    "Decision");
    var warnAM     = addElement(pkg, "LogWarning: AudioManager not found",                       "Action");
    var checkSound = addElement(pkg, "TryGetSource(clickSoundName)",                             "Action");
    var decSound   = addElement(pkg, "Звук зарегистрирован (AudioError.OK)?",                   "Decision");
    var warnSound  = addElement(pkg, "LogWarning: Sound not registered",                         "Action");
    var getButtons = addElement(pkg, "GetComponentsInChildren<Button>(includeInactive=true)",    "Action");
    var bind       = addElement(pkg, "Для каждой кнопки:\nbtn.onClick.AddListener(PlaySound)",   "Action");
    var play       = addElement(pkg, "[Runtime] PlaySound:\naudioManager.Play(clickSoundName, ChildType.PARENT)", "Action");
    var fin        = addElement(pkg, "",                                                          "ActivityFinal");

    var Y = 20;
    addToDiagram(diag, init,       X+90, Y,      30,  30); Y += 60;
    addToDiagram(diag, start,      X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, wait,       X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, getAM,      X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, decAM,      X+65, Y,       100, 40); Y += 70;
    addToDiagram(diag, warnAM,     X+280,Y,       W,   H);
    addToDiagram(diag, checkSound, X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, decSound,   X+65, Y,       100, 40); Y += 70;
    addToDiagram(diag, warnSound,  X+280,Y,       W,   H);
    addToDiagram(diag, getButtons, X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, bind,       X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, play,       X,    Y,       W,   H); Y += 70;
    addToDiagram(diag, fin,        X+90, Y,       30,  30);

    addFlow(diag, init,       start,      "");
    addFlow(diag, start,      wait,       "");
    addFlow(diag, wait,       getAM,      "");
    addFlow(diag, getAM,      decAM,      "");
    addFlow(diag, decAM,      warnAM,     "[да - null]");
    addFlow(diag, decAM,      checkSound, "[нет]");
    addFlow(diag, warnAM,     fin,        "");
    addFlow(diag, checkSound, decSound,   "");
    addFlow(diag, decSound,   warnSound,  "[нет - не найден]");
    addFlow(diag, decSound,   getButtons, "[да - OK]");
    addFlow(diag, warnSound,  fin,        "");
    addFlow(diag, getButtons, bind,       "");
    addFlow(diag, bind,       play,       "[клик по кнопке]");
    addFlow(diag, bind,       fin,        "");
    addFlow(diag, play,       fin,        "");

    repo.ReloadDiagram(diag.DiagramID);
    Session.Output("  Created: UI Audio Auto Installer");
}
