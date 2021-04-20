using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    internal class ListCountryPopupViewModel : EntityListViewModel<Country>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public override string Title => "{Country}";
        protected override void Configure()
        {
            Columns.Configure(c => c
                .Column
                    .Header("{Name}").Width(150)
                    .Content(s => s.Name)
                    .Localize()
                    .OrderByOrder(0)
                .Column
                    .Header("{Flag}").Width(60)
                    .Icon(s => s.IconPath)
            );

            Filter<TextFilter>(f => f.Title = "{Name}")
            .Link(List, s => s.Name);

            //Filters.Add(new EntityFilterViewModel<Customer,Country>().Configure(
            //    "Country",
            //    "Pays",
            //    c => c.Country,List
            //    ));
            List.Update();
        }
    }
}