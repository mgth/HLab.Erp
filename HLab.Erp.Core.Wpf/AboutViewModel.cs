using System;
using System.Collections.Generic;
using System.Text;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core
{
    public class AboutViewModel : N<AboutViewModel>
    {
        private IErpServices _erp;
        public string Note
        {
            get => _note.Get();
            set => _note.Set(value);
        }
        private readonly IProperty<string> _note = H.Property<string>();

//        public object Icon => _erp.


        public AboutViewModel(IErpServices erp)
        {
            _erp = erp;
        }
    }
}
