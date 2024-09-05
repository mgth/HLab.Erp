
using HLab.Core.ReactiveUI;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class AclLink : Entity
{
    public AclLink() 
    { 
        _group = Foreign(this, e => e.GroupId, e => e.Group);
        _member = Foreign(this, e => e.MemberId, e => e.Member);
    }

    public int? GroupId
    {
        get => _groupId;
        set => this.RaiseAndSetIfChanged(ref _groupId,value);
    }
    int? _groupId;

    [Ignore]
    public AclNode Group
    {
        get => _group.Value;
        set => GroupId = value.Id;
    }
    readonly ObservableAsPropertyHelper<AclNode> _group;

    public int? MemberId
    {
        get => _memberId;
        set => this.RaiseAndSetIfChanged(ref _memberId, value);
    }
    int? _memberId;

    [Ignore]
    public AclNode Member
    {
        get => _member.Value;
        set => _memberId = value.Id;
    }
    readonly ObservableAsPropertyHelper<AclNode>_member;
}