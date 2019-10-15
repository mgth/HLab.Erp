using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HLab.Erp.Base.Data;
using HLab.Mvvm;

namespace HLab.Erp.Base.Wpf
{
    public class CustomerViewModel : ViewModel<CustomerViewModel,Customer>
    {
        public string Title => Model.Name;
    }
}
