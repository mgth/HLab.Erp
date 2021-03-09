using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.ListFilters
{
    public static class FilterViewModelExtension
    {
        public static T Title<T>(this T filter, string title)
        where T : IFilterViewModel
        {
            filter.Title = title;
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
        public string Title
        {
            get => _title.Get();
            set => _title.Set(value);
        }
        private readonly IProperty<string> _title = H<FilterViewModel>.Property<string>();

        public string IconPath
        {
            get => _iconPath.Get();
            set => _iconPath.Set(value);
        }
        private readonly IProperty<string> _iconPath = H<FilterViewModel>.Property<string>();

        public bool Enabled
        {
            get => _enabled.Get();
            set => _enabled.Set(value);
        }
        private readonly IProperty<bool> _enabled = H<FilterViewModel>.Property<bool>();
    }
}