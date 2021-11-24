
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Annotations;
using System;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public class CustomersListViewModel : EntityListViewModel<Customer>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        { }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
        protected override bool CanExecuteAdd(Action<string> errorAction) => Erp.Acl.IsGranted(errorAction, ErpRights.ErpSignCustomer);
        protected override bool CanExecuteDelete(Customer customer, Action<string> errorAction) =>  Erp.Acl.IsGranted(errorAction, ErpRights.ErpSignCustomer);

        public CustomersListViewModel() : base(c => c
            .Column("Name")
                .Header("{Name}") 
//                .OrderByOrder(0)
                .Link(s => s.Name)
                .Filter()

            .Column("Country")
                .Header("{Country}")
                .Content(s => s.Country).Mvvm()
                .Link(s => s.Country)
                .Filter()
                //.Icon(s => s.IconPath)
// TODO                
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
        }
    }
}
