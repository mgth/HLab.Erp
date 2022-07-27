using HLab.Erp.Base.Data;
using HLab.Erp.Core;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public interface ICorporationViewModel
    {
        ICorporation Model { get; }
    }

    public class CustomerViewModel : CorporationViewModel<Customer>
    {
        public CustomerViewModel(Injector i) : base(i)
        {
        }
    }
}
