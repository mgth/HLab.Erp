using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using NPoco;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class AclNode : Entity
{
    public AclNode()
    {

    }

    public string Name
    {
        get => _name;
        set => this.RaiseAndSetIfChanged(ref _name, value);
    }
    string _name = "";

    public string TargetClass
    {
        get => _targetClass;
        set => this.RaiseAndSetIfChanged(ref _targetClass, value);
    }
    string _targetClass = "";

    public int? TargetId
    {
        get => _targetId;
        set => this.RaiseAndSetIfChanged(ref _targetId, value);
    }
    int? _targetId;

    [Ignore]
    public ObservableQuery<AclLink> Members
    {
        get => _members;
        set => this.RaiseAndSetIfChanged(ref _members, value.AddFilter("", ()=>n => n.GroupId == Id)
                .FluentUpdate());
    }

    ObservableQuery<AclLink> _members;

    [Ignore]
    public ObservableQuery<AclLink> Groups
    {
        get => _groups;
        set => this.RaiseAndSetIfChanged(ref _groups, value.AddFilter("", ()=>n => n.MemberId == Id)
                .FluentUpdate());
    }
    ObservableQuery<AclLink> _groups;

    public bool Grant(AclRight right, AclNode target)
    {
        DataService.Add<AclGranted>(
            t =>
            {
                t.ToNodeId = Id;
                t.Right = right;
                t.OnNodeId = target.Id;
            }
        );

        return true;
    }

    public IEnumerable<int?> GetGroups()
    {
        foreach (var g in Groups)
        {
            yield return g.Group.Id;
            foreach (var gg in g.Group.GetGroups())
            {
                yield return gg;
            }
        }
    }

    public async Task<bool> IsGrantedAsync(AclRight right, AclNode target)
    {
        var groups = GetGroups().ToList();
        var targets = target.GetGroups().ToList();

        var grants = DataService.FetchWhereAsync<AclGranted>(e => 
                groups.Contains(e.ToNodeId) && 
                targets.Contains(e.OnNodeId)
            ,null);

        var granted = false;
        await foreach (var grant in grants)
        {
            if (grant.Deny) return false;
            granted = true;
        }

        return granted;
    }
}
