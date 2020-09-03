using HLab.Core;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Tools.Details
{
    public class DetailsViewModel : ViewModel<DetailsViewModel>
    {

        [Import]
        public DetailsViewModel(IMessageBus messageBus)
        {
            messageBus.Subscribe<DetailMessage>(a=> Item = a.Item);
            H<DetailsViewModel>.Initialize(this);
        }

        public string Title => "Detail";


        public object Item
        {
            get => _item.Get();
            set => _item.Set(value);
        }
        private readonly IProperty<object> _item = H<DetailsViewModel>.Property<object>();
    }
}
