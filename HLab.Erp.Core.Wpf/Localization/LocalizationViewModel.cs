using System.Collections.ObjectModel;
using System.Linq;
using HLab.Erp.Base.Data;
using HLab.Erp.Data.Observables;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Localization
{
    using H = H<LocalizationViewModel>;

    internal class LocalizationViewModel : ViewModel
    {
       public ObservableQuery<LocalizeEntry> Entries { get; }
        public LocalizationViewModel(ObservableQuery<LocalizeEntry> entries)
        {
            Entries = entries;
        }

        public IObservableFilter<LocalizeEntry> Base { get; } = H.Filter<LocalizeEntry>(c => c
            .AddFilter(p => p.Tag == "en-US")
            .Link(e => e.Entries)
        );

        public ObservableCollection<string> Tags = new ObservableCollection<string>();

        public ObservableCollection<TagViewModel> Todo = new ObservableCollection<TagViewModel>();

        public void UpdateTags()
        {
            Tags.Clear();
            var lst = Entries.Select(e => e.Tag).Distinct();
            foreach (var t in lst)
            {
                Tags.Add(t);
            }
        }

        public string Tag
        {
            get => _tag.Get();
            set => _tag.Set(value);
        }
        private readonly IProperty<string> _tag = H.Property<string>();

    }
}