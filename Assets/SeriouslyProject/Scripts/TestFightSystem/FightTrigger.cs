using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using FightSystem.Data;
using UnityEngine;
using System;
using System.IO;

[RequireComponent(typeof(Collider2D))]
public class FightTrigger : MonoBehaviour
{
    [Header("FightSettings")]
    [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = true)]
    [SerializeField] private List<EnemyesSettings> enemies = new List<EnemyesSettings>();

    private const string SavePath = "enemies_data.json"; // <-- верно

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ISceneLoader>(out var sceneLoader))
        {
            EnterTrigger();
        }
    }

    private void EnterTrigger()
    {
        SaveEnemiesToFile(); // Сохраняем врагов перед переходом
        SceneManager.LoadScene("TestScene");
    }

    [Button("Сохранить врагов в файл")]
    private void SaveEnemiesToFile()
    {
        FightData data = new FightData { enemies = this.enemies };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(Path.Combine(Application.persistentDataPath, SavePath), json);
        Debug.Log($"[FightTrigger] Враги сохранены: {Path.Combine(Application.persistentDataPath, SavePath)}");
    }


    //[Button("Загрузить врагов из файла")]
    //private void LoadEnemiesFromFile()
    //{
    //    string path = Path.Combine(Application.persistentDataPath, SavePath);
    //    if (File.Exists(path))
    //    {
    //        string json = File.ReadAllText(path);
    //        EnemyListWrapper wrapper = JsonUtility.FromJson<EnemyListWrapper>(json);
    //        this.enemies = wrapper.enemies;
    //        Debug.Log($"[FightTrigger] Враги загружены: {this.enemies.Count} шт.");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("[FightTrigger] Файл врагов не найден!");
    //    }
    //}

    [Serializable]
    private class EnemyListWrapper
    {
        public List<EnemyesSettings> enemies;
    }
}

[Serializable]
public class EnemyesSettings
{
    [Space(1)]
    [LabelWidth(200)]
    [LabelText("Использовать шаблон врага")]
    public bool useEnemyData = true;
    [Space(1)]
    [LabelWidth(200)]
    [LabelText("Имя шаблона врага (ресурсы)")]
    public string enemyDataName;

    [ShowIf("useEnemyData")]
    [LabelText("Шаблон врага")]
    [InlineEditor(InlineEditorModes.GUIOnly)]
    public EnemyData enemyData;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public string _name;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")][TextArea(3, 10)] public string _description;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _damage;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _priority;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _maxMana;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _mana;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _maxHealth;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _health;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _heal;
    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public int _armor;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [LabelText("Путь до спрайта (Resources)")]
    public string spritePath;

    public Sprite GetSprite()
    {
        if (string.IsNullOrEmpty(spritePath)) return null;
        return Resources.Load<Sprite>(spritePath);
    }

    public EnemyData GetEnemyData()
    {
        if (!useEnemyData || string.IsNullOrEmpty(enemyDataName))
            return null;

        return Resources.Load<EnemyData>("EnemyData/"+enemyDataName);
    }
}
