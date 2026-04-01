using EchoRift.EchoRiftSaveLoadSystem;
using static EchoRift.EchoRiftSaveLoadSystem.SaveFileNames;
using PixelCrushers.DialogueSystem;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ChangeNameDialogueActor : MonoBehaviour
{
    [SerializeField] private string actorDatabaseName = "Player";

    private string _cachedName;

    public void SaveNameForDialogueActor(string newName)
    {
        if (string.IsNullOrEmpty(newName)) return;

        _cachedName = newName;
    }

    public void SaveNameForDialogueActor(string newName, bool isApply)
    {
        if (string.IsNullOrEmpty(newName)) return;

        _cachedName = newName;

        ApplyName();
    }

    public void ApplyName()
    {
        DialogueLua.SetActorField(actorDatabaseName, "Name", _cachedName);
        DialogueLua.SetActorField(actorDatabaseName, "Display Name", _cachedName);

        var playerNameData = new PLayerNameData
        {
            playerDialogueName = _cachedName
        };

        DialogueLua.SetVariable("PlayerName", _cachedName);
        SaveLoadSystem.Save(PLAYER_NAME, playerNameData, GAME_DIRECTORY);
    }

    [SerializeField]
    public class PLayerNameData
    {
        public string playerDialogueName;
    }
}