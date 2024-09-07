using System;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Units;

public class UnitListViewModel : Core.EntityLists.EntityListViewModel<Unit>, IMvvmContextProvider
{
    public class Bootloader : ParamBootloader
    { }

    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }

    readonly IAclService _acl;

    protected override bool AddCanExecute(Action<string> errorAction) => _acl.IsGranted(errorAction, ErpRights.ErpSignCustomer);
    protected override bool DeleteCanExecute(Unit unit, Action<string> errorAction) =>  _acl.IsGranted(errorAction, ErpRights.ErpSignCustomer);

    public UnitListViewModel(IAclService acl, Injector i) : base(i, c => c
        .Column("Name")
        .Header("{Name}") 
        .OrderByAsc(0)
        .Link(s => s.Name)
        .Filter()

        .Column("Symbol")
        .Header("{Symbol}") 
        .Link(s => s.Symbol)
        .Filter()
        .Icon(s => s.IconPath)
    )
    {
        _acl = acl;
    }
}