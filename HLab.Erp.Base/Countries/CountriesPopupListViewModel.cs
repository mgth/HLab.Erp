
using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;

namespace HLab.Erp.Base.Wpf.Entities.Countries;

public class CountriesPopupListViewModel(EntityListViewModel<Country>.Injector i)
    : EntityListViewModel<Country>(i, c => c
        .HideMenu()
        .Header("{Country}")
        .Column("Name")
        .Header("{Name}")
        .Localize(s => s.Name)
        .Link(s => s.Name)
        .Filter()
        .PostLink(s => i.Localization.Localize(s.Name))
        .Column("Flag")
        .Header("{Flag}")
        .Icon(s => s.IconPath, 50))
{
    // TODO                .OrderByOrder(0)
    //.Column()
    //.Header("{Continent}")
    //.Link(e => e.Continent)
    //       .Filter()
}