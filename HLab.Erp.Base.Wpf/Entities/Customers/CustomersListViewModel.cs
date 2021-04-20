using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public class CustomersListViewModel : EntityListViewModel<Customer>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        protected override void Configure()
        {
            AddAllowed = true;
            DeleteAllowed = true;

            Columns.Configure(c => c
                .Column.Header("{Name}").Content(s => s.Name).OrderByOrder(0)
                .Column.Header("{Country}").Content(s => new ViewLocator{Model = s.Country})
                .Column.Header("{eMail}").Content(s => s.Email)
                .Column.Header("{Address}").Content(s => s.Address)
            );

            using (List.Suspender.Get())
            {
                Filter<TextFilter>( f => f.Title("{Name}").Link(List, e => e.Name));
            }

            //Filters.Add(new EntityFilterViewModel<Customer,Country>().Configure(
            //    "Country",
            //    "Pays",
            //    c => c.Country,List
            //    ));
            List.Update();
        }
    }
}
