using HLab.Base.ReactiveUI;
using HLab.Mvvm.ReactiveUI;

namespace HLab.Erp.Core.Wpf.Localization
{
    internal class TagViewModel : ViewModel
    {
        public string Value
        {
            get => _value;
            set => this.SetAndRaise(ref _value,value);
        }

        string _value;

    }
}
