using System;
using System.Linq;
using System.Threading.Tasks;
using HLab.Erp.Base.Data;
using HLab.Erp.Core.EntityLists;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;


namespace HLab.Erp.Core.Wpf.Localization
{
    public class LocalizeEntriesListViewModel(EntityListViewModel<LocalizeEntry>.Injector i)
        : EntityListViewModel<LocalizeEntry>(i, c => c
            .Header("{Localization}")
            .Column("Todo")
            .Header("{Todo}")
            .Content(s => s.Todo ? "X" : "-")
            .Link(s => s.Todo)
            .Filter()
            .Column("Custom")
            .Header("{Custom}")
            .Content(s => s.Todo ? "X" : "-")
            .Link(s => s.Custom)
            .Filter()
            .Column("Tag")
            .Header("{Tag}")
            .Link(s => s.Tag)
            .Filter()
            .Column("Code")
            .Header("{Code}")
            .Link(s => s.Code)
            .Filter()
            .Column("Value")
            .Header("{Value}")
            .Link(s => s.Value)
            .Filter()
        ), IMvvmContextProvider
    {
        public class Bootloader : ParamBootloader { }

        protected override bool ExportCanExecute(Action<string> errorAction) => true; // Erp.Acl.IsGranted(ErpRights.ErpManageCountries);
        protected override bool ImportCanExecute(Action<string> errorAction) => true; // Erp.Acl.IsGranted(ErpRights.ErpManageCountries);

        protected override bool DeleteCanExecute(LocalizeEntry arg, Action<string> errorAction) => true;

        //.PostLink(s => erp.Localization.Localize(s.Name))

        protected override async Task ImportAsync(IDataService data, LocalizeEntry importValue)
        {
            var e = await data.FetchOneAsync<LocalizeEntry>(e =>
                e.Tag == importValue.Tag && e.Code == importValue.Code && e.Custom == importValue.Custom);

            try
            {
                if (e != null)
                {
                    if (e.Value == importValue.Value && e.Todo == false) return;

                    e.Value = importValue.Value;
                    e.Todo = false;
                    e.Custom = importValue.Custom;
                    await data.UpdateAsync(e, "Value", "Todo", "Custom");
                }
                else
                {
                    await data.AddAsync<LocalizeEntry>(i =>
                    {
                        i.Tag = importValue.Tag;
                        i.Code = importValue.Code;
                        i.Value = importValue.Value;
                        i.Todo = false;
                        i.Custom = importValue.Custom;
                    });

                }

            }
            catch (Exception ex)
            {

            }
        }
    }

}