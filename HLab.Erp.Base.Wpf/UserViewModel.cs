using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Base.Data;
using HLab.Mvvm;

namespace HLab.Erp.Base.Wpf
{
    class UserViewModel: ViewModel<CustomerViewModel,Customer>
    {
        public string Title => Model.Name;
    }
}
