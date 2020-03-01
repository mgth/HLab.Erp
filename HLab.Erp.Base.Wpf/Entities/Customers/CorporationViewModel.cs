using System.ComponentModel;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public abstract class CorporationViewModel<TClass, T> : EntityViewModel<TClass, T>, ICorporationViewModel
    where TClass : CorporationViewModel<TClass, T>
    where T : class, IEntity<int>, INotifyPropertyChanged, ICorporation, IListableModel
    {
        [Import]
        public IErpServices Erp { get; }

        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c
            .OneWayBind(e => e.Model.Caption)
        );

        ICorporation ICorporationViewModel.Model => Model;
    }
}
