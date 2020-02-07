using HLab.DependencyInjection.Annotations;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;
using HLab.Mvvm.Icons;
using HLab.Mvvm.Lang;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    class ListCountryViewModel : EntityListViewModel<ListCountryViewModel,Country>, IMvvmContextProvider
    {
        [Import]
        private IIconService _icons;
        [Import]
        private ILocalizationService _localize;

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public string Title => "{Country}";

        public ListCountryViewModel()
        {
            Columns
                .Column("{Name}", s => new Localize{Id=s.Name})
                .Column("{A2 Code}", s => s.IsoA2)
                .Column("{A3 Code}", s => s.IsoA3)
                .Column("{Code}", s => s.Iso)
                .Column("{Continent}", s => new Localize{Id = s.Continent.Name})
                .Column("{Flag}", s => new IconView{
                    MaxWidth = 50,
                    MinHeight = 50,
                    Path = s.IconPath
                });

            List.OrderBy = e => e.Name;

            Filters.Add(new FilterTextViewModel()
            {
                Title = "{Name}",
            }.PostLink(List, s =>  _localize.LocalizeAsync(s.Name).Result));

            //Filters.Add(new EntityFilterViewModel<Continent>()
            //{
            //    Title = "{Continent}",
            //}.Link(List, s => s.Continent));

            List.UpdateAsync();
        }
    }
}