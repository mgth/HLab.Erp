using System.ComponentModel;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public abstract class CorporationViewModel<T> : EntityViewModel<T>, ICorporationViewModel
    where T : class, IEntity<int>, INotifyPropertyChanged, ICorporation, IListableModel
    {
        public IErpServices Erp { get; private set; }
        public void Inject(IErpServices erp)
        {
            Erp = erp;
        }

        protected CorporationViewModel() => H<CorporationViewModel<T>>.Initialize(this);

        public override string Title => _title.Get();
        private readonly IProperty<string> _title = H<CorporationViewModel<T>>.Property<string>(c => c
            .Bind(e => e.Model.Caption)
        );

        ICorporation ICorporationViewModel.Model => Model;
    }
}
