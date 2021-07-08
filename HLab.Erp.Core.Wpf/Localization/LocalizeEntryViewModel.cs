using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Wpf.Localization
{
    using H = H<LocalizeEntryViewModel>;

    public class LocalizeEntryViewModel : EntityViewModel<LocalizeEntry>
    {
        public LocalizeEntryViewModel() => H.Initialize(this);

        public override object Header => _header.Get();

        private readonly IProperty<object> _header = H.Property<object>(c => c
            .On(e => e.Model.Code)
            .Set(e => (object)$"{e.Model.Tag} - {e.Model.Code}")
        );


    }
}
