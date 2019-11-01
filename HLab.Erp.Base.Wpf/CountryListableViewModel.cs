using System;
using System.Collections.Generic;
using System.Text;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Mvvm;

namespace HLab.Erp.Base.Wpf
{
    public class CountryListableViewModel : ViewModel<CountryListableViewModel,Country>, IListableModel
    {
        public string Caption => Model.Name;
        public string IconPath => Model.IconPath;
    }
}
