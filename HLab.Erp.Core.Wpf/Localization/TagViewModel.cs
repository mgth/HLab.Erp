using HLab.Mvvm.ReactiveUI;

namespace HLab.Erp.Core.Wpf.Localization
{
    internal class TagViewModel : ViewModel
    {
        public string Value
        {
            get => _value;
            set => SetAndRaise(ref _value,value);
        }

        string _value;

    }
}
