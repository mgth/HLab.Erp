
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
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
// TODO             .AddAllowed()
//TODO             .DeleteAllowed()
            .Column()
                .Header("{Name}")
//                .OrderByOrder(0)
                .Link(s => s.Name)
                .Filter()

            .Column()
                .Header("{Country}")
                .Content(s => s.Country).Mvvm()
                .Link(s => s.Country)
                .Filter()
                //.Icon(s => s.IconPath)
// TODO                
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
