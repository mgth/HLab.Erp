using Grace.DependencyInjection.Attributes;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.ListFilters;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    public class CountriesListViewModel : EntityListViewModel<Country>, IMvvmContextProvider
    {
        public CountriesListViewModel(IErpServices erp) : base(c => ColumnConfiguratorExtension.Content(c
                .Header("{Country}")
                .Column()
                .Header("{Name}")
                .Link(s => s.Name)
                .Localize()
// TODO                .OrderByOrder(0)
                .Filter()
                .PostLink(s => erp.Localization.LocalizeAsync(s.Name).Result)
                .Column()
                .Header("{A2 Code}")
                .Link(s => s.IsoA2)
                .Filter()
                .Column()
                .Header("{A3 Code}")
                .Link(s => s.IsoA3)
                .Filter()
                .Column()
                .Header("{Code}")
                .Content(s => s.Iso)
                //.Filter()
                .Column()
                .Header("{Continent}"), s => s.Continent.Name)
                .Localize()
            .Column()
                .Header("{Flag}")
                .Icon( s =>  s.IconPath, 50)
// TODO                .OrderBy(s => s.Name)
            )
        {
        }
    }
}