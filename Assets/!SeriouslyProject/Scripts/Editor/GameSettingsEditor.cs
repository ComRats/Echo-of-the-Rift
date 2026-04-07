using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(GameSettings))]
public class GameSettingsEditor : Editor
{
    // Все буквенные и цифровые клавиши для быстрого заполнения
    private static readonly KeyCode[] CommonKeys =
    {
        KeyCode.A, KeyCode.B, KeyCode.C, KeyCode.D, KeyCode.E, KeyCode.F,
        KeyCode.G, KeyCode.H, KeyCode.I, KeyCode.J, KeyCode.K, KeyCode.L,
        KeyCode.M, KeyCode.N, KeyCode.O, KeyCode.P, KeyCode.Q, KeyCode.R,
        KeyCode.S, KeyCode.T, KeyCode.U, KeyCode.V, KeyCode.W, KeyCode.X,
        KeyCode.Y, KeyCode.Z,
        KeyCode.Alpha0, KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3,
        KeyCode.Alpha4, KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7,
        KeyCode.Alpha8, KeyCode.Alpha9,
    };

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EditorGUILayout.Space();
        EditorGUILayout.HelpBox(
            "Заполни keySpriteMap вручную: каждая клавиша → индекс спрайта в SpriteCollection.\n" +
            "Кнопка ниже добавит все буквы/цифры с индексом 0 — потом поменяй нужные.",
            MessageType.Info);

        if (GUILayout.Button("Заполнить все буквы/цифры (индекс 0)"))
        {
            var settings = (GameSettings)target;
            Undo.RecordObject(settings, "Fill KeySpriteMap");

            foreach (var key in CommonKeys)
            {
                bool exists = false;
                foreach (var entry in settings.keySpriteMap)
                    if (entry.key == key) { exists = true; break; }

                if (!exists)
                    settings.keySpriteMap.Add(new GameSettings.KeySpriteEntry { key = key, spriteIndex = 0 });
            }

            EditorUtility.SetDirty(settings);
        }
    }
}
