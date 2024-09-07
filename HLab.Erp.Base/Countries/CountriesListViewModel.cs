using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Countries;

public class CountriesListViewModel(IAclService acl, EntityListViewModel<Country>.Injector i)
    : EntityListViewModel<Country>(i, c => c
        .Header("{Country}")
        .Column("Name")
        .Header("{Name}")
        .Localize(s => s.Name)
        .Link(s => s.Name)
        .OrderByAsc(0)
        .Filter()
        .PostLink(s => i.Localization.Localize(s.Name))
        .Column("A2Code")
        .Header("{A2 Code}")
        .Link(s => s.IsoA2)
        .Filter()
        .Column("A3Code")
        .Header("{A3 Code}")
        .Link(s => s.IsoA3)
        .Filter()
        .Column("Code")
        .Header("{Code}")
        .Content(s => s.Iso)
        .Column("Continent")
        .Header("{Continent}")
        .Localize(s => s.Continent.Name)
        .Column("Flag")
        .Header("{Flag}")
        .Icon(s => s.IconPath, 50)), IMvvmContextProvider
{
    public class Bootloader : NestedBootloader
    {
        public override string MenuPath => "param";
    }

    protected override bool ExportCanExecute(Action<string> errorAction) => acl.IsGranted(ErpRights.ErpManageCountries);
    protected override bool ImportCanExecute(Action<string> errorAction) => acl.IsGranted(ErpRights.ErpManageCountries);

    //.Filter() TODO : int filter
    // TODO                .OrderBy(s => s.Name)
}