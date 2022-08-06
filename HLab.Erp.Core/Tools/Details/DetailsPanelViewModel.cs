using HLab.Core.Annotations;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Tools.Details
{
    public class DetailsPanelViewModel : ViewModel
    {

        public DetailsPanelViewModel(IMessagesService messageBus)
        {
            messageBus.Subscribe<DetailMessage>(a=> Item = a.Item);
            H<DetailsPanelViewModel>.Initialize(this);
        }

        public string Title => "{Detail}";


        public object Item
        {
            get => _item.Get();
            set => _item.Set(value);
        }

        readonly IProperty<object> _item = H<DetailsPanelViewModel>.Property<object>();
    }
}
