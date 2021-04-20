using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    class ListCountryViewModel : EntityListViewModel<Country>, IMvvmContextProvider
    {
        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }

        public override string Title => "{Country}";
        protected override void Configure()
        {
            Columns.Configure(c => c
                .Column.Header("{Name}").Content(s => s.Name).Localize().OrderByOrder(0)
                .Column.Header("{A2 Code}").Content(s => s.IsoA2)
                .Column.Header("{A3 Code}").Content(s => s.IsoA3)
                .Column.Header("{Code}").Content(s => s.Iso)
                .Column.Header("{Continent}").Content(s => s.Continent.Name).Localize()
                .Column.Header("{Flag}").Icon( s =>  s.IconPath, 50).OrderBy(s => s.Name)
            );

            Filter<TextFilter>(f => f.Title("{Name}"))
                .PostLink(List, s =>  Erp.Localization.LocalizeAsync(s.Name).Result);

            //Filters.Add(new EntityFilterViewModel<Continent>()
            //{
            //    Title = "{Continent}",
            //}.Link(List, s => s.Continent));

            List.Update();
        }
    }
}