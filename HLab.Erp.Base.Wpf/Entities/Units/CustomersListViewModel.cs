using System;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Core.ListFilterConfigurators;
using HLab.Erp.Core.Wpf.EntityLists;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf.Entities.Units
{
    public class UnitClassPopupListViewModel : EntityListViewModel<UnitClass>, IMvvmContextProvider
    {
        public class Bootloader : NestedBootloader
        { }

        public void ConfigureMvvmContext(IMvvmContext ctx)
        {
        }
        protected override bool CanExecuteAdd(Action<string> errorAction) => Erp.Acl.IsGranted(errorAction, ErpRights.ErpSignCustomer);
        protected override bool CanExecuteDelete(UnitClass customer, Action<string> errorAction) =>  Erp.Acl.IsGranted(errorAction, ErpRights.ErpSignCustomer);

        public UnitClassPopupListViewModel() : base(c => c
            .Column("Name")
                .Header("{Name}") 
//                .OrderByOrder(0)
                .Link(s => s.Name)
                .Filter()

            .Column("Symbol")
            .Header("{Symbol}") 
//                .OrderByOrder(0)
            .Link(s => s.Symbol)
            .Filter()
                .Icon(s => s.IconPath)
        )
        {
        }
    }
}
