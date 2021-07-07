
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

        public CountriesListViewModel(IErpServices erp) : base(c => c
                .Header("{Country}")
                .Column()
                    .Header("{Name}")
                    .Link(s => s.Name)
                    .Localize()
                    // TODO                .OrderByOrder(0)
                    .Filter()
                    .PostLink(s => erp.Localization.Localize(s.Name))

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
                    .Header("{Continent}")
                    .Content(s => s.Continent.Name)
                    .Localize()

                .Column()
                    .Header("{Flag}")
                    .Icon(s => s.IconPath, 50)
            // TODO                .OrderBy(s => s.Name)
            )
        {
        }
    }
}