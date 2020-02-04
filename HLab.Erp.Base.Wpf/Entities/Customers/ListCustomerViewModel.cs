using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public class ListCustomerViewModel : EntityListViewModel<ListCustomerViewModel,Customer>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public string Title => "{Customers}";

        public ListCustomerViewModel()
        {
            AddAllowed = true;
            DeleteAllowed = true;

            Columns
                .Column("{Name}", s => s.Name)
                .Column("{Country}", s => new ViewLocator{Model = s.Country})
                .Column("{eMail}", s => s.Email)
                .Column("{Address}", s => s.Address);

            List.OrderBy = e => e.Name;

            using (List.Suspender.Get())
            {
                Filters.Add(new FilterTextViewModel {Title = "{Name}"}.Link(List, e => e.Name));
            }

            //Filters.Add(new EntityFilterViewModel<Customer,Country>().Configure(
            //    "Country",
            //    "Pays",
            //    c => c.Country,List
            //    ));
            List.UpdateAsync();
        }


    }
}
