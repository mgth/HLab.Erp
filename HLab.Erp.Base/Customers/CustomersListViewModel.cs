
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Annotations;
using System;
using HLab.Erp.Acl;

namespace HLab.Erp.Base.Wpf.Entities.Customers;

public class CustomersListViewModel : Core.EntityLists.EntityListViewModel<Customer>, IMvvmContextProvider
{
    public class Bootloader : NestedBootloader
    { }
    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }

    readonly IAclService _acl;
    protected override bool AddCanExecute(Action<string> errorAction) => _acl.IsGranted(errorAction, ErpRights.ErpSignCustomer);
    protected override bool DeleteCanExecute(Customer customer, Action<string> errorAction) =>  _acl.IsGranted(errorAction, ErpRights.ErpSignCustomer);

    public CustomersListViewModel(IAclService acl, Injector i) : base( i, c => c
        .Column("Name")
        .Header("{Name}") 
        .OrderByAsc()
        .Link(s => s.Name)
        .Filter()

        .ColumnListable(s => s.Country)

        .Column("Email")
        .Header("{eMail}")
        .Link(s => s.Email)
        .Filter()

        .Column("Address")
        .Header("{Address}")
        .Link(s => s.Address)
        .Filter()
    )
    {
        _acl = acl;
    }
}