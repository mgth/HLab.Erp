using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    class ListCountryPopupViewModel : EntityListViewModel<Country>, IMvvmContextProvider
    {
        [Import]
        private ILocalizationService _localization;

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public string Title => "{Country}";

        public ListCountryPopupViewModel()
        {
            Columns.Configure(c => c
                .Column.Header("{Name}").Content(s => s.Name).Localize().OrderByOrder(0)
                .Column.Header("{Flag}").Icon(s => s.IconPath)
            );

            Filter<TextFilter>(f => f.Title = "{Name}")
            .Link(List, s => s.Name);

            //Filters.Add(new EntityFilterViewModel<Customer,Country>().Configure(
            //    "Country",
            //    "Pays",
            //    c => c.Country,List
            //    ));
            List.UpdateAsync();
        }
    }
}