using System.ComponentModel.DataAnnotations.Schema;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using ReactiveUI;

namespace HLab.Erp.Acl;

public class AclList : Entity
{
    public AclList()
    {

    }

    [Column]
    public string Target
    {
        get => _target;
        set => this.RaiseAndSetIfChanged(ref _target, value);
    }
    string _target = "";

    [Column]
    public string ParentId
    {
        get => _parentId;
        set => this.RaiseAndSetIfChanged(ref _parentId, value);
    }

    string _parentId = "";

    [NotMapped]
    public ObservableQuery<AclGranted> Granted
    {
        get => _granted;
        set => this.RaiseAndSetIfChanged(ref _granted, value.AddFilter("", ()=>n => n.ToNodeId == Id)
                .FluentUpdate());
    }
    ObservableQuery<AclGranted> _granted;

    //[TriggerOn(nameof(ParentId))]
    //public AclList Parent
    //{
    //    get => E.GetForeign<AclList>(ParentId); set => ParentId = value.Id;
    //}

    [Column]
    public bool Inherit
    {
        get => _inherit;
        set => this.RaiseAndSetIfChanged(ref _inherit, value);
    }

    bool _inherit = true;

}
