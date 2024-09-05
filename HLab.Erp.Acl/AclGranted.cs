using HLab.Core.ReactiveUI;
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
        get => _rightId;
        set => this.RaiseAndSetIfChanged(ref _rightId,value);
    }
    int? _rightId;

    [Ignore]
    public AclRight Right
    {
        get => _right.Value;
        set => RightId = value.Id;
    }
    readonly ObservableAsPropertyHelper<AclRight> _right;


    public int? ToNodeId
    {
        get => _toNodeId;
        set => this.RaiseAndSetIfChanged(ref _toNodeId, value);
    }
    int? _toNodeId;

    [Ignore]
     public AclNode ToNode
     {
        get => _toNode.Value;
        set => ToNodeId = value.Id;
     }

    readonly ObservableAsPropertyHelper<AclNode> _toNode;

    public int? OnNodeId
    {
        get => _onNodeId;
        set => this.RaiseAndSetIfChanged(ref _onNodeId, value);
    }
    int? _onNodeId;

    [Ignore]
     public AclNode OnNode
     {
        get => _onNode.Value;
        set => OnNodeId = value.Id;
     }
     readonly ObservableAsPropertyHelper<AclNode> _onNode;
}
