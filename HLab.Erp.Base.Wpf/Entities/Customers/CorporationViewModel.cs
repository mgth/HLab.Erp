using System.ComponentModel;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Application;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public abstract class CorporationViewModel<T> : ListableEntityViewModel<T>, ICorporationViewModel
    where T : class, IEntity<int>, INotifyPropertyChanged, ICorporation, IListableModel
    {
        public IErpServices Erp { get; private set; }
        public void Inject(IErpServices erp)
        {
            Erp = erp;
        }

        protected CorporationViewModel() => H<CorporationViewModel<T>>.Initialize(this);

        ICorporation ICorporationViewModel.Model => Model;
    }
}
