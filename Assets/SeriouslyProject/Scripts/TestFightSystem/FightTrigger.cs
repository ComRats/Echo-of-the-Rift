using FightSystem.Data;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Drawing;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(Collider2D))]
public class FightTrigger : MonoBehaviour
{
    [Header("FightSettings")]
    [ListDrawerSettings(ShowIndexLabels = true, DraggableItems = true)]
    [SerializeField] private List<EnemySettings> enemies = new List<EnemySettings>();
    [SerializeField] private GameObject enemyesPrefab;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent<ISceneLoader>(out var sceneLoader))
        {
            EnterTrigger();

        }
    }

    private void EnterTrigger()
    {
        SceneManager.LoadScene("TestScene");
    }

}
[System.Serializable]
public class EnemySettings
{
    [PropertySpace(1)]
    [LabelWidth(186)]
    [LabelText("Использовать шаблон врага")]
    public bool useEnemyData = true;

    [ShowIf("useEnemyData")]
    [LabelText("Шаблон врага")]
    [InlineEditor(InlineEditorModes.GUIOnly)]
    public EnemyData enemyData;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public string _name;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")]
    [TextArea(3, 10)]
    public string _description;

    [HideIf("useEnemyData")]
    [FoldoutGroup("Параметры вручную")] public Sprite _sprite;

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
}

