using System.Collections.Generic;
using UnityEngine;
using Zenject;

/// <summary>
/// Управляет набором членов команды в UI инвентаря.
/// </summary>
public class InventoryTeam : MonoBehaviour
{
    [SerializeField] private GameObject _memberPrefab;
    [SerializeField] private Transform _membersContainer;
    
    private readonly List<Member> _members = new List<Member>();
    
    /// <summary>
    /// Возвращает список текущих членов команды только для чтения.
    /// </summary>
    public IReadOnlyList<Member> Members => _members;

    private DiContainer _container;

    [Inject]
    private void Construct(DiContainer container)
    {
        _container = container;
    }

    private void Start()
    {
        // Пример: добавляем члена команды по умолчанию при старте
        if (_memberPrefab != null && _membersContainer != null)
        {
            AddMember();
        }
    }

    /// <summary>
    /// Создает нового члена команды из префаба и добавляет его в команду.
    /// </summary>
    public void AddMember()
    {
        if (_memberPrefab == null) return;

        var memberInstance = _container.InstantiatePrefab(_memberPrefab, _membersContainer);
        var newMember = memberInstance.GetComponent<Member>();

        if (newMember != null)
        {
            _members.Add(newMember);
        }
        else
        {
            Debug.LogError("[InventoryTeam] The instantiated member prefab does not have a 'Member' component.");
            Destroy(memberInstance);
        }
    }
}
