using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    public abstract class FilterViewModel : ViewModel, IFilterViewModel
    {
        protected FilterViewModel() => H<FilterViewModel>.Initialize(this);
        public string Title
        {
            get => _title.Get();
            set => _title.Set(value);
        }
        private readonly IProperty<string> _title = H<FilterViewModel>.Property<string>();
        public bool Enabled
        {
            get => _enabled.Get();
            set => _enabled.Set(value);
        }
        private readonly IProperty<bool> _enabled = H<FilterViewModel>.Property<bool>();
    }
}