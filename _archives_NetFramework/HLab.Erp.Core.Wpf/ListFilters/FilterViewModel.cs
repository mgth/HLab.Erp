using System;
using System.Collections.Specialized;
using System.Windows.Input;
using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.ListFilters
{
    public abstract class FilterViewModel : ViewModel<FilterViewModel>, IFilterViewModel
    {
        public FilterViewModel()
        {
            H.Initialize(this);
        }

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