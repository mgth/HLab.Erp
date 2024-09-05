using HLab.Erp.Data;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class AclGranted : Entity
{
    AclGranted()
    {
        _right = Foreign(this, e => e.RightId, e => e.Right);
        _onNode = Foreign(this, e => e.OnNodeId, e => e.OnNode);
        _toNode = Foreign(this, e => e.ToNodeId, e => e.ToNode);
    }
    public bool Deny
    {
        get => _aclGranted;
        set => this.RaiseAndSetIfChanged(ref _aclGranted, value);
    }
    bool _aclGranted;

    public int? RightId
    {
        get => _right.Id;
        set => _right.SetId(value);
    }
    public AclRight Right
    {
        get => _right.Value;
        set => RightId = value.Id;
    }
    readonly ForeignPropertyHelper<AclGranted, AclRight> _right;


    public int? ToNodeId
    {
        get => _toNode.Id;
        set => _toNode.SetId(value);
    }
    public AclNode ToNode
    {
        get => _toNode.Value;
        set => ToNodeId = value.Id;
    }
    readonly ForeignPropertyHelper<AclGranted, AclNode> _toNode;

    public int? OnNodeId
    {
        get => _onNode.Id;
        set => _onNode.SetId(value);
    }
    [Ignore]
    public AclNode OnNode
    {
        get => _onNode.Value;
        set => OnNodeId = value.Id;
    }
    readonly ForeignPropertyHelper<AclGranted, AclNode> _onNode;
}
