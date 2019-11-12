using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    public abstract class FilterViewModel<TClass> : ViewModel<TClass>, IFilterViewModel
    where TClass : FilterViewModel<TClass>
    {

        public string Title
        {
            get => _title.Get();
            set => _title.Set(value);
        }
        private readonly IProperty<string> _title = H.Property<string>();
        public bool Enabled
        {
            get => _enabled.Get();
            set => _enabled.Set(value);
        }
        private readonly IProperty<bool> _enabled = H.Property<bool>();
    }
}