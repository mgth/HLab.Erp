using HLab.Erp.Data;
using HLab.Erp.Data.foreigners;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class AclLink : Entity
{
    public AclLink() 
    { 
        _group = this.Foreign( e => e.GroupId, e => e.Group);
        _member = this.Foreign( e => e.MemberId, e => e.Member);
    }

    public int? GroupId
    {
        get => _group.Id;
        set => _group.SetId(value);
    }

    [Ignore]
    public AclNode Group
    {
        get => _group.Value;
        set => GroupId = value.Id;
    }
    readonly ForeignPropertyHelper<AclLink,AclNode> _group;

    public int? MemberId
    {
        get => _member.Id;
        set => _group.SetId(value);
    }

    [Ignore]
    public AclNode Member
    {
        get => _member.Value;
        set => MemberId = value.Id;
    }
    readonly ForeignPropertyHelper<AclLink,AclNode>_member;
}