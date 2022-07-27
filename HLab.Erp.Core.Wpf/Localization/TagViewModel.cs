using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.Localization
{
    internal class TagViewModel : ViewModel
    {
        public TagViewModel() => H<TagViewModel>.Initialize(this);

        public string Value
        {
            get => _value.Get();
            set => _value.Set(value);
        }

        readonly IProperty<string> _value = H<TagViewModel>.Property<string>();

    }
}
