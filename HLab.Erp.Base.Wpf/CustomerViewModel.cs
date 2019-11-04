using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf
{
    public class CustomerViewModel : EntityViewModel<CustomerViewModel,Customer>
    {
         private readonly IIconService _iconService;
         public IErpServices Erp { get; }



        [Import] public CustomerViewModel(IIconService iconService, IErpServices erp)
        {
            _iconService = iconService;
            Erp = erp;
        }

        public string Title => _title.Get();
        private IProperty<string> _title = H.Property<string>(c => c
            .On(e => e.Model.Name)
            .On(e => e.Model.Id)
            //TODO : localize
            .Set(e => (e.Id < 0 && string.IsNullOrEmpty(e.Model.Name)) ? "Nouveau client" : e.Model.Name)
        );


        public object Icon => _iconService.GetIcon(Model.IconPath);

       
    }
}
