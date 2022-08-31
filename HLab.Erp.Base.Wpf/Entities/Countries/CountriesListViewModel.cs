
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Mvvm.Annotations;
using System;
using HLab.Erp.Acl;

namespace HLab.Erp.Base.Wpf.Entities.Countries
{
    public class CountriesListViewModel : Core.EntityLists.EntityListViewModel<Country>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        readonly IAclService _acl;
        protected override bool CanExecuteExport(Action<string> errorAction) => _acl.IsGranted(ErpRights.ErpManageCountries);
        protected override bool CanExecuteImport(Action<string> errorAction) => _acl.IsGranted(ErpRights.ErpManageCountries);

        public CountriesListViewModel(IAclService acl, Injector i) : base(i, c => c
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
                    //.Filter() TODO : int filter

                .Column("Continent")
                    .Header("{Continent}")
                    .Localize(s => s.Continent.Name)

                .Column("Flag")
                    .Header("{Flag}")
                    .Icon(s => s.IconPath, 50)
            // TODO                .OrderBy(s => s.Name)
            )
        {
            _acl = acl;
        }
    }
}