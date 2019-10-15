using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf
{
    class TagViewModel : ViewModel<TagViewModel>
    {
        public string Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }
        private readonly IProperty<string> _value = H.Property<string>();

    }
}
