using System.Collections.Generic;
using UnityEngine;

public class InventoryTeam : MonoBehaviour
{
    [SerializeField] private List<Member> members;
    [SerializeField] private Member member;

    private void Awake()
    {
        if (members == null)
            members = new List<Member>();
    }

    private void Start()
    {
        AddMember();
    }

    private void AddMember()
    {
        GameObject newMemberObj = Instantiate(member.gameObject, this.transform);

        Member currentMember = newMemberObj.GetComponent<Member>();
        members.Add(currentMember);

    }

}
