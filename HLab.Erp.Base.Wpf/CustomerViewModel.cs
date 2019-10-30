using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Mvvm.Annotations;

namespace HLab.Erp.Base.Wpf
{
    public class CustomerViewModel : EntityViewModel<CustomerViewModel,Customer>
    {
         private readonly IIconService _iconService;

        [Import] public CustomerViewModel(ObservableQuery<Country> countries, IIconService iconService)
        {
            Countries = countries;
            _iconService = iconService;
            countries.Update();
        }

        public string Title => Model.Name;

        public object Icon => _iconService.GetIcon(Model.IconPath);

       
        public ObservableQuery<Country> Countries {get;}

    }
}
