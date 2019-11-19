using System.ComponentModel;
using HLab.DependencyInjection.Annotations;
using HLab.Erp.Acl;
using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Erp.Data;
using HLab.Mvvm.Annotations;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public interface ICorporationViewModel
    {
        ICorporation Model { get; }
    }

    public abstract class CorporationViewModel<TClass, T> : EntityViewModel<TClass, T>, ICorporationViewModel
    where TClass : CorporationViewModel<TClass, T>
    where T : class, IEntity<int>, INotifyPropertyChanged, ICorporation, IListableModel
    {
        [Import]
        private readonly IIconService _iconService;
        [Import]
        public IErpServices Erp { get; }

        public string Title => _title.Get();
        private readonly IProperty<string> _title = H.Property<string>(c => c
            .OneWayBind(e => e.Model.Caption)
        );


        public object Icon => _iconService.GetIcon(Model.IconPath);

        ICorporation ICorporationViewModel.Model => Model;
    }

    public class CustomerViewModel : CorporationViewModel<CustomerViewModel, Customer>
    {


    }

    public class CustomerViewModelDesign : CustomerViewModel, IViewModelDesign
    {
        public CustomerViewModelDesign()
        {
            Model = Customer.GetDesignModel();
        }
    }
}
