using System;
using System.Linq;
using System.Threading.Tasks;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.Localization
{
    public class LocalizeEntriesListViewModel : EntityListViewModel<LocalizeEntry>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        {
            public override string MenuPath => "param";
        }

        protected override bool CanExecuteExport(Action<string> errorAction) => true; // Erp.Acl.IsGranted(ErpRights.ErpManageCountries);
        protected override bool CanExecuteImport(Action<string> errorAction) => true; // Erp.Acl.IsGranted(ErpRights.ErpManageCountries);

        protected override bool CanExecuteDelete(LocalizeEntry arg, Action<string> errorAction) => true;

        public LocalizeEntriesListViewModel(IErpServices erp) : base(c => c
                .Header("{Localization}")
                .Column()
                    .Header("{Todo}")
                    .Content(s => s.Todo ? "X" : "-")
                .Link(s => s.Todo)
                .Filter()
                //.PostLink(s => erp.Localization.Localize(s.Name))
                .Column()
                    .Header("{Custom}")
                    .Content(s => s.Todo ? "X" : "-")
                .Link(s => s.Custom)
                .Filter()

                .Column()
                    .Header("{Tag}")
                    .Link(s => s.Tag)
                    .Filter()

                .Column()
                    .Header("{Code}")
                    .Link(s => s.Code)
                    .Filter()

                .Column()
                    .Header("{Value}")
                    .Link(s => s.Value)
                    .Filter()

            )
        {
        }

        protected override async Task ImportAsync(IDataService data, LocalizeEntry newValue)
        {
            var e = await data.FetchOneAsync<LocalizeEntry>(e =>
                e.Tag == newValue.Tag && e.Code == newValue.Code && e.Custom == false &&
                (e.Value != newValue.Value || e.Todo));

            if(e != null)
            {
                e.Value = newValue.Value;
                e.Todo = false;
                e.Custom = newValue.Custom;
                await data.UpdateAsync(e, "Value", "Todo", "Custom");
            }
            else
            {
                await data.AddAsync<LocalizeEntry>(i =>
                {
                    i.Tag = newValue.Tag;
                    i.Value = newValue.Value;
                    i.Todo = false;
                    i.Custom = newValue.Custom;
                });
                
            }
        }
    }

}