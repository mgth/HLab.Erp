
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Annotations;
using System;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    public class CountriesListViewModel : EntityListViewModel<Country>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        protected override bool CanExecuteExport(Action<string> errorAction) => Erp.Acl.IsGranted(ErpRights.ErpManageCountries);
        protected override bool CanExecuteImport(Action<string> errorAction) => Erp.Acl.IsGranted(ErpRights.ErpManageCountries);

        public CountriesListViewModel(ILocalizationService localization) : base(c => c
                .Header("{Country}")
                .Column("Name")
                    .Header("{Name}")
                    .Content(s => s.Name).Localize()
                    .Link(s => s.Name)
                    
                    // TODO                .OrderByOrder(0)
                    .Filter()
                    .PostLink(s => localization.Localize(s.Name))

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
                    //.Filter()

                .Column("Continent")
                    .Header("{Continent}")
                    .Content(s => s.Continent.Name)
                    .Localize()

                .Column("Flag")
                    .Header("{Flag}")
                    .Icon(s => s.IconPath, 50)
            // TODO                .OrderBy(s => s.Name)
            )
        {
        }
    }
}