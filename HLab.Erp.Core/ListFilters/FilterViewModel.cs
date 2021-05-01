using HLab.Erp.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;
using System;
using System.Linq.Expressions;

namespace HLab.Erp.Core.ListFilters
{
    public static class FilterViewModelExtension
    {
        public static T Header<T>(this T filter, object header)
        where T : IFilterViewModel
        {
            filter.Header = header;
            return filter;
        }
        public static T IconPath<T>(this T filter, string path)
        where T : IFilterViewModel
        {
            filter.IconPath = path;
            return filter;
        }
    }


    public abstract class FilterViewModel : ViewModel, IFilterViewModel
    {
        protected FilterViewModel() => H<FilterViewModel>.Initialize(this);
        public object Header
        {
            get => _header.Get();
            set => _header.Set(value);
        }
        private readonly IProperty<object> _header = H<FilterViewModel>.Property<object>();

        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }
        private readonly IProperty<string> _iconPath = H<FilterViewModel>.Property<string>();

        public bool Enabled
        {
            get => _enabled.Get();
            set
            {
                if(_enabled.Set(value))
                {
                    if(value) Enable();
                    else Disable();
                }
            }
        }
        private readonly IProperty<bool> _enabled = H<FilterViewModel>.Property<bool>();

        protected Action enabledAction;
        protected Action disabledAction;
        protected virtual void Enable() => enabledAction?.Invoke();

        protected virtual void Disable() => disabledAction?.Invoke();

    }
}