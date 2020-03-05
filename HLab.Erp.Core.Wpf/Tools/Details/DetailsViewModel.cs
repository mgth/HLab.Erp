using System;
using HLab.Core;
using HLab.Core.Annotations;
using HLab.DependencyInjection.Annotations;
using HLab.Mvvm;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Core.Tools.Details
{
    public class DetailsModule : IBootloader //postboot
    {
        [Import] private readonly IDocumentService _docs;
        [Import] private readonly Func<DetailsViewModel> _getDetails;

        public bool Load() => true;
        //{
        //    //TODO :
        //    //_docs.OpenDocument(_getDetails());
        //}
    }

    public class DetailsViewModel : ViewModel<DetailsViewModel>
    {

        [Import]
        public DetailsViewModel(IMessageBus messageBus)
        {
            messageBus.Subscribe<DetailMessage>(a=> Item = a.Item);
        }

        public string Title => "Detail";


        public object Item
        {
            get => _item.Get();
            set => _item.Set(value);
        }
        private readonly IProperty<object> _item = H.Property<object>();
    }
}
