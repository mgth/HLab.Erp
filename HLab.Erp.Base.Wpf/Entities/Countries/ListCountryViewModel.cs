using Grace.DependencyInjection.Attributes;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    public class CountriesListViewModel : EntityListViewModel<Country>, IMvvmContextProvider
    {
        public CountriesListViewModel(IErpServices erp) : base(c => c
            .Header("{Country}")
            .Column()
                .Header("{Name}")
                .Content(s => s.Name)
                .Localize()
                .OrderByOrder(0)
                    .Filter<TextFilter>()
                    .PostLink(s => erp.Localization.LocalizeAsync(s.Name).Result)
            .Column()
                .Header("{A2 Code}")
                .Content(s => s.IsoA2)
            .Column()
                .Header("{A3 Code}")
                .Content(s => s.IsoA3)
            .Column()
                .Header("{Code}")
                .Content(s => s.Iso)
            .Column()
                .Header("{Continent}")
                .Content(s => s.Continent.Name)
                .Localize()
            .Column()
                .Header("{Flag}")
                .Icon( s =>  s.IconPath, 50)
                .OrderBy(s => s.Name)
            )
        {
        }
    }
}