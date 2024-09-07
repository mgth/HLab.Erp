using HLab.Core.Annotations;
using HLab.Mvvm.ReactiveUI;
using ReactiveUI;

namespace HLab.Erp.Core.Tools.Details
{
    public class DetailsPanelViewModel : ViewModel
    {

        public DetailsPanelViewModel(IMessagesService messageBus)
        {
            messageBus.Subscribe<DetailMessage>(a=> Item = a.Item);
        }

        public string Title => "{Detail}";


        public object Item
        {
            get => _item;
            set => this.RaiseAndSetIfChanged(ref _item, value);
        }
        object _item;
    }
}
