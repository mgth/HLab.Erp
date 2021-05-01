using Grace.DependencyInjection.Attributes;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public class CustomersListViewModel : EntityListViewModel<Customer>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        { }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public CustomersListViewModel() : base(c => c
            .AddAllowed()
            .DeleteAllowed()
            .Column()
                .Header("{Name}")
                .OrderByOrder(0)
                .Link(s => s.Name)
                .Filter()
            .Column()
                .Header("{Country}")
                .Content(s => s.Country)
                //.Icon(s => s.IconPath)
                .Mvvm()
            .Column()
                .Header("{eMail}")
                .Link(s => s.Email)
                .Filter()
            .Column()
                .Header("{Address}")
                .Link(s => s.Address)
                .Filter()
        )
        {
        }
    }
}
