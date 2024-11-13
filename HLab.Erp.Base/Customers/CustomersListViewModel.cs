using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Customers;

public class CustomersListViewModel(IAclService acl, EntityListViewModel<Customer>.Injector i)
    : EntityListViewModel<Customer>(i, c => c
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
        .Filter()), IMvvmContextProvider
{
    public class Bootloader : NestedBootloader
    { }
    public void ConfigureMvvmContext(IMvvmContext ctx)
    {
    }

    protected override bool AddCanExecute(Action<string> errorAction) => acl?.IsGranted(errorAction, ErpRights.ErpSignCustomer)??false;
    protected override bool DeleteCanExecute(Customer customer, Action<string> errorAction) =>  acl?.IsGranted(errorAction, ErpRights.ErpSignCustomer) ?? false;
}