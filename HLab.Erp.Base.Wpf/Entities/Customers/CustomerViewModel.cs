using HLab.Erp.Base.Data;
using HLab.Notify.PropertyChanged;

namespace HLab.Erp.Base.Wpf.Entities.Customers
{
    public interface ICorporationViewModel
    {
        ICorporation Model { get; }
    }

    public class CustomerViewModel : CorporationViewModel<Customer>
    {

    }
}
